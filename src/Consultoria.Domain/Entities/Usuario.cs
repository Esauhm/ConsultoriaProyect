using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Domain.Entities
{
    public class Usuario
    {
        public int UsuarioId { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string Rol { get; private set; } = string.Empty;
        public bool Activo { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        private Usuario()
        {
        }

        public Usuario(
            string nombre,
            string email,
            string passwordHash,
            string rol)
        {
            CambiarNombre(nombre);
            CambiarEmail(email);
            CambiarPasswordHash(passwordHash);
            CambiarRol(rol);

            Activo = true;
            FechaCreacion = DateTime.UtcNow;
        }

        public void CambiarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException(
                    "El nombre del usuario es obligatorio.",
                    nameof(nombre));

            Nombre = nombre.Trim();
        }

        public void CambiarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "El correo electrónico es obligatorio.",
                    nameof(email));

            Email = email.Trim().ToLowerInvariant();
        }

        public void CambiarPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException(
                    "La contraseña cifrada es obligatoria.",
                    nameof(passwordHash));

            PasswordHash = passwordHash;
        }

        public void CambiarRol(string rol)
        {
            if (rol != "Admin" && rol != "User")
                throw new ArgumentException(
                    "El rol debe ser Admin o User.",
                    nameof(rol));

            Rol = rol;
        }

        public void Activar()
        {
            Activo = true;
        }

        public void Desactivar()
        {
            Activo = false;
        }

        public static Usuario Reconstruir(
            int usuarioId,
            string nombre,
            string email,
            string passwordHash,
            string rol,
            bool activo,
            DateTime fechaCreacion)
        {
            return new Usuario
            {
                UsuarioId = usuarioId,
                Nombre = nombre,
                Email = email,
                PasswordHash = passwordHash,
                Rol = rol,
                Activo = activo,
                FechaCreacion = fechaCreacion
            };
        }
    }
}
