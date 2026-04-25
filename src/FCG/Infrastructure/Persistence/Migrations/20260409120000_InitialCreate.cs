using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Infrastructure.Persistence.Migrations;

/// <summary>Migration inicial: Users, Games, UserGames, Promotions.</summary>
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Email = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false),
                PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Users", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);

        migrationBuilder.CreateTable(
            name: "Games",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                Genre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Games", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Promotions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                DiscountPercent = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                ValidFromUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                ValidToUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                GameId = table.Column<Guid>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Promotions", x => x.Id));

        migrationBuilder.CreateTable(
            name: "UserGames",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                GameId = table.Column<Guid>(type: "TEXT", nullable: false),
                AcquiredAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserGames", x => new { x.UserId, x.GameId });
                table.ForeignKey(
                    name: "FK_UserGames_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UserGames_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "UserGames");
        migrationBuilder.DropTable(name: "Promotions");
        migrationBuilder.DropTable(name: "Games");
        migrationBuilder.DropTable(name: "Users");
    }
}
