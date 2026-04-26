using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Infrastructure.Persistence.Migrations;

/// <summary>Adiciona CredencialVersao em Usuarios para invalidar JWT apos troca de senha ou rotacao.</summary>
public partial class AddUsuarioCredencialVersao : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "CredencialVersao",
            table: "Usuarios",
            type: "INTEGER",
            nullable: false,
            defaultValue: 1);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CredencialVersao",
            table: "Usuarios");
    }
}
