using Consultoria.Application.DTOs.Consultores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Interfaces.Services
{
    public interface IConsultorService
    {
        Task<ConsultorDto> CrearAsync(
            CrearConsultorDto request,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<ConsultorDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default);

        Task<ConsultorDto> ObtenerPorIdAsync(
            int consultorId,
            CancellationToken cancellationToken = default);

        Task<ConsultorDto> ActualizarAsync(
            int consultorId,
            ActualizarConsultorDto request,
            CancellationToken cancellationToken = default);

        Task DesactivarAsync(
            int consultorId,
            CancellationToken cancellationToken = default);

        Task<ConsultorDto> ActivarAsync(
            int consultorId,
            CancellationToken cancellationToken = default);
    }
}
