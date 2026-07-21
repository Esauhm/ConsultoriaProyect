using Consultoria.Api.Models.Responses;
using Consultoria.Application.DTOs.AreasEspecializacion;
using Consultoria.Application.Interfaces.Services;
using Consultoria.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consultoria.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/areas-especializacion")]
    public sealed class EspecializacionAreasController : ControllerBase
    {
        private readonly IAreaEspecializacionService _areaService;

        public EspecializacionAreasController(
            IAreaEspecializacionService areaService)
        {
            _areaService = areaService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(
            typeof(ApiResponse<AreaEspecializacionDto>),
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
        public async Task<ActionResult<ApiResponse<AreaEspecializacionDto>>> Crear(
            [FromBody] CrearAreaEspecializacionDto request,
            CancellationToken cancellationToken)
        {
            AreaEspecializacionDto area =
                await _areaService.CrearAsync(
                    request,
                    cancellationToken);

            ApiResponse<AreaEspecializacionDto> response =
                ApiResponse<AreaEspecializacionDto>.Ok(
                    area,
                    "Área de especialización creada correctamente.");

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = area.AreaEspecializacionId },
                response);
        }

        [HttpGet]
        [ProducesResponseType(
            typeof(ApiResponse<IReadOnlyCollection<AreaEspecializacionDto>>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        public async Task<
            ActionResult<ApiResponse<IReadOnlyCollection<AreaEspecializacionDto>>>>
            ObtenerTodas(
                CancellationToken cancellationToken)
        {
            IReadOnlyCollection<AreaEspecializacionDto> areas =
                await _areaService.ObtenerTodasAsync(
                    cancellationToken);

            return Ok(
                ApiResponse<IReadOnlyCollection<AreaEspecializacionDto>>.Ok(
                    areas,
                    "Áreas de especialización obtenidas correctamente."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(ApiResponse<AreaEspecializacionDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AreaEspecializacionDto>>>
            ObtenerPorId(
                int id,
                CancellationToken cancellationToken)
        {
            AreaEspecializacionDto area =
                await _areaService.ObtenerPorIdAsync(
                    id,
                    cancellationToken);

            return Ok(
                ApiResponse<AreaEspecializacionDto>.Ok(
                    area,
                    "Área de especialización obtenida correctamente."));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(
            typeof(ApiResponse<AreaEspecializacionDto>),
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
        public async Task<ActionResult<ApiResponse<AreaEspecializacionDto>>>
            Actualizar(
                int id,
                [FromBody] ActualizarAreaEspecializacionDto request,
                CancellationToken cancellationToken)
        {
            AreaEspecializacionDto area =
                await _areaService.ActualizarAsync(
                    id,
                    request,
                    cancellationToken);

            return Ok(
                ApiResponse<AreaEspecializacionDto>.Ok(
                    area,
                    "Área de especialización actualizada correctamente."));
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
            await _areaService.DesactivarAsync(
                id,
                cancellationToken);

            return Ok(
                ApiResponse<object?>.Ok(
                    null,
                    "Área de especialización desactivada correctamente."));
        }

        [HttpPatch("{id:int}/activar")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(
            typeof(ApiResponse<AreaEspecializacionDto>),
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
        public async Task<ActionResult<ApiResponse<AreaEspecializacionDto>>> Activar(
            int id,
            CancellationToken cancellationToken)
        {
            AreaEspecializacionDto area =
                await _areaService
                    .ActivarAsync(
                        id,
                        cancellationToken);

            return Ok(
                ApiResponse<AreaEspecializacionDto>.Ok(
                    area,
                    "Área de especialización reactivada correctamente."));
        }
    }
}
