using static BCrypt.Net.BCrypt;
using FCG.Application.Abstractions;
using FCG.Domain.Constants;
using FCG.Domain.Entities;
using FCG.Domain.Services;
using Microsoft.Extensions.Options;

namespace FCG.Infrastructure.Seed;

public class DevAdminSeeder
{
    private readonly IUsuarioRepository _usuarios;
    private readonly DevAdminSeedOptions _options;

    public DevAdminSeeder(IUsuarioRepository usuarios, IOptions<DevAdminSeedOptions> options)
    {
        _usuarios = usuarios;
        _options = options.Value;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var email = _options.AdminEmail?.Trim().ToLowerInvariant();
        var password = _options.AdminPassword;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        if (await _usuarios.EmailExistsAsync(email, cancellationToken))
            return;

        if (!CredentialValidation.IsStrongPassword(password, out _))
            return;

        var hash = HashPassword(password);
        var admin = new Usuario(_options.AdminName.Trim(), email, hash, Roles.Administrador);
        await _usuarios.AddAsync(admin, cancellationToken);
    }
}
