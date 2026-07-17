using Consultoria.Application.DTOs.AreasEspecializacion;
using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Domain.Entities;
using Consultoria.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Consultoria.Infrastructure.Persistence.Repositories
{
    public sealed class AreaEspecializacionRepository
    : IAreaEspecializacionRepository
    {
        private readonly ConsultoriaDbContext _context;

        public AreaEspecializacionRepository(
            ConsultoriaDbContext context)
        {
            _context = context;
        }

        public async Task<int> CrearAsync(
            AreaEspecializacion area,
            CancellationToken cancellationToken = default)
        {
            await _context.AreasEspecializacion.AddAsync(
                area,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return area.AreaEspecializacionId;
        }

        public async Task<AreaEspecializacion?>
            ObtenerEntidadPorIdAsync(
                int areaEspecializacionId,
                CancellationToken cancellationToken = default)
        {
            return await _context.AreasEspecializacion
                .SingleOrDefaultAsync(
                    area =>
                        area.AreaEspecializacionId ==
                        areaEspecializacionId,
                    cancellationToken);
        }

        public async Task<AreaEspecializacionDto?> ObtenerPorIdAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken = default)
        {
            return await _context.AreasEspecializacion
                .AsNoTracking()
                .Where(
                    area =>
                        area.AreaEspecializacionId ==
                        areaEspecializacionId)
                .Select(area => new AreaEspecializacionDto
                {
                    AreaEspecializacionId =
                        area.AreaEspecializacionId,

                    Nombre = area.Nombre,

                    Activo = area.Activo
                })
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<AreaEspecializacionDto>>
            ObtenerTodasAsync(
                CancellationToken cancellationToken = default)
        {
            return await _context.AreasEspecializacion
                .AsNoTracking()
                .OrderBy(area => area.Nombre)
                .Select(area => new AreaEspecializacionDto
                {
                    AreaEspecializacionId =
                        area.AreaEspecializacionId,

                    Nombre = area.Nombre,

                    Activo = area.Activo
                })
                .ToListAsync(cancellationToken);
        }

        public async Task ActualizarAsync(
            AreaEspecializacion area,
            CancellationToken cancellationToken = default)
        {
            _context.AreasEspecializacion.Update(area);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DesactivarAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken = default)
        {
            AreaEspecializacion? area =
                await _context.AreasEspecializacion
                    .SingleOrDefaultAsync(
                        area =>
                            area.AreaEspecializacionId ==
                            areaEspecializacionId,
                        cancellationToken);

            if (area is null)
            {
                return;
            }

            area.Desactivar();

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExisteNombreAsync(
            string nombre,
            int? areaEspecializacionIdExcluir = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<AreaEspecializacion> query =
                _context.AreasEspecializacion
                    .AsNoTracking()
                    .Where(area => area.Nombre == nombre);

            if (areaEspecializacionIdExcluir.HasValue)
            {
                query = query.Where(
                    area =>
                        area.AreaEspecializacionId !=
                        areaEspecializacionIdExcluir.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> ExisteActivaAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken = default)
        {
            return await _context.AreasEspecializacion
                .AsNoTracking()
                .AnyAsync(
                    area =>
                        area.AreaEspecializacionId ==
                        areaEspecializacionId &&
                        area.Activo,
                    cancellationToken);
        }
    }
}
