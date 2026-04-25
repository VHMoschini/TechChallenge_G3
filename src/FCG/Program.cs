using FCG.Infrastructure;
using FCG.Infrastructure.Persistence;
using FCG.Infrastructure.Seed;
using FCG.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FIAP Cloud Games (FCG) API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT no header Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();

    if (!await SqliteUsersTableExistsAsync(db))
    {
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();

        if (!await SqliteUsersTableExistsAsync(db))
            throw new InvalidOperationException(
                "Nao foi possivel criar o esquema (tabela Users). Confirme que as migracoes EF estao no projeto e tente apagar o ficheiro fcg.db na pasta do projeto (ContentRoot).");
    }

    if (app.Environment.IsDevelopment())
        await scope.ServiceProvider.GetRequiredService<DevAdminSeeder>().SeedAsync();
}

app.MapControllers();

app.MapGet("/health", [AllowAnonymous] () => Results.Ok(new { status = "ok" }));

await app.RunAsync();

static async Task<bool> SqliteUsersTableExistsAsync(AppDbContext db, CancellationToken cancellationToken = default)
{
    const string sql = "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Users' LIMIT 1;";
    await db.Database.OpenConnectionAsync(cancellationToken);
    try
    {
        await using var cmd = db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = sql;
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return result is not null && result is not DBNull;
    }
    finally
    {
        await db.Database.CloseConnectionAsync();
    }
}
