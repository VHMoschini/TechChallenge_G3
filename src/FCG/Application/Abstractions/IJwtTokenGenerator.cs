using FCG.Domain.Entities;

namespace FCG.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string CreateToken(Usuario usuario, DateTime utcNow);
    DateTime GetExpiryUtc(DateTime utcNow);
}
