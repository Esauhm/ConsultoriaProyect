using Consultoria.Application.Common.Exceptions;
using Consultoria.Application.DTOs.AreasEspecializacion;
using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Application.Services;
using Consultoria.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.UnitTests.Application.Services
{
    public sealed class AreaEspecializacionServiceTests
    {
        private readonly Mock<IAreaEspecializacionRepository> _repositoryMock;
        private readonly Mock<ILogger<AreaEspecializacionService>> _loggerMock;
        private readonly AreaEspecializacionService _service;

        public AreaEspecializacionServiceTests()
        {
            _repositoryMock =
                new Mock<IAreaEspecializacionRepository>();

            _loggerMock =
                new Mock<ILogger<AreaEspecializacionService>>();

            _service = new AreaEspecializacionService(
                _repositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CrearAsync_DebeCrearArea_CuandoNombreEsValido()
        {
            // Arrange
            var request = new CrearAreaEspecializacionDto
            {
                Nombre = "  Desarrollo   de Software  "
            };

            var areaEsperada = new AreaEspecializacionDto
            {
                AreaEspecializacionId = 6,
                Nombre = "Desarrollo de Software",
                Activo = true
            };

            AreaEspecializacion? areaGuardada = null;

            _repositoryMock
                .Setup(repository =>
                    repository.ExisteNombreAsync(
                        "Desarrollo de Software",
                        null,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _repositoryMock
                .Setup(repository =>
                    repository.CrearAsync(
                        It.IsAny<AreaEspecializacion>(),
                        It.IsAny<CancellationToken>()))
                .Callback<AreaEspecializacion, CancellationToken>(
                    (area, _) => areaGuardada = area)
                .ReturnsAsync(6);

            _repositoryMock
                .Setup(repository =>
                    repository.ObtenerPorIdAsync(
                        6,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaEsperada);

            // Act
            AreaEspecializacionDto resultado =
                await _service.CrearAsync(request);

            // Assert
            Assert.NotNull(areaGuardada);
            Assert.Equal(
                "Desarrollo de Software",
                areaGuardada.Nombre);

            Assert.True(areaGuardada.Activo);

            Assert.Equal(
                6,
                resultado.AreaEspecializacionId);

            Assert.Equal(
                "Desarrollo de Software",
                resultado.Nombre);

            _repositoryMock.Verify(
                repository =>
                    repository.CrearAsync(
                        It.IsAny<AreaEspecializacion>(),
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CrearAsync_DebeLanzarConflictException_CuandoNombreYaExiste()
        {
            // Arrange
            var request = new CrearAreaEspecializacionDto
            {
                Nombre = "Finanzas"
            };

            _repositoryMock
                .Setup(repository =>
                    repository.ExisteNombreAsync(
                        "Finanzas",
                        null,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            Func<Task> accion = async () =>
                await _service.CrearAsync(request);

            // Assert
            ConflictException exception =
                await Assert.ThrowsAsync<ConflictException>(
                    accion);

            Assert.Contains(
                "Ya existe",
                exception.Message);

            _repositoryMock.Verify(
                repository =>
                    repository.CrearAsync(
                        It.IsAny<AreaEspecializacion>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_DebeRetornarArea_CuandoExiste()
        {
            // Arrange
            const int areaId = 2;

            var areaEsperada = new AreaEspecializacionDto
            {
                AreaEspecializacionId = areaId,
                Nombre = "Finanzas",
                Activo = true
            };

            _repositoryMock
                .Setup(repository =>
                    repository.ObtenerPorIdAsync(
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaEsperada);

            // Act
            AreaEspecializacionDto resultado =
                await _service.ObtenerPorIdAsync(areaId);

            // Assert
            Assert.Equal(areaId, resultado.AreaEspecializacionId);
            Assert.Equal("Finanzas", resultado.Nombre);
            Assert.True(resultado.Activo);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_DebeLanzarNotFoundException_CuandoNoExiste()
        {
            // Arrange
            const int areaId = 99;

            _repositoryMock
                .Setup(repository =>
                    repository.ObtenerPorIdAsync(
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((AreaEspecializacionDto?)null);

            // Act
            Func<Task> accion = async () =>
                await _service.ObtenerPorIdAsync(areaId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(
                accion);
        }

        [Fact]
        public async Task ActualizarAsync_DebeActualizarNombre_CuandoEsValido()
        {
            // Arrange
            const int areaId = 3;

            var request = new ActualizarAreaEspecializacionDto
            {
                Nombre = "  Seguridad   Informática "
            };

            var area = new AreaEspecializacion(
                "Ciberseguridad");

            var areaEsperada = new AreaEspecializacionDto
            {
                AreaEspecializacionId = areaId,
                Nombre = "Seguridad Informática",
                Activo = true
            };

            _repositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(area);

            _repositoryMock
                .Setup(repository =>
                    repository.ExisteNombreAsync(
                        "Seguridad Informática",
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _repositoryMock
                .Setup(repository =>
                    repository.ObtenerPorIdAsync(
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaEsperada);

            // Act
            AreaEspecializacionDto resultado =
                await _service.ActualizarAsync(
                    areaId,
                    request);

            // Assert
            Assert.Equal(
                "Seguridad Informática",
                area.Nombre);

            Assert.Equal(
                "Seguridad Informática",
                resultado.Nombre);

            _repositoryMock.Verify(
                repository =>
                    repository.ActualizarAsync(
                        area,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ActualizarAsync_DebeLanzarConflictException_CuandoNombrePerteneceAOtraArea()
        {
            // Arrange
            const int areaId = 3;

            var request = new ActualizarAreaEspecializacionDto
            {
                Nombre = "Finanzas"
            };

            var area = new AreaEspecializacion(
                "Ciberseguridad");

            _repositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(area);

            _repositoryMock
                .Setup(repository =>
                    repository.ExisteNombreAsync(
                        "Finanzas",
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            Func<Task> accion = async () =>
                await _service.ActualizarAsync(
                    areaId,
                    request);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(
                accion);

            _repositoryMock.Verify(
                repository =>
                    repository.ActualizarAsync(
                        It.IsAny<AreaEspecializacion>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task DesactivarAsync_DebeDesactivarArea_CuandoEstaActiva()
        {
            // Arrange
            const int areaId = 4;

            var area = new AreaEspecializacion(
                "Marketing");

            _repositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(area);

            // Act
            await _service.DesactivarAsync(areaId);

            // Assert
            _repositoryMock.Verify(
                repository =>
                    repository.DesactivarAsync(
                        areaId,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DesactivarAsync_DebeLanzarBusinessException_CuandoYaEstaInactiva()
        {
            // Arrange
            const int areaId = 4;

            var area = new AreaEspecializacion(
                "Marketing");

            area.Desactivar();

            _repositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(area);

            // Act
            Func<Task> accion = async () =>
                await _service.DesactivarAsync(areaId);

            // Assert
            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(
                    accion);

            Assert.Contains(
                "ya se encuentra desactivada",
                exception.Message);

            _repositoryMock.Verify(
                repository =>
                    repository.DesactivarAsync(
                        It.IsAny<int>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }


        [Fact]
        public async Task ActivarAsync_DebeActivarArea_CuandoEstaInactiva()
        {
            // Arrange
            const int areaId = 5;

            var area =
                new AreaEspecializacion("Arquitectura");

            area.Desactivar();

            var areaEsperada =
                new AreaEspecializacionDto
                {
                    AreaEspecializacionId = areaId,
                    Nombre = "Arquitectura",
                    Activo = true
                };

            _repositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(area);

            _repositoryMock
                .Setup(repository =>
                    repository.ObtenerPorIdAsync(
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaEsperada);

            // Act
            AreaEspecializacionDto resultado =
                await _service.ActivarAsync(areaId);

            // Assert
            Assert.True(area.Activo);
            Assert.True(resultado.Activo);

            _repositoryMock.Verify(
                repository =>
                    repository.ActualizarAsync(
                        area,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
