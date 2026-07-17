using Consultoria.Api.Models.Responses;
using Consultoria.Application.Common.Pagination;
using Consultoria.Application.DTOs.Reportes;
using Consultoria.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consultoria.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/reportes")]
    public sealed class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportesController(
            IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet("paquetes-por-area")]
        [ProducesResponseType(
            typeof(ApiResponse<PagedResult<PaquetesPorAreaDto>>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        public async Task<
            ActionResult<ApiResponse<PagedResult<PaquetesPorAreaDto>>>>
            ObtenerPaquetesPorArea(
                [FromQuery] PaquetesPorAreaRequestDto request,
                CancellationToken cancellationToken)
        {
            PagedResult<PaquetesPorAreaDto> resultado =
                await _reporteService.ObtenerPaquetesPorAreaAsync(
                    request,
                    cancellationToken);

            return Ok(
                ApiResponse<PagedResult<PaquetesPorAreaDto>>.Ok(
                    resultado,
                    "Reporte de paquetes por área obtenido correctamente."));
        }

        [HttpGet("consultores-top-facturacion")]
        [ProducesResponseType(
            typeof(ApiResponse<PagedResult<ConsultorTopFacturacionDto>>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        public async Task<
            ActionResult<ApiResponse<PagedResult<ConsultorTopFacturacionDto>>>>
            ObtenerConsultoresTopFacturacion(
                [FromQuery] ConsultoresTopFacturacionRequestDto request,
                CancellationToken cancellationToken)
        {
            PagedResult<ConsultorTopFacturacionDto> resultado =
                await _reporteService
                    .ObtenerConsultoresTopFacturacionAsync(
                        request,
                        cancellationToken);

            return Ok(
                ApiResponse<
                    PagedResult<ConsultorTopFacturacionDto>>.Ok(
                        resultado,
                        "Reporte de consultores con mayor facturación obtenido correctamente."));
        }
    }
}
