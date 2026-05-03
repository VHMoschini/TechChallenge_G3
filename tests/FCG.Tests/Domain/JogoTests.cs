using FCG.Domain.Entities;
using FluentAssertions;

namespace FCG.Tests.Domain;

public class JogoTests
{
    [Fact]
    public void Atualizar_altera_titulo_genero_preco()
    {
        var jogo = new Jogo("Antigo", "Acao", 10m);
        jogo.Atualizar("Novo", "RPG", 29.90m);
        jogo.Titulo.Should().Be("Novo");
        jogo.Genero.Should().Be("RPG");
        jogo.Preco.Should().Be(29.90m);
    }

    [Fact]
    public void Atualizar_titulo_vazio_lanca()
    {
        var jogo = new Jogo("X", "Y", 1m);
        var act = () => jogo.Atualizar("  ", "Y", 1m);
        act.Should().Throw<ArgumentException>().WithParameterName("titulo");
    }

    [Fact]
    public void Atualizar_preco_negativo_lanca()
    {
        var jogo = new Jogo("X", "Y", 1m);
        var act = () => jogo.Atualizar("X", "Y", -0.01m);
        act.Should().Throw<ArgumentException>().WithParameterName("preco");
    }

    [Fact]
    public void Atualizar_genero_null_vazio_string()
    {
        var jogo = new Jogo("X", "Y", 1m);
        jogo.Atualizar("Z", null!, 2m);
        jogo.Genero.Should().BeEmpty();
    }

    [Fact]
    public void Novo_jogo_esta_ativo()
    {
        var jogo = new Jogo("X", "Y", 1m);
        jogo.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Inativar_e_Reativar_alteram_Ativo()
    {
        var jogo = new Jogo("X", "Y", 1m);
        jogo.Inativar();
        jogo.Ativo.Should().BeFalse();
        jogo.Reativar();
        jogo.Ativo.Should().BeTrue();
    }
}
