using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Application.Interfaces.Services;
using Consultoria.Infrastructure.Authentication;
using Consultoria.Infrastructure.Caching;
using Consultoria.Infrastructure.Persistence.Context;
using Consultoria.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            RegistrarPersistencia(services, configuration);
            RegistrarRepositorios(services);
            RegistrarAutenticacion(services, configuration);
            RegistrarCache(services);

            return services;
        }

        private static void RegistrarPersistencia(
            IServiceCollection services,
            IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'DefaultConnection'.");

            services.AddDbContext<ConsultoriaDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }

        private static void RegistrarRepositorios(
            IServiceCollection services)
        {
            services.AddScoped<
                IConsultorRepository,
                ConsultorRepository>();

            services.AddScoped<
                IPaqueteServicioRepository,
                PaqueteServicioRepository>();

            services.AddScoped<
                IUsuarioRepository,
                UsuarioRepository>();

            services.AddScoped<
                IReporteRepository,
                ReporteRepository>();
        }

        private static void RegistrarAutenticacion(
            IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<JwtSettings>()
                .Bind(
                    configuration.GetRequiredSection(
                        JwtSettings.SectionName))
                .Validate(
                    settings =>
                        !string.IsNullOrWhiteSpace(settings.Key),
                    "La clave JWT es obligatoria.")
                .Validate(
                    settings =>
                        Encoding.UTF8.GetByteCount(settings.Key) >= 32,
                    "La clave JWT debe contener al menos 32 bytes.")
                .Validate(
                    settings =>
                        !string.IsNullOrWhiteSpace(settings.Issuer),
                    "El emisor JWT es obligatorio.")
                .Validate(
                    settings =>
                        !string.IsNullOrWhiteSpace(settings.Audience),
                    "La audiencia JWT es obligatoria.")
                .Validate(
                    settings =>
                        settings.ExpirationMinutes > 0,
                    "La expiración del JWT debe ser mayor que cero.")
                .ValidateOnStart();

            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            services.AddSingleton<ITokenService, JwtTokenService>();
        }

        private static void RegistrarCache(
            IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddSingleton<ICacheService, MemoryCacheService>();
        }
    }
}
