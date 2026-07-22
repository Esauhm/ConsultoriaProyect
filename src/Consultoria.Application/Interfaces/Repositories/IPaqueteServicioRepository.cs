using Consultoria.Application.DTOs.Paquetes;
using Consultoria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Interfaces.Repositories
{
    public interface IPaqueteServicioRepository
    {
        Task<int> CrearAsync(
            PaqueteServicio paquete,
            CancellationToken cancellationToken = default);

        Task<PaqueteServicio?> ObtenerEntidadPorIdAsync(
            int paqueteId,
            CancellationToken cancellationToken = default);

        Task<PaqueteServicioDto?> ObtenerPorIdAsync(
            int paqueteId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<PaqueteServicioDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default);

        Task ActualizarAsync(
            PaqueteServicio paquete,
            CancellationToken cancellationToken = default);

        Task ActualizarAsync(
            PaqueteServicio paquete,
            byte[] rowVersion,
            CancellationToken cancellationToken = default);

        Task DesactivarAsync(
            int paqueteId,
            CancellationToken cancellationToken = default);
    }
}
