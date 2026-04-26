using FCG.Infrastructure;
using FCG.Infrastructure.Persistence;
using FCG.Infrastructure.Seed;
using FCG.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, _, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();

    var newRelicEnabled = context.Configuration.GetValue<bool>("NewRelic:Enabled");
    if (!newRelicEnabled)
        return;

    var endpointUrl = context.Configuration["NewRelic:LogsEndpointUrl"] ?? "https://log-api.newrelic.com/log/v1";
    var applicationName = context.Configuration["NewRelic:ApplicationName"] ?? "FCG";
    var licenseKey = context.Configuration["NewRelic:LicenseKey"] ?? string.Empty;
    var insertKey = context.Configuration["NewRelic:InsertKey"] ?? string.Empty;

    if (string.IsNullOrWhiteSpace(licenseKey) && string.IsNullOrWhiteSpace(insertKey))
        return;

    loggerConfiguration.WriteTo.NewRelicLogs(
        endpointUrl: endpointUrl,
        applicationName: applicationName,
        licenseKey: licenseKey,
        insertKey: insertKey,
        restrictedToMinimumLevel: LogEventLevel.Information);
});

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddJwtAuthentication(builder.Configuration);
ConfigureOpenTelemetry(builder.Services, builder.Configuration);

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

    if (!await SqliteTabelaUsuariosExisteAsync(db))
    {
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();

        if (!await SqliteTabelaUsuariosExisteAsync(db))
            throw new InvalidOperationException(
                "Nao foi possivel criar o esquema (tabela Usuarios). Confirme que as migracoes EF estao no projeto e tente apagar o ficheiro fcg.db na pasta do projeto (ContentRoot).");
    }

    if (app.Environment.IsDevelopment())
        await scope.ServiceProvider.GetRequiredService<DevAdminSeeder>().SeedAsync();
}

app.MapControllers();

app.MapGet("/health", [AllowAnonymous] () => Results.Ok(new { status = "ok" }));

await app.RunAsync();

static void ConfigureOpenTelemetry(IServiceCollection services, IConfiguration configuration)
{
    var enabled = configuration.GetValue<bool>("Observability:Enabled");
    if (!enabled)
        return;

    var serviceName = configuration["Observability:ServiceName"] ?? "FCG";
    var serviceVersion = configuration["Observability:ServiceVersion"] ?? "1.0.0";
    var otlpEndpoint = configuration["Observability:OtlpEndpoint"] ?? string.Empty;
    var otlpHeaders = configuration["Observability:OtlpHeaders"] ?? string.Empty;

    services
        .AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService(serviceName: serviceName, serviceVersion: serviceVersion))
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
            {
                tracing.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpEndpoint);
                    if (!string.IsNullOrWhiteSpace(otlpHeaders))
                        options.Headers = otlpHeaders;
                });
            }
        })
        .WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation();

            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
            {
                metrics.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpEndpoint);
                    if (!string.IsNullOrWhiteSpace(otlpHeaders))
                        options.Headers = otlpHeaders;
                });
            }
        });
}

static async Task<bool> SqliteTabelaUsuariosExisteAsync(AppDbContext db, CancellationToken cancellationToken = default)
{
    const string sql = "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Usuarios' LIMIT 1;";
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
