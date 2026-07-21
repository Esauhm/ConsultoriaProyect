using Consultoria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.UnitTests.Domain.Entities
{
    public sealed class ConsultorTests
    {
        [Fact]
        public void Constructor_DebeCrearConsultorActivo_ConDatosCorrectos()
        {
            // Act
            var consultor = new Consultor(
                nombre: "Juan Gómez",
                areaEspecializacionId: 2,
                tarifaHora: 45m,
                emailCorporativo: "juan.gomez@consultoria.com");

            // Assert
            Assert.Equal("Juan Gómez", consultor.Nombre);
            Assert.Equal(2, consultor.AreaEspecializacionId);
            Assert.Equal(45m, consultor.TarifaHora);
            Assert.Equal(
                "juan.gomez@consultoria.com",
                consultor.EmailCorporativo);

            Assert.True(consultor.Activo);
            Assert.NotEqual(default, consultor.FechaIngreso);
        }

        [Fact]
        public void Actualizar_DebeModificarLosDatosDelConsultor()
        {
            // Arrange
            var consultor = new Consultor(
                nombre: "Juan Gómez",
                areaEspecializacionId: 2,
                tarifaHora: 45m,
                emailCorporativo: "juan.gomez@consultoria.com");

            // Act
            consultor.Actualizar(
                nombre: "Juan Carlos Gómez",
                areaEspecializacionId: 3,
                tarifaHora: 60m,
                emailCorporativo: "juan.carlos@consultoria.com");

            // Assert
            Assert.Equal(
                "Juan Carlos Gómez",
                consultor.Nombre);

            Assert.Equal(
                3,
                consultor.AreaEspecializacionId);

            Assert.Equal(
                60m,
                consultor.TarifaHora);

            Assert.Equal(
                "juan.carlos@consultoria.com",
                consultor.EmailCorporativo);
        }

        [Fact]
        public void ActivarYDesactivar_DebenCambiarElEstadoDelConsultor()
        {
            // Arrange
            var consultor = new Consultor(
                nombre: "Juan Gómez",
                areaEspecializacionId: 2,
                tarifaHora: 45m,
                emailCorporativo: "juan.gomez@consultoria.com");

            // Act y Assert: desactivar
            consultor.Desactivar();

            Assert.False(consultor.Activo);

            // Act y Assert: reactivar
            consultor.Activar();

            Assert.True(consultor.Activo);
        }
    }
}
