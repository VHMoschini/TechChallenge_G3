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
}
