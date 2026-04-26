using FCG.Domain.Entities;
using FCG.Infrastructure.Services;
using FCG.Tests.Support;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FCG.Tests.Services;

public class BibliotecaServiceTests
{
    [Fact]
    public async Task Acquire_Remove_GetMyLibrary_fluxo_basico()
    {
        await using var db = await TestDatabase.CreateAsync();
        var usuario = new Usuario("U", "u@test.com", "h");
        db.Db.Usuarios.Add(usuario);

        var jogo = new Jogo("G1", "Acao", 5m);
        db.Db.Jogos.Add(jogo);
        await db.Db.SaveChangesAsync();
        var userId = usuario.Id;

        var svc = new BibliotecaService(db.Db);
        await svc.AcquireAsync(userId, jogo.Id);

        var lib = await svc.GetMyLibraryAsync(userId);
        lib.Should().HaveCount(1);
        lib[0].Titulo.Should().Be("G1");

        await svc.RemoveAsync(userId, jogo.Id);

        var lib2 = await svc.GetMyLibraryAsync(userId);
        lib2.Should().BeEmpty();
    }

    [Fact]
    public async Task Remove_jogo_que_nao_esta_na_biblioteca_lanca()
    {
        await using var db = await TestDatabase.CreateAsync();
        db.Db.Usuarios.Add(new Usuario("U", "u@test.com", "h"));
        await db.Db.SaveChangesAsync();
        var userId = (await db.Db.Usuarios.FirstAsync()).Id;

        var jogo = new Jogo("G1", "Acao", 5m);
        db.Db.Jogos.Add(jogo);
        await db.Db.SaveChangesAsync();

        var svc = new BibliotecaService(db.Db);
        var act = async () => await svc.RemoveAsync(userId, jogo.Id);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*nao esta na sua biblioteca*");
    }

    [Fact]
    public async Task Acquire_mesmo_jogo_duas_vezes_lanca()
    {
        await using var db = await TestDatabase.CreateAsync();
        var usuario = new Usuario("U", "u@test.com", "h");
        db.Db.Usuarios.Add(usuario);
        var jogo = new Jogo("G1", "Acao", 5m);
        db.Db.Jogos.Add(jogo);
        await db.Db.SaveChangesAsync();

        var svc = new BibliotecaService(db.Db);
        await svc.AcquireAsync(usuario.Id, jogo.Id);
        var act = async () => await svc.AcquireAsync(usuario.Id, jogo.Id);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*ja esta na sua biblioteca*");
    }

    [Fact]
    public async Task Acquire_jogo_inexistente_lanca_KeyNotFoundException()
    {
        await using var db = await TestDatabase.CreateAsync();
        db.Db.Usuarios.Add(new Usuario("U", "u@test.com", "h"));
        await db.Db.SaveChangesAsync();
        var userId = (await db.Db.Usuarios.FirstAsync()).Id;

        var svc = new BibliotecaService(db.Db);
        var act = async () => await svc.AcquireAsync(userId, Guid.NewGuid());
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
