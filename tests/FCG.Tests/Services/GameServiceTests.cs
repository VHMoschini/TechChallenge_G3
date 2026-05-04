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
    public async Task CreateAsync_persiste_jogo_e_retorna_response()
    {
        await using var db = await TestDatabase.CreateAsync();
        var svc = new GameService(db.Db);

        var res = await svc.CreateAsync(new CreateGameRequest("Doom", "Acao", 49.9m));

        res.Id.Should().NotBeEmpty();
        res.Titulo.Should().Be("Doom");
        var fromDb = await db.Db.Jogos.AsNoTracking().FirstAsync();
        fromDb.Titulo.Should().Be("Doom");
        fromDb.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_titulo_em_branco_lanca()
    {
        await using var db = await TestDatabase.CreateAsync();
        var svc = new GameService(db.Db);
        var act = async () => await svc.CreateAsync(new CreateGameRequest(" ", "Acao", 1m));
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Titulo*");
    }

    [Fact]
    public async Task CreateAsync_preco_negativo_lanca()
    {
        await using var db = await TestDatabase.CreateAsync();
        var svc = new GameService(db.Db);
        var act = async () => await svc.CreateAsync(new CreateGameRequest("X", "Y", -1m));
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Preco*");
    }

    [Fact]
    public async Task DeactivateAsync_jogo_inexistente_lanca_KeyNotFoundException()
    {
        await using var db = await TestDatabase.CreateAsync();
        var svc = new GameService(db.Db);
        var act = async () => await svc.DeactivateAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ListAsync_retorna_em_ordem_alfabetica_por_titulo()
    {
        await using var db = await TestDatabase.CreateAsync();
        db.Db.Jogos.AddRange(
            new FCG.Domain.Entities.Jogo("Zelda", "Aventura", 1m),
            new FCG.Domain.Entities.Jogo("Mario", "Plataforma", 1m),
            new FCG.Domain.Entities.Jogo("Counter", "FPS", 1m));
        await db.Db.SaveChangesAsync();

        var list = await new GameService(db.Db).ListAsync();

        list.Select(g => g.Titulo).Should().ContainInOrder("Counter", "Mario", "Zelda");
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
