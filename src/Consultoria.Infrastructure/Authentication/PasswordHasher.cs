using Consultoria.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Infrastructure.Authentication
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private static readonly object UsuarioGenerico = new();

        private readonly PasswordHasher<object> _passwordHasher = new();

        public string GenerarHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    "La contraseña es obligatoria.",
                    nameof(password));
            }

            return _passwordHasher.HashPassword(
                UsuarioGenerico,
                password);
        }

        public bool Verificar(
            string password,
            string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            PasswordVerificationResult resultado =
                _passwordHasher.VerifyHashedPassword(
                    UsuarioGenerico,
                    passwordHash,
                    password);

            return resultado is
                PasswordVerificationResult.Success or
                PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
