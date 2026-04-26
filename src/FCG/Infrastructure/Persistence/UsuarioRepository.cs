using FCG.Application.Abstractions;
using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Persistence;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _db;

    public UsuarioRepository(AppDbContext db) => _db = db;

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<Usuario?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
        _db.Usuarios.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

    public async Task<IReadOnlyList<Usuario>> ListAsync(CancellationToken cancellationToken = default) =>
        await _db.Usuarios.AsNoTracking().OrderBy(u => u.Nome).ToListAsync(cancellationToken);

    public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await _db.Usuarios.AddAsync(usuario, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
