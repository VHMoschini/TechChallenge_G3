using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Infrastructure.Persistence.Migrations;

/// <summary>Migration inicial: Usuarios, Jogos, UsuarioJogos, Promocoes.</summary>
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Usuarios",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Email = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false),
                SenhaHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                Perfil = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Usuarios", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_Usuarios_Email",
            table: "Usuarios",
            column: "Email",
            unique: true);

        migrationBuilder.CreateTable(
            name: "Jogos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Titulo = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                Genero = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                Preco = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Jogos", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Promocoes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Descricao = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                PercentualDesconto = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                ValidoDeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                ValidoAteUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                JogoId = table.Column<Guid>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Promocoes", x => x.Id));

        migrationBuilder.CreateTable(
            name: "UsuarioJogos",
            columns: table => new
            {
                UsuarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                JogoId = table.Column<Guid>(type: "TEXT", nullable: false),
                AdquiridoEmUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UsuarioJogos", x => new { x.UsuarioId, x.JogoId });
                table.ForeignKey(
                    name: "FK_UsuarioJogos_Jogos_JogoId",
                    column: x => x.JogoId,
                    principalTable: "Jogos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UsuarioJogos_Usuarios_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "Usuarios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "UsuarioJogos");
        migrationBuilder.DropTable(name: "Promocoes");
        migrationBuilder.DropTable(name: "Jogos");
        migrationBuilder.DropTable(name: "Usuarios");
    }
}
