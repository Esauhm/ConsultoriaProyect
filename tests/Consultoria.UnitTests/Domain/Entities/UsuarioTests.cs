using Consultoria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.UnitTests.Domain.Entities
{
    public sealed class UsuarioTests
    {
        [Fact]
        public void Constructor_DebeCrearUsuarioActivo_ConDatosCorrectos()
        {
            // Act
            var usuario = new Usuario(
                nombre: "Administrador",
                email: "admin@demo.com",
                passwordHash: "HASH_DE_PRUEBA",
                rol: "Admin");

            // Assert
            Assert.Equal(
                "Administrador",
                usuario.Nombre);

            Assert.Equal(
                "admin@demo.com",
                usuario.Email);

            Assert.Equal(
                "HASH_DE_PRUEBA",
                usuario.PasswordHash);

            Assert.Equal(
                "Admin",
                usuario.Rol);

            Assert.True(usuario.Activo);

            Assert.NotEqual(
                default,
                usuario.FechaCreacion);
        }

        [Theory]
        [InlineData("Supervisor")]
        [InlineData("Invitado")]
        [InlineData("")]
        public void Constructor_DebeLanzarExcepcion_CuandoRolNoEsValido(
            string rol)
        {
            // Act
            Action accion = () =>
                new Usuario(
                    nombre: "Usuario",
                    email: "usuario@demo.com",
                    passwordHash: "HASH_DE_PRUEBA",
                    rol: rol);

            // Assert
            Assert.Throws<ArgumentException>(
                accion);
        }

        [Fact]
        public void Constructor_DebeLanzarExcepcion_CuandoEmailEstaVacio()
        {
            // Act
            Action accion = () =>
                new Usuario(
                    nombre: "Administrador",
                    email: " ",
                    passwordHash: "HASH_DE_PRUEBA",
                    rol: "Admin");

            // Assert
            Assert.Throws<ArgumentException>(
                accion);
        }

        [Fact]
        public void ActivarYDesactivar_DebenCambiarElEstado()
        {
            // Arrange
            var usuario = new Usuario(
                nombre: "Usuario Demo",
                email: "usuario@demo.com",
                passwordHash: "HASH_DE_PRUEBA",
                rol: "User");

            // Act y Assert: desactivar
            usuario.Desactivar();

            Assert.False(usuario.Activo);

            // Act y Assert: activar
            usuario.Activar();

            Assert.True(usuario.Activo);
        }
    }
}
