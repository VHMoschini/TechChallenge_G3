using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using FCG.Application.Abstractions;
using FCG.Domain.Entities;
using FCG.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace FCG.Tests.Security;

public class JwtTokenGeneratorTests
{
    private static IJwtTokenGenerator CreateGenerator()
    {
        var opt = Options.Create(new JwtOptions
        {
            Key = "FCG_CHAVE_LOCAL_MINIMO_32_CARACTERES_PARA_HS256_SEGURO",
            Issuer = "FCG",
            Audience = "FCG",
            ExpiresMinutes = 60
        });
        return new JwtTokenGenerator(opt);
    }

    [Fact]
    public void CreateToken_inclui_claim_cred_v_igual_a_CredencialVersao()
    {
        var usuario = new Usuario("Ana", "ana@test.com", "hash");
        usuario.RotacionarCredencialJwt();
        usuario.RotacionarCredencialJwt();

        var token = CreateGenerator().CreateToken(usuario, DateTime.UtcNow);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var cred = jwt.Claims.FirstOrDefault(c => c.Type == JwtCustomClaims.CredencialVersao);

        cred.Should().NotBeNull();
        cred!.Value.Should().Be("3");
    }
}
