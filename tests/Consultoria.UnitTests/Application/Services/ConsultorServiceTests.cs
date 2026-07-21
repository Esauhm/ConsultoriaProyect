using Consultoria.Application.Common.Exceptions;
using Consultoria.Application.DTOs.Consultores;
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
    public sealed class ConsultorServiceTests
    {
        private readonly Mock<IConsultorRepository> _consultorRepositoryMock;
        private readonly Mock<IAreaEspecializacionRepository> _areaRepositoryMock;
        private readonly Mock<ILogger<ConsultorService>> _loggerMock;
        private readonly ConsultorService _service;

        public ConsultorServiceTests()
        {
            _consultorRepositoryMock =
                new Mock<IConsultorRepository>();

            _areaRepositoryMock =
                new Mock<IAreaEspecializacionRepository>();

            _loggerMock =
                new Mock<ILogger<ConsultorService>>();

            _service = new ConsultorService(
                _consultorRepositoryMock.Object,
                _areaRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CrearAsync_DebeCrearConsultor_CuandoDatosSonValidos()
        {
            // Arrange
            var request = new CrearConsultorDto
            {
                Nombre = "  Juan Gómez  ",
                AreaEspecializacionId = 2,
                TarifaHora = 45m,
                EmailCorporativo = "  JUAN.GOMEZ@CONSULTORIA.COM  "
            };

            var consultorEsperado = new ConsultorDto
            {
                ConsultorId = 10,
                Nombre = "Juan Gómez",
                AreaEspecializacionId = 2,
                AreaEspecializacion = "Finanzas",
                TarifaHora = 45m,
                EmailCorporativo = "juan.gomez@consultoria.com",
                Activo = true
            };

            _areaRepositoryMock
                .Setup(repository =>
                    repository.ExisteActivaAsync(
                        2,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ExisteEmailAsync(
                        "juan.gomez@consultoria.com",
                        null,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ExisteNombreAreaAsync(
                        "Juan Gómez",
                        2,
                        null,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.CrearAsync(
                        It.IsAny<Consultor>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(10);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ObtenerPorIdAsync(
                        10,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultorEsperado);

            // Act
            ConsultorDto resultado =
                await _service.CrearAsync(request);

            // Assert
            Assert.Equal(10, resultado.ConsultorId);
            Assert.Equal("Juan Gómez", resultado.Nombre);
            Assert.Equal("juan.gomez@consultoria.com", resultado.EmailCorporativo);
            Assert.Equal(45m, resultado.TarifaHora);
            Assert.True(resultado.Activo);

            _consultorRepositoryMock.Verify(
                repository =>
                    repository.CrearAsync(
                        It.Is<Consultor>(consultor =>
                            consultor.Nombre == "Juan Gómez" &&
                            consultor.EmailCorporativo ==
                                "juan.gomez@consultoria.com" &&
                            consultor.AreaEspecializacionId == 2 &&
                            consultor.TarifaHora == 45m &&
                            consultor.Activo),
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CrearAsync_DebeLanzarBusinessException_CuandoAreaNoEstaActiva()
        {
            // Arrange
            var request = CrearRequestValido();

            _areaRepositoryMock
                .Setup(repository =>
                    repository.ExisteActivaAsync(
                        request.AreaEspecializacionId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            Func<Task> accion = async () =>
                await _service.CrearAsync(request);

            // Assert
            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(
                    accion);

            Assert.Contains(
                "no existe o se encuentra inactiva",
                exception.Message);

            _consultorRepositoryMock.Verify(
                repository =>
                    repository.CrearAsync(
                        It.IsAny<Consultor>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CrearAsync_DebeLanzarConflictException_CuandoEmailYaExiste()
        {
            // Arrange
            var request = CrearRequestValido();

            _areaRepositoryMock
                .Setup(repository =>
                    repository.ExisteActivaAsync(
                        request.AreaEspecializacionId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ExisteEmailAsync(
                        request.EmailCorporativo,
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
                "correo corporativo",
                exception.Message);

            _consultorRepositoryMock.Verify(
                repository =>
                    repository.CrearAsync(
                        It.IsAny<Consultor>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CrearAsync_DebeLanzarConflictException_CuandoNombreYAreaYaExisten()
        {
            // Arrange
            var request = CrearRequestValido();

            _areaRepositoryMock
                .Setup(repository =>
                    repository.ExisteActivaAsync(
                        request.AreaEspecializacionId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ExisteEmailAsync(
                        request.EmailCorporativo,
                        null,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ExisteNombreAreaAsync(
                        request.Nombre,
                        request.AreaEspecializacionId,
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
                "mismo nombre y área",
                exception.Message);

            _consultorRepositoryMock.Verify(
                repository =>
                    repository.CrearAsync(
                        It.IsAny<Consultor>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_DebeLanzarNotFoundException_CuandoNoExiste()
        {
            // Arrange
            const int consultorId = 99;

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ObtenerPorIdAsync(
                        consultorId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((ConsultorDto?)null);

            // Act
            Func<Task> accion = async () =>
                await _service.ObtenerPorIdAsync(consultorId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(
                accion);
        }

        [Fact]
        public async Task DesactivarAsync_DebeLanzarBusinessException_CuandoYaEstaInactivo()
        {
            // Arrange
            const int consultorId = 5;

            Consultor consultor =
                Consultor.Reconstruir(
                    consultorId: consultorId,
                    nombre: "Juan Gómez",
                    areaEspecializacionId: 2,
                    tarifaHora: 45m,
                    emailCorporativo: "juan.gomez@consultoria.com",
                    activo: false,
                    fechaIngreso: DateTime.UtcNow);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        consultorId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultor);

            // Act
            Func<Task> accion = async () =>
                await _service.DesactivarAsync(consultorId);

            // Assert
            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(
                    accion);

            Assert.Contains(
                "ya se encuentra inactivo",
                exception.Message);

            _consultorRepositoryMock.Verify(
                repository =>
                    repository.DesactivarAsync(
                        It.IsAny<int>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ActivarAsync_DebeActivarConsultor_CuandoAreaEstaActiva()
        {
            // Arrange
            const int consultorId = 5;
            const int areaId = 2;

            Consultor consultor =
                Consultor.Reconstruir(
                    consultorId: consultorId,
                    nombre: "Juan Gómez",
                    areaEspecializacionId: areaId,
                    tarifaHora: 45m,
                    emailCorporativo: "juan.gomez@consultoria.com",
                    activo: false,
                    fechaIngreso: DateTime.UtcNow);

            var consultorEsperado =
                new ConsultorDto
                {
                    ConsultorId = consultorId,
                    Nombre = "Juan Gómez",
                    AreaEspecializacionId = areaId,
                    AreaEspecializacion = "Finanzas",
                    TarifaHora = 45m,
                    EmailCorporativo =
                        "juan.gomez@consultoria.com",
                    Activo = true
                };

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        consultorId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultor);

            _areaRepositoryMock
                .Setup(repository =>
                    repository.ExisteActivaAsync(
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ObtenerPorIdAsync(
                        consultorId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultorEsperado);

            // Act
            ConsultorDto resultado =
                await _service.ActivarAsync(consultorId);

            // Assert
            Assert.True(consultor.Activo);
            Assert.True(resultado.Activo);

            _consultorRepositoryMock.Verify(
                repository =>
                    repository.ActualizarAsync(
                        consultor,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ActivarAsync_DebeRechazarActivacion_CuandoAreaEstaInactiva()
        {
            // Arrange
            const int consultorId = 5;
            const int areaId = 2;

            Consultor consultor =
                Consultor.Reconstruir(
                    consultorId: consultorId,
                    nombre: "Juan Gómez",
                    areaEspecializacionId: areaId,
                    tarifaHora: 45m,
                    emailCorporativo: "juan.gomez@consultoria.com",
                    activo: false,
                    fechaIngreso: DateTime.UtcNow);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        consultorId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultor);

            _areaRepositoryMock
                .Setup(repository =>
                    repository.ExisteActivaAsync(
                        areaId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            Func<Task> accion = async () =>
                await _service.ActivarAsync(consultorId);

            // Assert
            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(
                    accion);

            Assert.Contains(
                "área de especialización",
                exception.Message);

            Assert.False(consultor.Activo);

            _consultorRepositoryMock.Verify(
                repository =>
                    repository.ActualizarAsync(
                        It.IsAny<Consultor>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        private static CrearConsultorDto CrearRequestValido()
        {
            return new CrearConsultorDto
            {
                Nombre = "Juan Gómez",
                AreaEspecializacionId = 2,
                TarifaHora = 45m,
                EmailCorporativo = "juan.gomez@consultoria.com"
            };
        }
    }
}
