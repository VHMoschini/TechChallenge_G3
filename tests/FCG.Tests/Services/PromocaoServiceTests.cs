using FCG.Application.Contracts;
using FCG.Domain.Entities;
using FCG.Infrastructure.Services;
using FCG.Tests.Support;
using FluentAssertions;

namespace FCG.Tests.Services;

public class PromocaoServiceTests
{
    private static CreatePromocaoRequest Padrao(
        string titulo = "Black Friday",
        decimal desconto = 30m,
        DateTime? inicio = null,
        DateTime? fim = null,
        Guid? gameId = null) =>
        new(titulo,
            "Promo de teste",
            desconto,
            inicio ?? DateTime.UtcNow.AddDays(-1),
            fim ?? DateTime.UtcNow.AddDays(7),
            gameId);

    [Fact]
    public async Task CreateAsync_persiste_promocao_valida()
    {
        await using var db = await TestDatabase.CreateAsync();
        var svc = new PromocaoService(db.Db);

        var res = await svc.CreateAsync(Padrao());

        res.Id.Should().NotBeEmpty();
        res.Titulo.Should().Be("Black Friday");
        res.PercentualDesconto.Should().Be(30m);
    }

    [Fact]
    public async Task CreateAsync_titulo_em_branco_lanca()
    {
        await using var db = await TestDatabase.CreateAsync();
        var svc = new PromocaoService(db.Db);
        var act = async () => await svc.CreateAsync(Padrao(titulo: "  "));
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Titulo*");
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(100.01)]
    public async Task CreateAsync_desconto_fora_de_0_a_100_lanca(decimal desconto)
    {
        await using var db = await TestDatabase.CreateAsync();
        var svc = new PromocaoService(db.Db);
        var act = async () => await svc.CreateAsync(Padrao(desconto: desconto));
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Desconto*");
    }

    [Fact]
    public async Task CreateAsync_data_inicio_apos_fim_lanca()
    {
        await using var db = await TestDatabase.CreateAsync();
        var svc = new PromocaoService(db.Db);
        var act = async () => await svc.CreateAsync(Padrao(
            inicio: DateTime.UtcNow.AddDays(2),
            fim: DateTime.UtcNow.AddDays(1)));
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*vigencia*");
    }

    [Fact]
    public async Task CreateAsync_game_id_inexistente_lanca()
    {
        await using var db = await TestDatabase.CreateAsync();
        var svc = new PromocaoService(db.Db);
        var act = async () => await svc.CreateAsync(Padrao(gameId: Guid.NewGuid()));
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*nao existe*");
    }

    [Fact]
    public async Task CreateAsync_game_id_existente_e_aceito()
    {
        await using var db = await TestDatabase.CreateAsync();
        var jogo = new Jogo("Doom", "Acao", 1m);
        db.Db.Jogos.Add(jogo);
        await db.Db.SaveChangesAsync();
        var svc = new PromocaoService(db.Db);

        var res = await svc.CreateAsync(Padrao(gameId: jogo.Id));

        res.GameId.Should().Be(jogo.Id);
    }

    [Fact]
    public async Task ListActiveAsync_inclui_apenas_promocoes_no_periodo_corrente()
    {
        await using var db = await TestDatabase.CreateAsync();
        var svc = new PromocaoService(db.Db);
        await svc.CreateAsync(Padrao(titulo: "Atual",
            inicio: DateTime.UtcNow.AddDays(-1),
            fim: DateTime.UtcNow.AddDays(1)));
        await svc.CreateAsync(Padrao(titulo: "Futura",
            inicio: DateTime.UtcNow.AddDays(5),
            fim: DateTime.UtcNow.AddDays(10)));
        await svc.CreateAsync(Padrao(titulo: "Passada",
            inicio: DateTime.UtcNow.AddDays(-30),
            fim: DateTime.UtcNow.AddDays(-1)));

        var ativas = await svc.ListActiveAsync();

        ativas.Should().ContainSingle();
        ativas[0].Titulo.Should().Be("Atual");
    }
}
