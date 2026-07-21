using Consultoria.Api.Filters;
using Consultoria.Api.Middleware;
using Consultoria.Api.OpenApi;
using Consultoria.Application;
using Consultoria.Infrastructure;
using Consultoria.Infrastructure.Authentication;
using Consultoria.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. Controladores
// ---------------------------------------------------------

builder.Services.AddScoped<ValidationFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ValidationFilter>();
});

// ---------------------------------------------------------
// 2. Documentación OpenAPI
// ---------------------------------------------------------

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<
        BearerSecuritySchemeTransformer>();

    options.AddOperationTransformer<
        BearerSecurityRequirementTransformer>();
});

// ---------------------------------------------------------
// 3. Capas de la aplicación
// ---------------------------------------------------------

builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration);

// ---------------------------------------------------------
// Health Checks
// ---------------------------------------------------------

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ConsultoriaDbContext>(
        name: "sqlserver",
        tags: ["ready"]);


// ---------------------------------------------------------
// 4. Leer configuración JWT
// ---------------------------------------------------------

IConfigurationSection jwtSection =
    builder.Configuration.GetRequiredSection(
        JwtSettings.SectionName);

string jwtKey =
    jwtSection["Key"]
    ?? throw new InvalidOperationException(
        "No se encontró la configuración 'Jwt:Key'.");

string jwtIssuer =
    jwtSection["Issuer"]
    ?? throw new InvalidOperationException(
        "No se encontró la configuración 'Jwt:Issuer'.");

string jwtAudience =
    jwtSection["Audience"]
    ?? throw new InvalidOperationException(
        "No se encontró la configuración 'Jwt:Audience'.");

// ---------------------------------------------------------
// 5. Autenticación JWT
// ---------------------------------------------------------

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateLifetime = true,

                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role,

                ClockSkew = TimeSpan.Zero
            };
    });

// ---------------------------------------------------------
// 6. Autorización
// ---------------------------------------------------------

builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<
    GlobalExceptionHandler>();

// ---------------------------------------------------------
// 7. Construir aplicación
// ---------------------------------------------------------

var app = builder.Build();

// ---------------------------------------------------------
// 8. OpenAPI y Swagger solo en desarrollo
// ---------------------------------------------------------

bool isDevelopment =
    app.Environment.IsDevelopment();

bool isTesting =
    app.Environment.IsEnvironment("Testing");

// Aplicar migraciones en desarrollo y pruebas de integración
if (isDevelopment || isTesting)
{
    await using var scope =
        app.Services.CreateAsyncScope();

    ConsultoriaDbContext dbContext =
        scope.ServiceProvider
            .GetRequiredService<ConsultoriaDbContext>();

    await dbContext.Database.MigrateAsync();
}

// OpenAPI y Swagger únicamente en desarrollo
if (isDevelopment)
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Consultoria API v1");
    });
}

// ---------------------------------------------------------
// 9. Pipeline HTTP
// ---------------------------------------------------------

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// Comprueba únicamente que la API está viva.
app.MapHealthChecks(
    "/health/live",
    new HealthCheckOptions
    {
        Predicate = _ => false
    });

// Comprueba dependencias necesarias para operar,
// actualmente SQL Server.
app.MapHealthChecks(
    "/health/ready",
    new HealthCheckOptions
    {
        Predicate = healthCheck =>
            healthCheck.Tags.Contains("ready")
    });

app.MapControllers();

app.Run();

// ---------------------------------------------------------
// Clase parcial requerida para habilitar la infraestructura de pruebas de integración.
// ---------------------------------------------------------

public partial class Program{ }