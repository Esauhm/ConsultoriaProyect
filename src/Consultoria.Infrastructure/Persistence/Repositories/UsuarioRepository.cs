using Consultoria.Application.Interfaces.Repositories;
using Consultoria.Domain.Entities;
using Consultoria.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Infrastructure.Persistence.Repositories
{
    public sealed class UsuarioRepository : IUsuarioRepository
    {
        private readonly ConsultoriaDbContext _context;

        public UsuarioRepository(ConsultoriaDbContext context)
        {
            _context = context;
        }

        public Task<Usuario?> ObtenerPorEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            return _context.Usuarios
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    usuario => usuario.Email == email,
                    cancellationToken);
        }
    }
}
