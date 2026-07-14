using Consultoria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Infrastructure.Persistence.Context
{
    public sealed class ConsultoriaDbContext : DbContext
    {
        public ConsultoriaDbContext(
            DbContextOptions<ConsultoriaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Consultor> Consultores => Set<Consultor>();

        public DbSet<PaqueteServicio> PaquetesServicio =>
            Set<PaqueteServicio>();

        public DbSet<AreaEspecializacion> AreasEspecializacion =>
            Set<AreaEspecializacion>();

        public DbSet<Usuario> Usuarios => Set<Usuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ConsultoriaDbContext).Assembly);
        }
    }
}
