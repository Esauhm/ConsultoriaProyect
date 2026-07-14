using Consultoria.Application.Common.Pagination;
using Consultoria.Application.DTOs.Reportes;
using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Services
{
    public sealed class ReporteService : IReporteService
    {
        private static readonly TimeSpan DuracionCache =
            TimeSpan.FromMinutes(5);

        private readonly IReporteRepository _reporteRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ReporteService> _logger;

        public ReporteService(
            IReporteRepository reporteRepository,
            ICacheService cacheService,
            ILogger<ReporteService> logger)
        {
            _reporteRepository = reporteRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<PagedResult<PaquetesPorAreaDto>>
            ObtenerPaquetesPorAreaAsync(
                PaquetesPorAreaRequestDto request,
                CancellationToken cancellationToken = default)
        {
            string claveCache = CrearClavePaquetesPorArea(request);

            PagedResult<PaquetesPorAreaDto>? resultadoCache =
                _cacheService.Obtener<PagedResult<PaquetesPorAreaDto>>(
                    claveCache);

            if (resultadoCache is not null)
            {
                _logger.LogInformation(
                    "Reporte de paquetes por área obtenido desde caché. Clave: {CacheKey}.",
                    claveCache);

                return resultadoCache;
            }

            PagedResult<PaquetesPorAreaDto> resultado =
                await _reporteRepository.ObtenerPaquetesPorAreaAsync(
                    request,
                    cancellationToken);

            _cacheService.Guardar(
                claveCache,
                resultado,
                DuracionCache);

            _logger.LogInformation(
                "Reporte de paquetes por área generado. Página {Page}, tamaño {PageSize}, total {TotalCount}.",
                resultado.Page,
                resultado.PageSize,
                resultado.TotalCount);

            return resultado;
        }

        public async Task<PagedResult<ConsultorTopFacturacionDto>>
            ObtenerConsultoresTopFacturacionAsync(
                ConsultoresTopFacturacionRequestDto request,
                CancellationToken cancellationToken = default)
        {
            string claveCache =
                CrearClaveConsultoresTopFacturacion(request);

            PagedResult<ConsultorTopFacturacionDto>? resultadoCache =
                _cacheService.Obtener<
                    PagedResult<ConsultorTopFacturacionDto>>(
                    claveCache);

            if (resultadoCache is not null)
            {
                _logger.LogInformation(
                    "Reporte de consultores por facturación obtenido desde caché. Clave: {CacheKey}.",
                    claveCache);

                return resultadoCache;
            }

            PagedResult<ConsultorTopFacturacionDto> resultado =
                await _reporteRepository
                    .ObtenerConsultoresTopFacturacionAsync(
                        request,
                        cancellationToken);

            _cacheService.Guardar(
                claveCache,
                resultado,
                DuracionCache);

            _logger.LogInformation(
                "Reporte de consultores por facturación generado. Página {Page}, tamaño {PageSize}, total {TotalCount}.",
                resultado.Page,
                resultado.PageSize,
                resultado.TotalCount);

            return resultado;
        }

        private static string CrearClavePaquetesPorArea(
            PaquetesPorAreaRequestDto request)
        {
            string nombrePaquete = request.NombrePaquete?
                .Trim()
                .ToLowerInvariant() ?? "todos";

            string sortBy = request.SortBy?
                .Trim()
                .ToLowerInvariant() ?? "default";

            string sortDir = request.SortDir
                .Trim()
                .ToLowerInvariant();

            return string.Join(
                ":",
                "reportes",
                "paquetes-por-area",
                request.Page,
                request.PageSize,
                request.AreaEspecializacionId?.ToString() ?? "todas",
                nombrePaquete,
                request.Activo?.ToString() ?? "todos",
                sortBy,
                sortDir);
        }

        private static string CrearClaveConsultoresTopFacturacion(
            ConsultoresTopFacturacionRequestDto request)
        {
            string nombreConsultor = request.NombreConsultor?
                .Trim()
                .ToLowerInvariant() ?? "todos";

            string sortBy = request.SortBy?
                .Trim()
                .ToLowerInvariant() ?? "default";

            string sortDir = request.SortDir
                .Trim()
                .ToLowerInvariant();

            return string.Join(
                ":",
                "reportes",
                "consultores-top-facturacion",
                request.Page,
                request.PageSize,
                request.AreaEspecializacionId?.ToString() ?? "todas",
                nombreConsultor,
                request.Activo?.ToString() ?? "todos",
                sortBy,
                sortDir);
        }
    }
}
