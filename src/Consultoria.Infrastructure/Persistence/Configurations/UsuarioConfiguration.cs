using Consultoria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Infrastructure.Persistence.Configurations
{
    public sealed class UsuarioConfiguration
    : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable(
                "Usuarios",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_Usuarios_Rol",
                        "[Rol] IN ('Admin', 'User')");
                });

            builder.HasKey(x => x.UsuarioId);

            builder.Property(x => x.UsuarioId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Rol)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Activo)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.FechaCreacion)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("UX_Usuarios_Email");

            builder.HasIndex(x => new
            {
                x.Rol,
                x.Activo
            })
                .HasDatabaseName("IX_Usuarios_Rol_Activo");
        }
    }
}
