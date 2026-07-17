using Consultoria.Api.Models.Responses;
using Consultoria.Application.DTOs.Paquetes;
using Consultoria.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consultoria.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/paquetes")]
    public sealed class PaquetesController : ControllerBase
    {
        private readonly IPaqueteServicioService _paqueteServicioService;

        public PaquetesController(
            IPaqueteServicioService paqueteServicioService)
        {
            _paqueteServicioService = paqueteServicioService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(
            typeof(ApiResponse<PaqueteServicioDto>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ApiResponse<PaqueteServicioDto>>> Crear(
            [FromBody] CrearPaqueteServicioDto request,
            CancellationToken cancellationToken)
        {
            PaqueteServicioDto paquete =
                await _paqueteServicioService.CrearAsync(
                    request,
                    cancellationToken);

            ApiResponse<PaqueteServicioDto> response =
                ApiResponse<PaqueteServicioDto>.Ok(
                    paquete,
                    "Paquete de servicio creado correctamente.");

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = paquete.PaqueteId },
                response);
        }

        [HttpGet]
        [ProducesResponseType(
            typeof(ApiResponse<IReadOnlyCollection<PaqueteServicioDto>>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        public async Task<
            ActionResult<ApiResponse<IReadOnlyCollection<PaqueteServicioDto>>>>
            ObtenerTodos(
                CancellationToken cancellationToken)
        {
            IReadOnlyCollection<PaqueteServicioDto> paquetes =
                await _paqueteServicioService.ObtenerTodosAsync(
                    cancellationToken);

            return Ok(
                ApiResponse<IReadOnlyCollection<PaqueteServicioDto>>.Ok(
                    paquetes,
                    "Paquetes de servicio obtenidos correctamente."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(ApiResponse<PaqueteServicioDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaqueteServicioDto>>>
            ObtenerPorId(
                int id,
                CancellationToken cancellationToken)
        {
            PaqueteServicioDto paquete =
                await _paqueteServicioService.ObtenerPorIdAsync(
                    id,
                    cancellationToken);

            return Ok(
                ApiResponse<PaqueteServicioDto>.Ok(
                    paquete,
                    "Paquete de servicio obtenido correctamente."));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(
            typeof(ApiResponse<PaqueteServicioDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status404NotFound)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ApiResponse<PaqueteServicioDto>>>
            Actualizar(
                int id,
                [FromBody] ActualizarPaqueteServicioDto request,
                CancellationToken cancellationToken)
        {
            PaqueteServicioDto paquete =
                await _paqueteServicioService.ActualizarAsync(
                    id,
                    request,
                    cancellationToken);

            return Ok(
                ApiResponse<PaqueteServicioDto>.Ok(
                    paquete,
                    "Paquete de servicio actualizado correctamente."));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(
            typeof(ApiResponse<object?>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status404NotFound)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ApiResponse<object?>>> Desactivar(
            int id,
            CancellationToken cancellationToken)
        {
            await _paqueteServicioService.DesactivarAsync(
                id,
                cancellationToken);

            return Ok(
                ApiResponse<object?>.Ok(
                    null,
                    "Paquete de servicio desactivado correctamente."));
        }
    }
}
