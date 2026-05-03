using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Domain.Constants;
using FCG.Domain.Services;
using FCG.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Services;

public class AdminUserService : IAdminUserService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly AppDbContext _db;

    public AdminUserService(IUsuarioRepository usuarios, AppDbContext db)
    {
        _usuarios = usuarios;
        _db = db;
    }

    public async Task<IReadOnlyList<UserSummaryResponse>> ListUsersAsync(CancellationToken cancellationToken = default)
    {
        var usuarios = await _usuarios.ListAsync(cancellationToken);
        return usuarios.Select(u => new UserSummaryResponse(u.Id, u.Nome, u.Email, u.Perfil)).ToList();
    }

    public async Task UpdateRoleAsync(Guid userId, UpdateUserRoleRequest request, CancellationToken cancellationToken = default)
    {
        if (!Roles.IsValid(request.Role))
            throw new InvalidOperationException("Perfil invalido. Use Usuario ou Administrador.");

        var usuario = await _usuarios.GetByIdAsync(userId, cancellationToken);
        if (usuario is null)
            throw new KeyNotFoundException("Usuario nao encontrado.");

        usuario.DefinirPerfil(request.Role);
        usuario.RotacionarCredencialJwt();
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserSummaryResponse> UpdateUserAsync(Guid userId, UpdateUserAdminRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new InvalidOperationException("Nome e obrigatorio.");

        var email = request.Email.Trim().ToLowerInvariant();
        if (!CredentialValidation.IsValidEmail(email))
            throw new InvalidOperationException("E-mail invalido.");

        var usuario = await _usuarios.GetByIdAsync(userId, cancellationToken);
        if (usuario is null)
            throw new KeyNotFoundException("Usuario nao encontrado.");

        if (await _usuarios.EmailTakenByAnotherUserAsync(email, userId, cancellationToken))
            throw new InvalidOperationException("E-mail ja cadastrado.");

        var emailMudou = usuario.Email != email;
        usuario.AtualizarDados(request.Name.Trim(), email);
        if (emailMudou)
            usuario.RotacionarCredencialJwt();

        await _db.SaveChangesAsync(cancellationToken);

        return new UserSummaryResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.Perfil);
    }

    public async Task DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var usuario = await _db.Usuarios.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (usuario is null)
            throw new KeyNotFoundException("Usuario nao encontrado.");
        usuario.Inativar();
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ReactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var usuario = await _db.Usuarios.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (usuario is null)
            throw new KeyNotFoundException("Usuario nao encontrado.");
        usuario.Reativar();
        await _db.SaveChangesAsync(cancellationToken);
    }
}
