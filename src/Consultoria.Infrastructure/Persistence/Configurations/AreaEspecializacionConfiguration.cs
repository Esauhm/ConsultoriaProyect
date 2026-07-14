using Consultoria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Infrastructure.Persistence.Configurations
{
    public sealed class AreaEspecializacionConfiguration
    : IEntityTypeConfiguration<AreaEspecializacion>
    {
        public void Configure(
            EntityTypeBuilder<AreaEspecializacion> builder)
        {
            builder.ToTable("AreasEspecializacion");

            builder.HasKey(x => x.AreaEspecializacionId);

            builder.Property(x => x.AreaEspecializacionId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Activo)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasIndex(x => x.Nombre)
                .IsUnique()
                .HasDatabaseName("UX_AreasEspecializacion_Nombre");
        }
    }
}
