using FCG.Application.Contracts;
using FCG.Infrastructure.Services;
using FCG.Tests.Support;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FCG.Tests.Services;

public class GameServiceTests
{
    [Fact]
    public async Task UpdateAsync_altera_jogo_existente()
    {
        await using var db = await TestDatabase.CreateAsync();
        var jogo = new FCG.Domain.Entities.Jogo("A", "Acao", 10m);
        db.Db.Jogos.Add(jogo);
        await db.Db.SaveChangesAsync();

        var svc = new GameService(db.Db);
        var res = await svc.UpdateAsync(jogo.Id, new UpdateGameRequest("B", "RPG", 15m));

        res.Titulo.Should().Be("B");
        res.Genero.Should().Be("RPG");
        res.Preco.Should().Be(15m);

        var fromDb = await db.Db.Jogos.AsNoTracking().FirstAsync(g => g.Id == jogo.Id);
        fromDb.Titulo.Should().Be("B");
    }

    [Fact]
    public async Task UpdateAsync_jogo_inexistente_lanca_KeyNotFoundException()
    {
        await using var db = await TestDatabase.CreateAsync();
        var svc = new GameService(db.Db);
        var act = async () => await svc.UpdateAsync(Guid.NewGuid(), new UpdateGameRequest("X", "Y", 1m));
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ListAsync_nao_inclui_jogo_inativado()
    {
        await using var db = await TestDatabase.CreateAsync();
        var ativo = new FCG.Domain.Entities.Jogo("A", "Acao", 1m);
        var inativo = new FCG.Domain.Entities.Jogo("B", "RPG", 2m);
        db.Db.Jogos.AddRange(ativo, inativo);
        await db.Db.SaveChangesAsync();

        var svc = new GameService(db.Db);
        await svc.DeactivateAsync(inativo.Id);

        var list = await svc.ListAsync();
        list.Should().HaveCount(1);
        list[0].Titulo.Should().Be("A");
    }

    [Fact]
    public async Task UpdateAsync_jogo_inativado_lanca_KeyNotFoundException()
    {
        await using var db = await TestDatabase.CreateAsync();
        var jogo = new FCG.Domain.Entities.Jogo("A", "Acao", 1m);
        db.Db.Jogos.Add(jogo);
        await db.Db.SaveChangesAsync();
        var svc = new GameService(db.Db);
        await svc.DeactivateAsync(jogo.Id);

        var act = async () => await svc.UpdateAsync(jogo.Id, new UpdateGameRequest("X", "Y", 1m));
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ReactivateAsync_volta_a_aparecer_em_List()
    {
        await using var db = await TestDatabase.CreateAsync();
        var jogo = new FCG.Domain.Entities.Jogo("Z", "Acao", 1m);
        db.Db.Jogos.Add(jogo);
        await db.Db.SaveChangesAsync();
        var svc = new GameService(db.Db);
        await svc.DeactivateAsync(jogo.Id);
        (await svc.ListAsync()).Should().BeEmpty();

        await svc.ReactivateAsync(jogo.Id);

        (await svc.ListAsync()).Should().ContainSingle(x => x.Titulo == "Z");
    }
}
