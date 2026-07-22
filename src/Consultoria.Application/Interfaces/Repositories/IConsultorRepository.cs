using Consultoria.Application.DTOs.Consultores;
using Consultoria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Interfaces.Repositories
{
    public interface IConsultorRepository
    {
        Task<int> CrearAsync(
            Consultor consultor,
            CancellationToken cancellationToken = default);

        Task<Consultor?> ObtenerEntidadPorIdAsync(
            int consultorId,
            CancellationToken cancellationToken = default);

        Task<ConsultorDto?> ObtenerPorIdAsync(
            int consultorId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<ConsultorDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default);

        Task ActualizarAsync(
            Consultor consultor,
            CancellationToken cancellationToken = default);

        Task ActualizarAsync(
            Consultor consultor,
            byte[] rowVersion,
            CancellationToken cancellationToken = default);

        Task DesactivarAsync(
            int consultorId,
            CancellationToken cancellationToken = default);

        Task<bool> ExisteEmailAsync(
            string emailCorporativo,
            int? consultorIdExcluir = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExisteNombreAreaAsync(
            string nombre,
            int areaEspecializacionId,
            int? consultorIdExcluir = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExisteActivoAsync(
            int consultorId,
            CancellationToken cancellationToken = default);
    }
}
