using Consultoria.Application.Common.Pagination;
using Consultoria.Application.DTOs.Reportes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Interfaces.Repositories
{
    public interface IReporteRepository
    {
        Task<PagedResult<PaquetesPorAreaDto>> ObtenerPaquetesPorAreaAsync(
            PaquetesPorAreaRequestDto request,
            CancellationToken cancellationToken = default);

        Task<PagedResult<ConsultorTopFacturacionDto>> ObtenerConsultoresTopFacturacionAsync(
            ConsultoresTopFacturacionRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
