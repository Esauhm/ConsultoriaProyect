using Consultoria.Application.Common.Exceptions;
using Consultoria.Application.DTOs.Auth;
using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request,
            CancellationToken cancellationToken = default)
        {
            string emailNormalizado = request.Email
                .Trim()
                .ToLowerInvariant();

            var usuario = await _usuarioRepository.ObtenerPorEmailAsync(
                emailNormalizado,
                cancellationToken);

            if (usuario is null || !usuario.Activo)
            {
                _logger.LogWarning(
                    "Intento de inicio de sesión rechazado para el correo {Email}.",
                    emailNormalizado);

                throw new UnauthorizedException(
                    "El correo o la contraseña son incorrectos.");
            }

            bool passwordValido = _passwordHasher.Verificar(
                request.Password,
                usuario.PasswordHash);

            if (!passwordValido)
            {
                _logger.LogWarning(
                    "Intento de inicio de sesión rechazado para el usuario {UsuarioId}.",
                    usuario.UsuarioId);

                throw new UnauthorizedException(
                    "El correo o la contraseña son incorrectos.");
            }

            _logger.LogInformation(
                "El usuario {UsuarioId} inició sesión correctamente con el rol {Rol}.",
                usuario.UsuarioId,
                usuario.Rol);

            return _tokenService.GenerarToken(usuario);
        }
    }
}
