using static BCrypt.Net.BCrypt;
using FCG.Application.Abstractions;
using FCG.Application.Contracts;
using FCG.Domain.Services;
using FCG.Infrastructure.Persistence;

namespace FCG.Infrastructure.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly AppDbContext _db;

    public UserProfileService(IUsuarioRepository usuarios, AppDbContext db)
    {
        _usuarios = usuarios;
        _db = db;
    }

    public async Task<UserSummaryResponse> UpdateMyProfileAsync(Guid userId, UpdateMyProfileRequest request, CancellationToken cancellationToken = default)
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

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (request.NewPassword != request.ConfirmPassword)
            throw new InvalidOperationException("A nova senha e a confirmacao nao conferem.");

        var usuario = await _usuarios.GetByIdAsync(userId, cancellationToken);
        if (usuario is null)
            throw new KeyNotFoundException("Usuario nao encontrado.");

        if (string.IsNullOrEmpty(request.CurrentPassword))
            throw new InvalidOperationException("Informe a senha atual.");

        if (!Verify(request.CurrentPassword, usuario.SenhaHash))
            throw new UnauthorizedAccessException("Senha atual invalida.");

        if (!CredentialValidation.IsStrongPassword(request.NewPassword, out var pwdError))
            throw new InvalidOperationException(pwdError!);

        usuario.DefinirSenhaHash(HashPassword(request.NewPassword));
        usuario.RotacionarCredencialJwt();
        await _db.SaveChangesAsync(cancellationToken);
    }
}
