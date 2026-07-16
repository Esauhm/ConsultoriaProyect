using Consultoria.Api.Models.Responses;
using Consultoria.Application.DTOs.Auth;
using Consultoria.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consultoria.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(
            typeof(ApiResponse<LoginResponseDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(
            [FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            LoginResponseDto resultado =
                await _authService.LoginAsync(
                    request,
                    cancellationToken);

            return Ok(
                ApiResponse<LoginResponseDto>.Ok(
                    resultado,
                    "Inicio de sesión exitoso."));
        }
    }
}
