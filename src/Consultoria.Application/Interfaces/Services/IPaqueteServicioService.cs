using Consultoria.Application.DTOs.Paquetes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Interfaces.Services
{
    public interface IPaqueteServicioService
    {
        Task<PaqueteServicioDto> CrearAsync(
            CrearPaqueteServicioDto request,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<PaqueteServicioDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default);

        Task<PaqueteServicioDto> ObtenerPorIdAsync(
            int paqueteId,
            CancellationToken cancellationToken = default);

        Task<PaqueteServicioDto> ActualizarAsync(
            int paqueteId,
            ActualizarPaqueteServicioDto request,
            CancellationToken cancellationToken = default);

        Task DesactivarAsync(
            int paqueteId,
            CancellationToken cancellationToken = default);
    }
}
