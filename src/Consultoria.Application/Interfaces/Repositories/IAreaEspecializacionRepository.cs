using Consultoria.Application.DTOs.AreasEspecializacion;
using Consultoria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Interfaces.Repositories
{
    public interface IAreaEspecializacionRepository
    {
        Task<int> CrearAsync(
            AreaEspecializacion area,
            CancellationToken cancellationToken = default);

        Task<AreaEspecializacion?> ObtenerEntidadPorIdAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken = default);

        Task<AreaEspecializacionDto?> ObtenerPorIdAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<AreaEspecializacionDto>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default);

        Task ActualizarAsync(
            AreaEspecializacion area,
            CancellationToken cancellationToken = default);

        Task ActualizarAsync(
            AreaEspecializacion area,
            byte[] rowVersion,
            CancellationToken cancellationToken = default);

        Task DesactivarAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken = default);

        Task<bool> ExisteNombreAsync(
            string nombre,
            int? areaEspecializacionIdExcluir = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExisteActivaAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken = default);
    }
}
