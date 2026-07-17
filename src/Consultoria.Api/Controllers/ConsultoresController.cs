using Consultoria.Api.Models.Responses;
using Consultoria.Application.DTOs.Consultores;
using Consultoria.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consultoria.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/consultores")]
    public sealed class ConsultoresController : ControllerBase
    {
        private readonly IConsultorService _consultorService;

        public ConsultoresController(
            IConsultorService consultorService)
        {
            _consultorService = consultorService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(
            typeof(ApiResponse<ConsultorDto>),
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
            StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<ConsultorDto>>> Crear(
            [FromBody] CrearConsultorDto request,
            CancellationToken cancellationToken)
        {
            ConsultorDto consultor =
                await _consultorService.CrearAsync(
                    request,
                    cancellationToken);

            ApiResponse<ConsultorDto> response =
                ApiResponse<ConsultorDto>.Ok(
                    consultor,
                    "Consultor creado correctamente.");

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = consultor.ConsultorId },
                response);
        }

        [HttpGet]
        [ProducesResponseType(
            typeof(ApiResponse<IReadOnlyCollection<ConsultorDto>>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        public async Task<
            ActionResult<ApiResponse<IReadOnlyCollection<ConsultorDto>>>>ObtenerTodos(
                CancellationToken cancellationToken)
        {
            IReadOnlyCollection<ConsultorDto> consultores =
                await _consultorService.ObtenerTodosAsync(
                    cancellationToken);

            return Ok(
                ApiResponse<IReadOnlyCollection<ConsultorDto>>.Ok(
                    consultores,
                    "Consultores obtenidos correctamente."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(ApiResponse<ConsultorDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ConsultorDto>>> ObtenerPorId(int id,
                CancellationToken cancellationToken)
        {
            ConsultorDto consultor =
                await _consultorService.ObtenerPorIdAsync(
                    id,
                    cancellationToken);

            return Ok(
                ApiResponse<ConsultorDto>.Ok(
                    consultor,
                    "Consultor obtenido correctamente."));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(
            typeof(ApiResponse<ConsultorDto>),
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
            StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<ConsultorDto>>>Actualizar(int id,
                [FromBody] ActualizarConsultorDto request,
                CancellationToken cancellationToken)
        {
            ConsultorDto consultor =
                await _consultorService.ActualizarAsync(
                    id,
                    request,
                    cancellationToken);

            return Ok(
                ApiResponse<ConsultorDto>.Ok(
                    consultor,
                    "Consultor actualizado correctamente."));
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
        public async Task<ActionResult<ApiResponse<object?>>>Desactivar(int id,
                CancellationToken cancellationToken)
        {
            await _consultorService.DesactivarAsync(
                id,
                cancellationToken);

            return Ok(
                ApiResponse<object?>.Ok(
                    null,
                    "Consultor desactivado correctamente."));
        }
    }
}
