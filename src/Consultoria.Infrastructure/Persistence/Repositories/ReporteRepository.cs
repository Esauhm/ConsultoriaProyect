using Consultoria.Application.Common.Pagination;
using Consultoria.Application.DTOs.Reportes;
using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Infrastructure.Persistence.Repositories
{
    public sealed class ReporteRepository : IReporteRepository
    {
        private readonly ConsultoriaDbContext _context;

        public ReporteRepository(ConsultoriaDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<PaquetesPorAreaDto>>
            ObtenerPaquetesPorAreaAsync(
                PaquetesPorAreaRequestDto request,
                CancellationToken cancellationToken = default)
        {
            var paquetesQuery = _context.PaquetesServicio
                .AsNoTracking()
                .AsQueryable();

            if (request.AreaEspecializacionId.HasValue)
            {
                paquetesQuery = paquetesQuery.Where(
                    paquete =>
                        paquete.AreaEspecializacionId ==
                        request.AreaEspecializacionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.NombrePaquete))
            {
                string nombrePaquete = request.NombrePaquete.Trim();

                paquetesQuery = paquetesQuery.Where(
                    paquete => paquete.Nombre.Contains(nombrePaquete));
            }

            if (request.Activo.HasValue)
            {
                paquetesQuery = paquetesQuery.Where(
                    paquete => paquete.Activo == request.Activo.Value);
            }

            IQueryable<PaquetesPorAreaDto> reporteQuery =
                from paquete in paquetesQuery

                join area in _context.AreasEspecializacion.AsNoTracking()
                    on paquete.AreaEspecializacionId
                    equals area.AreaEspecializacionId

                group paquete by new
                {
                    area.AreaEspecializacionId,
                    area.Nombre
                }
                into grupo

                select new PaquetesPorAreaDto
                {
                    AreaEspecializacionId =
                        grupo.Key.AreaEspecializacionId,

                    AreaEspecializacion = grupo.Key.Nombre,

                    CantidadPaquetes = grupo.Count(),

                    TotalHoras = grupo.Sum(
                        paquete => paquete.DuracionHoras),

                    CostoTotal = grupo.Sum(
                        paquete => paquete.Costo),

                    CostoPromedio = grupo.Average(
                        paquete => paquete.Costo)
                };

            reporteQuery = AplicarOrdenamientoPaquetesPorArea(
                reporteQuery,
                request.SortBy,
                request.SortDir);

            int totalCount = await reporteQuery.CountAsync(
                cancellationToken);

            List<PaquetesPorAreaDto> items = await reporteQuery
                .Skip(request.Offset)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<PaquetesPorAreaDto>(
                items,
                totalCount,
                request.Page,
                request.PageSize);
        }

        public async Task<PagedResult<ConsultorTopFacturacionDto>>
            ObtenerConsultoresTopFacturacionAsync(
                ConsultoresTopFacturacionRequestDto request,
                CancellationToken cancellationToken = default)
        {
            var facturacionQuery =
                from paquete in _context.PaquetesServicio.AsNoTracking()

                join consultor in _context.Consultores.AsNoTracking()
                    on paquete.ConsultorId
                    equals consultor.ConsultorId

                join area in _context.AreasEspecializacion.AsNoTracking()
                    on consultor.AreaEspecializacionId
                    equals area.AreaEspecializacionId

                select new
                {
                    Paquete = paquete,
                    Consultor = consultor,
                    Area = area
                };

            if (request.AreaEspecializacionId.HasValue)
            {
                facturacionQuery = facturacionQuery.Where(
                    item =>
                        item.Consultor.AreaEspecializacionId ==
                        request.AreaEspecializacionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.NombreConsultor))
            {
                string nombreConsultor =
                    request.NombreConsultor.Trim();

                facturacionQuery = facturacionQuery.Where(
                    item =>
                        item.Consultor.Nombre.Contains(nombreConsultor));
            }

            if (request.Activo.HasValue)
            {
                facturacionQuery = facturacionQuery.Where(
                    item =>
                        item.Consultor.Activo == request.Activo.Value);
            }

            IQueryable<ConsultorTopFacturacionDto> reporteQuery =
                from item in facturacionQuery

                group item by new
                {
                    item.Consultor.ConsultorId,
                    item.Consultor.Nombre,
                    AreaEspecializacion = item.Area.Nombre,
                    item.Consultor.TarifaHora
                }
                into grupo

                select new ConsultorTopFacturacionDto
                {
                    ConsultorId = grupo.Key.ConsultorId,

                    NombreConsultor = grupo.Key.Nombre,

                    AreaEspecializacion =
                        grupo.Key.AreaEspecializacion,

                    TarifaHora = grupo.Key.TarifaHora,

                    CantidadPaquetes = grupo.Count(),

                    TotalHoras = grupo.Sum(
                        item => item.Paquete.DuracionHoras),

                    TotalFacturado = grupo.Sum(
                        item => item.Paquete.Costo)
                };

            reporteQuery =
                AplicarOrdenamientoConsultoresFacturacion(
                    reporteQuery,
                    request.SortBy,
                    request.SortDir);

            int totalCount = await reporteQuery.CountAsync(
                cancellationToken);

            List<ConsultorTopFacturacionDto> items =
                await reporteQuery
                    .Skip(request.Offset)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

            return new PagedResult<ConsultorTopFacturacionDto>(
                items,
                totalCount,
                request.Page,
                request.PageSize);
        }

        private static IQueryable<PaquetesPorAreaDto>
            AplicarOrdenamientoPaquetesPorArea(
                IQueryable<PaquetesPorAreaDto> query,
                string? sortBy,
                string sortDir)
        {
            string campo = sortBy?
                .Trim()
                .ToLowerInvariant()
                ?? "areaespecializacion";

            bool descendente = sortDir.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase);

            return (campo, descendente) switch
            {
                ("areaespecializacion", true) =>
                    query
                        .OrderByDescending(
                            item => item.AreaEspecializacion)
                        .ThenBy(item => item.AreaEspecializacionId),

                ("cantidadpaquetes", false) =>
                    query
                        .OrderBy(item => item.CantidadPaquetes)
                        .ThenBy(item => item.AreaEspecializacionId),

                ("cantidadpaquetes", true) =>
                    query
                        .OrderByDescending(
                            item => item.CantidadPaquetes)
                        .ThenBy(item => item.AreaEspecializacionId),

                ("totalhoras", false) =>
                    query
                        .OrderBy(item => item.TotalHoras)
                        .ThenBy(item => item.AreaEspecializacionId),

                ("totalhoras", true) =>
                    query
                        .OrderByDescending(item => item.TotalHoras)
                        .ThenBy(item => item.AreaEspecializacionId),

                ("costototal", false) =>
                    query
                        .OrderBy(item => item.CostoTotal)
                        .ThenBy(item => item.AreaEspecializacionId),

                ("costototal", true) =>
                    query
                        .OrderByDescending(item => item.CostoTotal)
                        .ThenBy(item => item.AreaEspecializacionId),

                ("costopromedio", false) =>
                    query
                        .OrderBy(item => item.CostoPromedio)
                        .ThenBy(item => item.AreaEspecializacionId),

                ("costopromedio", true) =>
                    query
                        .OrderByDescending(item => item.CostoPromedio)
                        .ThenBy(item => item.AreaEspecializacionId),

                _ =>
                    query
                        .OrderBy(item => item.AreaEspecializacion)
                        .ThenBy(item => item.AreaEspecializacionId)
            };
        }

        private static IQueryable<ConsultorTopFacturacionDto>
            AplicarOrdenamientoConsultoresFacturacion(
                IQueryable<ConsultorTopFacturacionDto> query,
                string? sortBy,
                string sortDir)
        {
            string campo = sortBy?
                .Trim()
                .ToLowerInvariant()
                ?? "totalfacturado";

            bool descendente = sortDir.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase);

            return (campo, descendente) switch
            {
                ("nombreconsultor", false) =>
                    query
                        .OrderBy(item => item.NombreConsultor)
                        .ThenBy(item => item.ConsultorId),

                ("nombreconsultor", true) =>
                    query
                        .OrderByDescending(item => item.NombreConsultor)
                        .ThenBy(item => item.ConsultorId),

                ("areaespecializacion", false) =>
                    query
                        .OrderBy(item => item.AreaEspecializacion)
                        .ThenBy(item => item.ConsultorId),

                ("areaespecializacion", true) =>
                    query
                        .OrderByDescending(
                            item => item.AreaEspecializacion)
                        .ThenBy(item => item.ConsultorId),

                ("tarifahora", false) =>
                    query
                        .OrderBy(item => item.TarifaHora)
                        .ThenBy(item => item.ConsultorId),

                ("tarifahora", true) =>
                    query
                        .OrderByDescending(item => item.TarifaHora)
                        .ThenBy(item => item.ConsultorId),

                ("cantidadpaquetes", false) =>
                    query
                        .OrderBy(item => item.CantidadPaquetes)
                        .ThenBy(item => item.ConsultorId),

                ("cantidadpaquetes", true) =>
                    query
                        .OrderByDescending(
                            item => item.CantidadPaquetes)
                        .ThenBy(item => item.ConsultorId),

                ("totalhoras", false) =>
                    query
                        .OrderBy(item => item.TotalHoras)
                        .ThenBy(item => item.ConsultorId),

                ("totalhoras", true) =>
                    query
                        .OrderByDescending(item => item.TotalHoras)
                        .ThenBy(item => item.ConsultorId),

                ("totalfacturado", false) =>
                    query
                        .OrderBy(item => item.TotalFacturado)
                        .ThenBy(item => item.ConsultorId),

                _ =>
                    query
                        .OrderByDescending(item => item.TotalFacturado)
                        .ThenBy(item => item.ConsultorId)
            };
        }
    }
}
