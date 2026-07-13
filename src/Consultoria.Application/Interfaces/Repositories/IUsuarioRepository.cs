using Consultoria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorEmailAsync(
            string email,
            CancellationToken cancellationToken = default);
    }
}
