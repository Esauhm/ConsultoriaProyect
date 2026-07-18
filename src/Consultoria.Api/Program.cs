using Consultoria.Api.Filters;
using Consultoria.Api.Middleware;
using Consultoria.Api.OpenApi;
using Consultoria.Application;
using Consultoria.Infrastructure;
using Consultoria.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Consultoria.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

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

if (app.Environment.IsDevelopment())
{
    await using AsyncServiceScope scope =
        app.Services.CreateAsyncScope();

    ConsultoriaDbContext dbContext =
        scope.ServiceProvider
            .GetRequiredService<ConsultoriaDbContext>();

    await dbContext.Database.MigrateAsync();

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

app.MapControllers();

app.Run();