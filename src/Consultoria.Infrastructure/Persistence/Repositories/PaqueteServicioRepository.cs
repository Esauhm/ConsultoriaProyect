using Consultoria.Application.DTOs.Paquetes;
using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Domain.Entities;
using Consultoria.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Infrastructure.Persistence.Repositories
{
    public sealed class PaqueteServicioRepository
    : IPaqueteServicioRepository
    {
        private readonly ConsultoriaDbContext _context;

        public PaqueteServicioRepository(
            ConsultoriaDbContext context)
        {
            _context = context;
        }

        public async Task<int> CrearAsync(
            PaqueteServicio paquete,
            CancellationToken cancellationToken = default)
        {
            await _context.PaquetesServicio.AddAsync(
                paquete,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return paquete.PaqueteId;
        }

        public Task<PaqueteServicio?> ObtenerEntidadPorIdAsync(
            int paqueteId,
            CancellationToken cancellationToken = default)
        {
            return _context.PaquetesServicio
                .FirstOrDefaultAsync(
                    paquete => paquete.PaqueteId == paqueteId,
                    cancellationToken);
        }

        public async Task<PaqueteServicioDto?> ObtenerPorIdAsync(
            int paqueteId,
            CancellationToken cancellationToken = default)
        {
            return await (
                from paquete in _context.PaquetesServicio.AsNoTracking()

                join area in _context.AreasEspecializacion.AsNoTracking()
                    on paquete.AreaEspecializacionId
                    equals area.AreaEspecializacionId

                join consultor in _context.Consultores.AsNoTracking()
                    on paquete.ConsultorId
                    equals consultor.ConsultorId

                where paquete.PaqueteId == paqueteId

                select new PaqueteServicioDto
                {
                    PaqueteId = paquete.PaqueteId,
                    Nombre = paquete.Nombre,

                    AreaEspecializacionId =
                        paquete.AreaEspecializacionId,

                    AreaEspecializacion = area.Nombre,

                    ConsultorId = paquete.ConsultorId,
                    Consultor = consultor.Nombre,

                    DuracionHoras = paquete.DuracionHoras,
                    Costo = paquete.Costo,
                    Descripcion = paquete.Descripcion,
                    Activo = paquete.Activo,
                    FechaCreacion = paquete.FechaCreacion
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<PaqueteServicioDto>>
            ObtenerTodosAsync(
                CancellationToken cancellationToken = default)
        {
            return await (
                from paquete in _context.PaquetesServicio.AsNoTracking()

                join area in _context.AreasEspecializacion.AsNoTracking()
                    on paquete.AreaEspecializacionId
                    equals area.AreaEspecializacionId

                join consultor in _context.Consultores.AsNoTracking()
                    on paquete.ConsultorId
                    equals consultor.ConsultorId

                orderby paquete.Nombre

                select new PaqueteServicioDto
                {
                    PaqueteId = paquete.PaqueteId,
                    Nombre = paquete.Nombre,

                    AreaEspecializacionId =
                        paquete.AreaEspecializacionId,

                    AreaEspecializacion = area.Nombre,

                    ConsultorId = paquete.ConsultorId,
                    Consultor = consultor.Nombre,

                    DuracionHoras = paquete.DuracionHoras,
                    Costo = paquete.Costo,
                    Descripcion = paquete.Descripcion,
                    Activo = paquete.Activo,
                    FechaCreacion = paquete.FechaCreacion
                })
                .ToListAsync(cancellationToken);
        }

        public async Task ActualizarAsync(
            PaqueteServicio paquete,
            CancellationToken cancellationToken = default)
        {
            if (_context.Entry(paquete).State == EntityState.Detached)
            {
                _context.PaquetesServicio.Update(paquete);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DesactivarAsync(
            int paqueteId,
            CancellationToken cancellationToken = default)
        {
            PaqueteServicio? paquete =
                await _context.PaquetesServicio
                    .FirstOrDefaultAsync(
                        paquete => paquete.PaqueteId == paqueteId,
                        cancellationToken);

            if (paquete is null)
            {
                return;
            }

            paquete.Desactivar();

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
