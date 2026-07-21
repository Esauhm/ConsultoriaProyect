using Consultoria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.UnitTests.Domain.Entities
{
    public sealed class AreaEspecializacionTests
    {
        [Fact]
        public void Constructor_DebeCrearAreaActiva_ConNombreCorrecto()
        {
            // Act
            var area = new AreaEspecializacion(
                "Desarrollo de Software");

            // Assert
            Assert.Equal(
                "Desarrollo de Software",
                area.Nombre);

            Assert.True(area.Activo);
        }

        [Fact]
        public void ActualizarNombre_DebeModificarElNombre()
        {
            // Arrange
            var area = new AreaEspecializacion(
                "Tecnología");

            // Act
            area.ActualizarNombre(
                "Arquitectura de Software");

            // Assert
            Assert.Equal(
                "Arquitectura de Software",
                area.Nombre);
        }

        [Fact]
        public void Constructor_DebeLanzarExcepcion_CuandoNombreEstaVacio()
        {
            // Act
            Action accion = () =>
                new AreaEspecializacion(" ");

            // Assert
            Assert.Throws<ArgumentException>(
                accion);
        }

        [Fact]
        public void ActivarYDesactivar_DebenCambiarElEstado()
        {
            // Arrange
            var area = new AreaEspecializacion(
                "Finanzas");

            // Act y Assert: desactivar
            area.Desactivar();

            Assert.False(area.Activo);

            // Act y Assert: activar
            area.Activar();

            Assert.True(area.Activo);
        }
    }
}
