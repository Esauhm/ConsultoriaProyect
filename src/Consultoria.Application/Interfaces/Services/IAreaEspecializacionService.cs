using Consultoria.Application.DTOs.AreasEspecializacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Interfaces.Services
{
    public interface IAreaEspecializacionService
    {
        Task<AreaEspecializacionDto> CrearAsync(
            CrearAreaEspecializacionDto request,
            CancellationToken cancellationToken = default);

        Task<AreaEspecializacionDto> ObtenerPorIdAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<AreaEspecializacionDto>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default);

        Task<AreaEspecializacionDto> ActualizarAsync(
            int areaEspecializacionId,
            ActualizarAreaEspecializacionDto request,
            CancellationToken cancellationToken = default);

        Task DesactivarAsync(
            int areaEspecializacionId,
            CancellationToken cancellationToken = default);
    }
}
