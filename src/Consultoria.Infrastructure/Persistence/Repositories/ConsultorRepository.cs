using Consultoria.Application.DTOs.Consultores;
using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Domain.Entities;
using Consultoria.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Infrastructure.Persistence.Repositories
{
    public sealed class ConsultorRepository : IConsultorRepository
    {
        private readonly ConsultoriaDbContext _context;

        public ConsultorRepository(ConsultoriaDbContext context)
        {
            _context = context;
        }

        public async Task<int> CrearAsync(
            Consultor consultor,
            CancellationToken cancellationToken = default)
        {
            await _context.Consultores.AddAsync(
                consultor,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return consultor.ConsultorId;
        }

        public Task<Consultor?> ObtenerEntidadPorIdAsync(
            int consultorId,
            CancellationToken cancellationToken = default)
        {
            return _context.Consultores
                .FirstOrDefaultAsync(
                    consultor => consultor.ConsultorId == consultorId,
                    cancellationToken);
        }

        public async Task<ConsultorDto?> ObtenerPorIdAsync(
            int consultorId,
            CancellationToken cancellationToken = default)
        {
            return await (
                from consultor in _context.Consultores.AsNoTracking()
                join area in _context.AreasEspecializacion.AsNoTracking()
                    on consultor.AreaEspecializacionId
                    equals area.AreaEspecializacionId
                where consultor.ConsultorId == consultorId
                select new ConsultorDto
                {
                    ConsultorId = consultor.ConsultorId,
                    Nombre = consultor.Nombre,
                    AreaEspecializacionId =
                        consultor.AreaEspecializacionId,
                    AreaEspecializacion = area.Nombre,
                    TarifaHora = consultor.TarifaHora,
                    EmailCorporativo = consultor.EmailCorporativo,
                    Activo = consultor.Activo,
                    FechaIngreso = consultor.FechaIngreso
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<ConsultorDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            return await (
                from consultor in _context.Consultores.AsNoTracking()
                join area in _context.AreasEspecializacion.AsNoTracking()
                    on consultor.AreaEspecializacionId
                    equals area.AreaEspecializacionId
                orderby consultor.Nombre
                select new ConsultorDto
                {
                    ConsultorId = consultor.ConsultorId,
                    Nombre = consultor.Nombre,
                    AreaEspecializacionId =
                        consultor.AreaEspecializacionId,
                    AreaEspecializacion = area.Nombre,
                    TarifaHora = consultor.TarifaHora,
                    EmailCorporativo = consultor.EmailCorporativo,
                    Activo = consultor.Activo,
                    FechaIngreso = consultor.FechaIngreso
                })
                .ToListAsync(cancellationToken);
        }

        public async Task ActualizarAsync(
            Consultor consultor,
            CancellationToken cancellationToken = default)
        {
            if (_context.Entry(consultor).State == EntityState.Detached)
            {
                _context.Consultores.Update(consultor);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DesactivarAsync(
            int consultorId,
            CancellationToken cancellationToken = default)
        {
            Consultor? consultor = await _context.Consultores
                .FirstOrDefaultAsync(
                    consultor => consultor.ConsultorId == consultorId,
                    cancellationToken);

            if (consultor is null)
            {
                return;
            }

            consultor.Desactivar();

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExisteEmailAsync(
            string emailCorporativo,
            int? consultorIdExcluir = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Consultor> query =
                _context.Consultores.AsNoTracking();

            query = query.Where(
                consultor =>
                    consultor.EmailCorporativo == emailCorporativo);

            if (consultorIdExcluir.HasValue)
            {
                query = query.Where(
                    consultor =>
                        consultor.ConsultorId != consultorIdExcluir.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> ExisteNombreAreaAsync(
            string nombre,
            int areaEspecializacionId,
            int? consultorIdExcluir = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Consultor> query =
                _context.Consultores.AsNoTracking();

            query = query.Where(
                consultor =>
                    consultor.Nombre == nombre
                    && consultor.AreaEspecializacionId
                        == areaEspecializacionId);

            if (consultorIdExcluir.HasValue)
            {
                query = query.Where(
                    consultor =>
                        consultor.ConsultorId != consultorIdExcluir.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public Task<bool> ExisteActivoAsync(
            int consultorId,
            CancellationToken cancellationToken = default)
        {
            return _context.Consultores
                .AsNoTracking()
                .AnyAsync(
                    consultor =>
                        consultor.ConsultorId == consultorId
                        && consultor.Activo,
                    cancellationToken);
        }
    }
}
