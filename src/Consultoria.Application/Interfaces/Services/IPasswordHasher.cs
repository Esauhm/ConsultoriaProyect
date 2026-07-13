using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Interfaces.Services
{
    public interface IPasswordHasher
    {
        string GenerarHash(string password);

        bool Verificar(
            string password,
            string passwordHash);
    }
}
