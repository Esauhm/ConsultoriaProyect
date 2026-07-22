using Consultoria.Application.Common.Exceptions;
using Consultoria.Application.DTOs.Paquetes;
using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Application.Services;
using Consultoria.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Consultoria.UnitTests.Application.Services
{
    public sealed class PaqueteServicioServiceTests
    {
        private readonly Mock<IPaqueteServicioRepository>
            _paqueteRepositoryMock;

        private readonly Mock<IConsultorRepository>
            _consultorRepositoryMock;

        private readonly Mock<IAreaEspecializacionRepository>
            _areaRepositoryMock;

        private readonly Mock<ILogger<PaqueteServicioService>>
            _loggerMock;

        private readonly PaqueteServicioService _service;

        public PaqueteServicioServiceTests()
        {
            _paqueteRepositoryMock =
                new Mock<IPaqueteServicioRepository>();

            _consultorRepositoryMock =
                new Mock<IConsultorRepository>();

            _areaRepositoryMock =
                new Mock<IAreaEspecializacionRepository>();

            _loggerMock =
                new Mock<ILogger<PaqueteServicioService>>();

            _service = new PaqueteServicioService(
                _paqueteRepositoryMock.Object,
                _consultorRepositoryMock.Object,
                _areaRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CrearAsync_DebeUsarAreaYTarifaDelConsultor()
        {
            // Arrange
            var request = new CrearPaqueteServicioDto
            {
                Nombre = "Administración financiera",
                ConsultorId = 2,
                DuracionHoras = 10,
                Descripcion =
                    "Taller de buenas prácticas financieras."
            };

            Consultor consultor = CrearConsultor(
                consultorId: 2,
                areaEspecializacionId: 3,
                tarifaHora: 45m);

            PaqueteServicio? paqueteGuardado = null;

            var paqueteEsperado = new PaqueteServicioDto
            {
                PaqueteId = 20,
                Nombre = "Administración financiera",
                AreaEspecializacionId = 3,
                ConsultorId = 2,
                DuracionHoras = 10,
                TarifaHoraAplicada = 45m,
                Costo = 450m,
                Descripcion =
                    "Taller de buenas prácticas financieras.",
                Activo = true
            };

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        2,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultor);

            _areaRepositoryMock
                .Setup(repository =>
                    repository.ExisteActivaAsync(
                        3,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _paqueteRepositoryMock
                .Setup(repository =>
                    repository.CrearAsync(
                        It.IsAny<PaqueteServicio>(),
                        It.IsAny<CancellationToken>()))
                .Callback<PaqueteServicio, CancellationToken>(
                    (paquete, _) =>
                    {
                        paqueteGuardado = paquete;
                    })
                .ReturnsAsync(20);

            _paqueteRepositoryMock
                .Setup(repository =>
                    repository.ObtenerPorIdAsync(
                        20,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(paqueteEsperado);

            // Act
            PaqueteServicioDto resultado =
                await _service.CrearAsync(request);

            // Assert
            Assert.NotNull(paqueteGuardado);

            Assert.Equal(
                3,
                paqueteGuardado.AreaEspecializacionId);

            Assert.Equal(
                2,
                paqueteGuardado.ConsultorId);

            Assert.Equal(
                45m,
                paqueteGuardado.TarifaHoraAplicada);

            Assert.Equal(
                10,
                paqueteGuardado.DuracionHoras);

            Assert.Equal(
                450m,
                paqueteGuardado.Costo);

            Assert.Equal(
                20,
                resultado.PaqueteId);

            Assert.Equal(
                450m,
                resultado.Costo);

            _paqueteRepositoryMock.Verify(
                repository =>
                    repository.CrearAsync(
                        It.IsAny<PaqueteServicio>(),
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CrearAsync_DebeLanzarBusinessException_CuandoConsultorNoExiste()
        {
            // Arrange
            CrearPaqueteServicioDto request =
                CrearRequestValido();

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        request.ConsultorId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((Consultor?)null);

            // Act
            Func<Task> accion = async () =>
                await _service.CrearAsync(request);

            // Assert
            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(
                    accion);

            Assert.Contains(
                "no existe",
                exception.Message);

            _paqueteRepositoryMock.Verify(
                repository =>
                    repository.CrearAsync(
                        It.IsAny<PaqueteServicio>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CrearAsync_DebeLanzarBusinessException_CuandoConsultorEstaInactivo()
        {
            // Arrange
            CrearPaqueteServicioDto request =
                CrearRequestValido();

            Consultor consultor = CrearConsultor(
                consultorId: request.ConsultorId,
                areaEspecializacionId: 2,
                tarifaHora: 45m,
                activo: false);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        request.ConsultorId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultor);

            // Act
            Func<Task> accion = async () =>
                await _service.CrearAsync(request);

            // Assert
            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(
                    accion);

            Assert.Contains(
                "se encuentra inactivo",
                exception.Message);

            _paqueteRepositoryMock.Verify(
                repository =>
                    repository.CrearAsync(
                        It.IsAny<PaqueteServicio>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CrearAsync_DebeLanzarBusinessException_CuandoAreaDelConsultorEstaInactiva()
        {
            // Arrange
            CrearPaqueteServicioDto request =
                CrearRequestValido();

            Consultor consultor = CrearConsultor(
                consultorId: request.ConsultorId,
                areaEspecializacionId: 2,
                tarifaHora: 45m);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        request.ConsultorId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultor);

            _areaRepositoryMock
                .Setup(repository =>
                    repository.ExisteActivaAsync(
                        2,
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
                "área de especialización",
                exception.Message);

            _paqueteRepositoryMock.Verify(
                repository =>
                    repository.CrearAsync(
                        It.IsAny<PaqueteServicio>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ActualizarAsync_DebeRecalcularCosto_ConTarifaActualDelConsultor()
        {
            // Arrange
            const int paqueteId = 15;

            byte[] rowVersion =
                [1, 2, 3, 4, 5, 6, 7, 8];

            byte[] rowVersionActualizado =
                [9, 10, 11, 12, 13, 14, 15, 16];

            var request = new ActualizarPaqueteServicioDto
            {
                Nombre = "Arquitectura empresarial",
                ConsultorId = 5,
                DuracionHoras = 8,
                Descripcion =
                    "Diseño de una arquitectura empresarial.",
                RowVersion = rowVersion
            };

            var paqueteExistente = new PaqueteServicio(
                nombre: "Paquete anterior",
                areaEspecializacionId: 1,
                consultorId: 2,
                duracionHoras: 5,
                tarifaHoraAplicada: 40m,
                descripcion: "Descripción anterior.");

            Consultor consultor = CrearConsultor(
                consultorId: 5,
                areaEspecializacionId: 4,
                tarifaHora: 60m);

            PaqueteServicio? paqueteActualizado = null;

            var resultadoEsperado = new PaqueteServicioDto
            {
                PaqueteId = paqueteId,
                Nombre = "Arquitectura empresarial",
                AreaEspecializacionId = 4,
                ConsultorId = 5,
                DuracionHoras = 8,
                TarifaHoraAplicada = 60m,
                Costo = 480m,
                Descripcion =
                    "Diseño de una arquitectura empresarial.",
                Activo = true,
                RowVersion = rowVersionActualizado
            };

            _paqueteRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        paqueteId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(paqueteExistente);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        5,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultor);

            _areaRepositoryMock
                .Setup(repository =>
                    repository.ExisteActivaAsync(
                        4,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _paqueteRepositoryMock
                .Setup(repository =>
                    repository.ActualizarAsync(
                        It.IsAny<PaqueteServicio>(),
                        rowVersion,
                        It.IsAny<CancellationToken>()))
                .Callback<
                    PaqueteServicio,
                    byte[],
                    CancellationToken>(
                    (paquete, _, _) =>
                    {
                        paqueteActualizado = paquete;
                    })
                .Returns(Task.CompletedTask);

            _paqueteRepositoryMock
                .Setup(repository =>
                    repository.ObtenerPorIdAsync(
                        paqueteId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            PaqueteServicioDto resultado =
                await _service.ActualizarAsync(
                    paqueteId,
                    request);

            // Assert
            Assert.NotNull(paqueteActualizado);

            Assert.Equal(
                4,
                paqueteActualizado.AreaEspecializacionId);

            Assert.Equal(
                5,
                paqueteActualizado.ConsultorId);

            Assert.Equal(
                60m,
                paqueteActualizado.TarifaHoraAplicada);

            Assert.Equal(
                480m,
                paqueteActualizado.Costo);

            Assert.Equal(
                480m,
                resultado.Costo);

            Assert.Equal(
                rowVersionActualizado,
                resultado.RowVersion);

            _paqueteRepositoryMock.Verify(
                repository =>
                    repository.ActualizarAsync(
                        It.IsAny<PaqueteServicio>(),
                        rowVersion,
                        It.IsAny<CancellationToken>()),
                Times.Once);

            _paqueteRepositoryMock.Verify(
                repository =>
                    repository.ActualizarAsync(
                        It.IsAny<PaqueteServicio>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task DesactivarAsync_DebeLanzarBusinessException_CuandoPaqueteYaEstaInactivo()
        {
            // Arrange
            const int paqueteId = 8;

            var paquete = new PaqueteServicio(
                nombre: "Administración financiera",
                areaEspecializacionId: 2,
                consultorId: 1,
                duracionHoras: 10,
                tarifaHoraAplicada: 45m,
                descripcion:
                    "Servicio de consultoría financiera.");

            paquete.Desactivar();

            _paqueteRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        paqueteId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(paquete);

            // Act
            Func<Task> accion = async () =>
                await _service.DesactivarAsync(paqueteId);

            // Assert
            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(
                    accion);

            Assert.Contains(
                "ya se encuentra inactivo",
                exception.Message);

            _paqueteRepositoryMock.Verify(
                repository =>
                    repository.DesactivarAsync(
                        It.IsAny<int>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ActivarAsync_DebeActivarPaquete_CuandoRelacionesEstanActivas()
        {
            // Arrange
            const int paqueteId = 10;
            const int consultorId = 5;
            const int areaId = 2;

            var paquete = new PaqueteServicio(
                nombre: "Consultoría financiera",
                areaEspecializacionId: areaId,
                consultorId: consultorId,
                duracionHoras: 10,
                tarifaHoraAplicada: 50m,
                descripcion: "Paquete financiero.");

            paquete.Desactivar();

            Consultor consultor =
                Consultor.Reconstruir(
                    consultorId: consultorId,
                    nombre: "Juan Gómez",
                    areaEspecializacionId: areaId,
                    tarifaHora: 50m,
                    emailCorporativo:
                        "juan.gomez@consultoria.com",
                    activo: true,
                    fechaIngreso: DateTime.UtcNow);

            var paqueteEsperado =
                new PaqueteServicioDto
                {
                    PaqueteId = paqueteId,
                    Nombre = "Consultoría financiera",
                    AreaEspecializacionId = areaId,
                    ConsultorId = consultorId,
                    DuracionHoras = 10,
                    TarifaHoraAplicada = 50m,
                    Costo = 500m,
                    Descripcion = "Paquete financiero.",
                    Activo = true
                };

            _paqueteRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        paqueteId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(paquete);

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

            _paqueteRepositoryMock
                .Setup(repository =>
                    repository.ActualizarAsync(
                        paquete,
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _paqueteRepositoryMock
                .Setup(repository =>
                    repository.ObtenerPorIdAsync(
                        paqueteId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(paqueteEsperado);

            // Act
            PaqueteServicioDto resultado =
                await _service.ActivarAsync(paqueteId);

            // Assert
            Assert.True(paquete.Activo);
            Assert.True(resultado.Activo);

            _paqueteRepositoryMock.Verify(
                repository =>
                    repository.ActualizarAsync(
                        paquete,
                        It.IsAny<CancellationToken>()),
                Times.Once);

            _paqueteRepositoryMock.Verify(
                repository =>
                    repository.ActualizarAsync(
                        paquete,
                        It.IsAny<byte[]>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ActivarAsync_DebeRechazarActivacion_CuandoConsultorEstaInactivo()
        {
            // Arrange
            const int paqueteId = 10;
            const int consultorId = 5;
            const int areaId = 2;

            var paquete = new PaqueteServicio(
                nombre: "Consultoría financiera",
                areaEspecializacionId: areaId,
                consultorId: consultorId,
                duracionHoras: 10,
                tarifaHoraAplicada: 50m,
                descripcion: "Paquete financiero.");

            paquete.Desactivar();

            Consultor consultor =
                Consultor.Reconstruir(
                    consultorId: consultorId,
                    nombre: "Juan Gómez",
                    areaEspecializacionId: areaId,
                    tarifaHora: 50m,
                    emailCorporativo:
                        "juan.gomez@consultoria.com",
                    activo: false,
                    fechaIngreso: DateTime.UtcNow);

            _paqueteRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        paqueteId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(paquete);

            _consultorRepositoryMock
                .Setup(repository =>
                    repository.ObtenerEntidadPorIdAsync(
                        consultorId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(consultor);

            // Act
            Func<Task> accion = async () =>
                await _service.ActivarAsync(paqueteId);

            // Assert
            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(
                    accion);

            Assert.Contains(
                "consultor asociado se encuentra inactivo",
                exception.Message);

            Assert.False(paquete.Activo);

            _paqueteRepositoryMock.Verify(
                repository =>
                    repository.ActualizarAsync(
                        It.IsAny<PaqueteServicio>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);

            _paqueteRepositoryMock.Verify(
                repository =>
                    repository.ActualizarAsync(
                        It.IsAny<PaqueteServicio>(),
                        It.IsAny<byte[]>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        private static CrearPaqueteServicioDto CrearRequestValido()
        {
            return new CrearPaqueteServicioDto
            {
                Nombre = "Administración financiera",
                ConsultorId = 2,
                DuracionHoras = 10,
                Descripcion =
                    "Taller de buenas prácticas financieras."
            };
        }

        private static Consultor CrearConsultor(
            int consultorId,
            int areaEspecializacionId,
            decimal tarifaHora,
            bool activo = true)
        {
            return Consultor.Reconstruir(
                consultorId: consultorId,
                nombre: "Juan Gómez",
                areaEspecializacionId:
                    areaEspecializacionId,
                tarifaHora: tarifaHora,
                emailCorporativo:
                    $"consultor{consultorId}@consultoria.com",
                activo: activo,
                fechaIngreso: DateTime.UtcNow);
        }
    }
}