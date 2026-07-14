using Consultoria.Application.DTOs.Auth;
using Consultoria.Application.Interfaces.Services;
using Consultoria.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Consultoria.Infrastructure.Authentication
{
    public sealed class JwtTokenService : ITokenService
    {
        private readonly JwtSettings _settings;

        public JwtTokenService(
            IOptions<JwtSettings> options)
        {
            ArgumentNullException.ThrowIfNull(options);

            _settings = options.Value;

            ValidarConfiguracion();
        }

        public LoginResponseDto GenerarToken(Usuario usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);

            DateTime fechaActualUtc = DateTime.UtcNow;

            DateTime fechaExpiracionUtc = fechaActualUtc.AddMinutes(
                _settings.ExpirationMinutes);

            var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                usuario.UsuarioId.ToString()),

            new(
                ClaimTypes.NameIdentifier,
                usuario.UsuarioId.ToString()),

            new(
                ClaimTypes.Name,
                usuario.Nombre),

            new(
                JwtRegisteredClaimNames.Email,
                usuario.Email),

            new(
                ClaimTypes.Role,
                usuario.Rol),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

            byte[] keyBytes = Encoding.UTF8.GetBytes(
                _settings.Key);

            var securityKey = new SymmetricSecurityKey(
                keyBytes);

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: fechaActualUtc,
                expires: fechaExpiracionUtc,
                signingCredentials: credentials);

            string tokenSerializado =
                new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDto
            {
                Token = tokenSerializado,
                ExpiresAtUtc = fechaExpiracionUtc,
                UserName = usuario.Nombre,
                Email = usuario.Email,
                Role = usuario.Rol
            };
        }

        private void ValidarConfiguracion()
        {
            if (string.IsNullOrWhiteSpace(_settings.Key))
            {
                throw new InvalidOperationException(
                    "La clave de configuración JWT es obligatoria.");
            }

            if (Encoding.UTF8.GetByteCount(_settings.Key) < 32)
            {
                throw new InvalidOperationException(
                    "La clave JWT debe contener al menos 32 bytes.");
            }

            if (string.IsNullOrWhiteSpace(_settings.Issuer))
            {
                throw new InvalidOperationException(
                    "El emisor JWT es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(_settings.Audience))
            {
                throw new InvalidOperationException(
                    "La audiencia JWT es obligatoria.");
            }

            if (_settings.ExpirationMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "El tiempo de expiración del JWT debe ser mayor que cero.");
            }
        }
    }
}
