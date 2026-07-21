using Consultoria.Application.Common.Exceptions;
using Consultoria.Application.DTOs.Auth;
using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Application.Interfaces.Services;
using Consultoria.Application.Services;
using Consultoria.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.UnitTests.Application.Services
{
    public sealed class AuthServiceTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _usuarioRepositoryMock =
                new Mock<IUsuarioRepository>();

            _passwordHasherMock =
                new Mock<IPasswordHasher>();

            _tokenServiceMock =
                new Mock<ITokenService>();

            _loggerMock =
                new Mock<ILogger<AuthService>>();

            _service = new AuthService(
                _usuarioRepositoryMock.Object,
                _passwordHasherMock.Object,
                _tokenServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task LoginAsync_DebeRetornarToken_CuandoCredencialesSonValidas()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "  ADMIN@DEMO.COM  ",
                Password = "Admin.123"
            };

            Usuario usuario = CrearUsuarioActivo();

            DateTime fechaExpiracion =
                DateTime.UtcNow.AddHours(1);

            var respuestaEsperada = new LoginResponseDto
            {
                Token = "jwt-token-de-prueba",
                ExpiresAtUtc = fechaExpiracion,
                UserName = "Administrador",
                Email = "admin@demo.com",
                Role = "Admin"
            };

            _usuarioRepositoryMock
                .Setup(repository =>
                    repository.ObtenerPorEmailAsync(
                        "admin@demo.com",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.Verificar(
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(true);

            _tokenServiceMock
                .Setup(service =>
                    service.GenerarToken(usuario))
                .Returns(respuestaEsperada);

            // Act
            LoginResponseDto resultado =
                await _service.LoginAsync(request);

            // Assert
            Assert.Equal(
                "jwt-token-de-prueba",
                resultado.Token);

            Assert.Equal(
                "Administrador",
                resultado.UserName);

            Assert.Equal(
                "admin@demo.com",
                resultado.Email);

            Assert.Equal(
                "Admin",
                resultado.Role);

            Assert.Equal(
                fechaExpiracion,
                resultado.ExpiresAtUtc);

            _usuarioRepositoryMock.Verify(
                repository =>
                    repository.ObtenerPorEmailAsync(
                        "admin@demo.com",
                        It.IsAny<CancellationToken>()),
                Times.Once);

            _passwordHasherMock.Verify(
                hasher =>
                    hasher.Verificar(
                        It.IsAny<string>(),
                        It.IsAny<string>()),
                Times.Once);

            _tokenServiceMock.Verify(
                service =>
                    service.GenerarToken(usuario),
                Times.Once);
        }

        [Fact]
        public async Task LoginAsync_DebeLanzarUnauthorizedException_CuandoUsuarioNoExiste()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "inexistente@demo.com",
                Password = "Password.123"
            };

            _usuarioRepositoryMock
                .Setup(repository =>
                    repository.ObtenerPorEmailAsync(
                        "inexistente@demo.com",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            Func<Task> accion = async () =>
                await _service.LoginAsync(request);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                accion);

            _passwordHasherMock.Verify(
                hasher =>
                    hasher.Verificar(
                        It.IsAny<string>(),
                        It.IsAny<string>()),
                Times.Never);

            _tokenServiceMock.Verify(
                service =>
                    service.GenerarToken(
                        It.IsAny<Usuario>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_DebeLanzarUnauthorizedException_CuandoUsuarioEstaInactivo()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "admin@demo.com",
                Password = "Admin.123"
            };

            Usuario usuario = CrearUsuarioActivo();

            usuario.Desactivar();

            _usuarioRepositoryMock
                .Setup(repository =>
                    repository.ObtenerPorEmailAsync(
                        "admin@demo.com",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            Func<Task> accion = async () =>
                await _service.LoginAsync(request);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                accion);

            _passwordHasherMock.Verify(
                hasher =>
                    hasher.Verificar(
                        It.IsAny<string>(),
                        It.IsAny<string>()),
                Times.Never);

            _tokenServiceMock.Verify(
                service =>
                    service.GenerarToken(
                        It.IsAny<Usuario>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_DebeLanzarUnauthorizedException_CuandoPasswordEsIncorrecta()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "admin@demo.com",
                Password = "Password.Incorrecta"
            };

            Usuario usuario = CrearUsuarioActivo();

            _usuarioRepositoryMock
                .Setup(repository =>
                    repository.ObtenerPorEmailAsync(
                        "admin@demo.com",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.Verificar(
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(false);

            // Act
            Func<Task> accion = async () =>
                await _service.LoginAsync(request);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                accion);

            _passwordHasherMock.Verify(
                hasher =>
                    hasher.Verificar(
                        It.IsAny<string>(),
                        It.IsAny<string>()),
                Times.Once);

            _tokenServiceMock.Verify(
                service =>
                    service.GenerarToken(
                        It.IsAny<Usuario>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_DebeNormalizarEmail_AntesDeConsultarRepositorio()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "   USUARIO@DEMO.COM   ",
                Password = "User.123"
            };

            var usuario = new Usuario(
                nombre: "Usuario Demo",
                email: "usuario@demo.com",
                passwordHash: "HASH_USUARIO",
                rol: "User");

            var respuestaEsperada = new LoginResponseDto
            {
                Token = "token-user",
                ExpiresAtUtc = DateTime.UtcNow.AddHours(1),
                UserName = "Usuario Demo",
                Email = "usuario@demo.com",
                Role = "User"
            };

            _usuarioRepositoryMock
                .Setup(repository =>
                    repository.ObtenerPorEmailAsync(
                        "usuario@demo.com",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.Verificar(
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(true);

            _tokenServiceMock
                .Setup(service =>
                    service.GenerarToken(usuario))
                .Returns(respuestaEsperada);

            // Act
            LoginResponseDto resultado =
                await _service.LoginAsync(request);

            // Assert
            Assert.Equal(
                "usuario@demo.com",
                resultado.Email);

            Assert.Equal(
                "User",
                resultado.Role);

            _usuarioRepositoryMock.Verify(
                repository =>
                    repository.ObtenerPorEmailAsync(
                        "usuario@demo.com",
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static Usuario CrearUsuarioActivo()
        {
            return new Usuario(
                nombre: "Administrador",
                email: "admin@demo.com",
                passwordHash: "HASH_ADMIN",
                rol: "Admin");
        }
    }
}
