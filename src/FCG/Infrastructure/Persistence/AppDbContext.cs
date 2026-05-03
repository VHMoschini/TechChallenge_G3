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
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).HasMaxLength(200).HasColumnName("Nome");
            entity.Property(e => e.Email).HasMaxLength(320).HasColumnName("Email");
            entity.Property(e => e.SenhaHash).HasMaxLength(500).HasColumnName("SenhaHash");
            entity.Property(e => e.Perfil).HasMaxLength(50).HasColumnName("Perfil");
            entity.Property(e => e.CredencialVersao).HasColumnName("CredencialVersao");
            entity.Property(e => e.Ativo).HasColumnName("Ativo");
            entity.HasIndex(e => e.Email).IsUnique().HasFilter("\"Ativo\" = 1");
            entity.HasQueryFilter(e => e.Ativo);
        });

        modelBuilder.Entity<Jogo>(entity =>
        {
            entity.ToTable("Jogos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).HasMaxLength(300).HasColumnName("Titulo");
            entity.Property(e => e.Genero).HasMaxLength(100).HasColumnName("Genero");
            entity.Property(e => e.Preco).HasPrecision(18, 2).HasColumnName("Preco");
            entity.Property(e => e.Ativo).HasColumnName("Ativo");
            entity.HasQueryFilter(e => e.Ativo);
        });

        modelBuilder.Entity<UsuarioJogo>(entity =>
        {
            entity.ToTable("UsuarioJogos");
            entity.HasKey(e => new { e.UsuarioId, e.JogoId });
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioId");
            entity.Property(e => e.JogoId).HasColumnName("JogoId");
            entity.Property(e => e.AdquiridoEmUtc).HasColumnName("AdquiridoEmUtc");
            entity.HasOne<Usuario>().WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Jogo>().WithMany().HasForeignKey(e => e.JogoId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Promocao>(entity =>
        {
            entity.ToTable("Promocoes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).HasMaxLength(200).HasColumnName("Titulo");
            entity.Property(e => e.Descricao).HasMaxLength(2000).HasColumnName("Descricao");
            entity.Property(e => e.PercentualDisconto).HasPrecision(5, 2).HasColumnName("PercentualDesconto");
            entity.Property(e => e.DataPromoInicio).HasColumnName("ValidoDeUtc");
            entity.Property(e => e.DataPromoFim).HasColumnName("ValidoAteUtc");
            entity.Property(e => e.GameId).HasColumnName("JogoId");
        });
    }
}
