using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Jogo> Jogos => Set<Jogo>();
    public DbSet<UsuarioJogo> UsuarioJogos => Set<UsuarioJogo>();
    public DbSet<Promocao> Promocoes => Set<Promocao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Jogo>(entity =>
        {
            entity.ToTable("Games");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(300);
            entity.Property(e => e.Genre).HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<UsuarioJogo>(entity =>
        {
            entity.ToTable("UserGames");
            entity.HasKey(e => new { e.UsuarioId, e.JogoId });
            entity.Property(e => e.UsuarioId).HasColumnName("UserId");
            entity.Property(e => e.JogoId).HasColumnName("GameId");
            entity.HasOne<Usuario>().WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Jogo>().WithMany().HasForeignKey(e => e.JogoId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Promocao>(entity =>
        {
            entity.ToTable("Promotions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.DiscountPercent).HasPrecision(5, 2);
        });
    }
}
