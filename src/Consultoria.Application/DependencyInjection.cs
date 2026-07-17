using Consultoria.Application.Interfaces.Services;
using Consultoria.Application.Services;
using Consultoria.Application.Validators.Auth;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<
                IConsultorService,
                ConsultorService>();

            services.AddScoped<
                IPaqueteServicioService,
                PaqueteServicioService>();

            services.AddScoped<
                IReporteService,
                ReporteService>();

            services.AddValidatorsFromAssemblyContaining<
                LoginRequestValidator>(
                    ServiceLifetime.Scoped);

            services.AddScoped<
                IAreaEspecializacionService,
                AreaEspecializacionService>();

            return services;
        }
    }
}
