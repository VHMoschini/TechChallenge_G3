using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FCG.Application.Abstractions;
using FCG.Infrastructure.Persistence;
using FCG.Infrastructure.Security;
using FCG.Infrastructure.Seed;
using FCG.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace FCG.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' nao configurada.");

        connectionString = ResolveSqliteDataSourcePath(connectionString, hostEnvironment.ContentRootPath);

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<DevAdminSeedOptions>(configuration.GetSection(DevAdminSeedOptions.SectionName));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<ILibraryService, LibraryService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<DevAdminSeeder>();

        return services;
    }

    /// <summary>
    /// Evita <c>Data Source=fcg.db</c> relativo ao diretório de trabalho do processo; fixa em ContentRoot.
    /// </summary>
    private static string ResolveSqliteDataSourcePath(string connectionString, string contentRootPath)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);
        var ds = builder.DataSource;
        if (string.IsNullOrEmpty(ds)
            || ds.Equals(":memory:", StringComparison.OrdinalIgnoreCase)
            || Path.IsPathRooted(ds))
            return connectionString;

        builder.DataSource = Path.GetFullPath(Path.Combine(contentRootPath, ds));
        return builder.ConnectionString;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("Secao Jwt ausente em appsettings.");

        if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
            throw new InvalidOperationException("Jwt:Key deve ter pelo menos 32 caracteres.");

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

        services.AddAuthorization();
        return services;
    }
}
