using Consultoria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.UnitTests.Domain.Entities
{
    public sealed class PaqueteServicioTests
    {
        [Fact]
        public void Constructor_DebeCalcularCosto_UsandoDuracionYTarifa()
        {
            // Arrange
            const int duracionHoras = 10;
            const decimal tarifaHora = 45m;

            // Act
            var paquete = new PaqueteServicio(
                nombre: "Administración financiera",
                areaEspecializacionId: 2,
                consultorId: 1,
                duracionHoras: duracionHoras,
                tarifaHoraAplicada: tarifaHora,
                descripcion: "Servicio de consultoría financiera.");

            // Assert
            Assert.Equal(450m, paquete.Costo);
            Assert.Equal(45m, paquete.TarifaHoraAplicada);
            Assert.Equal(10, paquete.DuracionHoras);
            Assert.True(paquete.Activo);
        }

        [Fact]
        public void Constructor_DebeLanzarExcepcion_CuandoDuracionEsCero()
        {
            // Act
            Action crearPaquete = () =>
                new PaqueteServicio(
                    nombre: "Administración financiera",
                    areaEspecializacionId: 2,
                    consultorId: 1,
                    duracionHoras: 0,
                    tarifaHoraAplicada: 45m,
                    descripcion: "Servicio de consultoría financiera.");

            // Assert
            ArgumentOutOfRangeException exception =
                Assert.Throws<ArgumentOutOfRangeException>(
                    crearPaquete);

            Assert.Equal(
                "duracionHoras",
                exception.ParamName);
        }

        [Fact]
        public void Constructor_DebeLanzarExcepcion_CuandoTarifaEsCero()
        {
            // Act
            Action crearPaquete = () =>
                new PaqueteServicio(
                    nombre: "Administración financiera",
                    areaEspecializacionId: 2,
                    consultorId: 1,
                    duracionHoras: 10,
                    tarifaHoraAplicada: 0,
                    descripcion: "Servicio de consultoría financiera.");

            // Assert
            ArgumentOutOfRangeException exception =
                Assert.Throws<ArgumentOutOfRangeException>(
                    crearPaquete);

            Assert.Equal(
                "tarifaHoraAplicada",
                exception.ParamName);
        }

        [Fact]
        public void Desactivar_DebeCambiarActivoAFalso()
        {
            // Arrange
            var paquete = new PaqueteServicio(
                nombre: "Administración financiera",
                areaEspecializacionId: 2,
                consultorId: 1,
                duracionHoras: 10,
                tarifaHoraAplicada: 45m,
                descripcion: "Servicio de consultoría financiera.");

            // Act
            paquete.Desactivar();

            // Assert
            Assert.False(paquete.Activo);
        }

        [Fact]
        public void Activar_DebeCambiarActivoAVerdadero()
        {
            // Arrange
            var paquete = new PaqueteServicio(
                nombre: "Administración financiera",
                areaEspecializacionId: 2,
                consultorId: 1,
                duracionHoras: 10,
                tarifaHoraAplicada: 45m,
                descripcion: "Servicio de consultoría financiera.");

            paquete.Desactivar();

            // Act
            paquete.Activar();

            // Assert
            Assert.True(paquete.Activo);
        }
    }
}
