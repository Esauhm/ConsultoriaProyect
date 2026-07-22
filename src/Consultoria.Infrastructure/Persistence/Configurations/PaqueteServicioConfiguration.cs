using Consultoria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Infrastructure.Persistence.Configurations
{
    public sealed class PaqueteServicioConfiguration
    : IEntityTypeConfiguration<PaqueteServicio>
    {
        public void Configure(
            EntityTypeBuilder<PaqueteServicio> builder)
        {
            builder.ToTable(
                "PaquetesServicio",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_PaquetesServicio_DuracionHoras",
                        "[DuracionHoras] > 0");

                    tableBuilder.HasCheckConstraint(
                        "CK_PaquetesServicio_TarifaHoraAplicada",
                        "[TarifaHoraAplicada] > 0");

                    tableBuilder.HasCheckConstraint(
                        "CK_PaquetesServicio_Costo",
                        "[Costo] > 0");
                });

            builder.HasKey(x => x.PaqueteId);

            builder.Property(x => x.PaqueteId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.AreaEspecializacionId)
                .IsRequired();

            builder.Property(x => x.ConsultorId)
                .IsRequired();

            builder.Property(x => x.DuracionHoras)
                .IsRequired();

            builder.Property(paquete => paquete.TarifaHoraAplicada)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(paquete => paquete.Costo)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Descripcion)
                .HasMaxLength(500);

            builder.Property(x => x.Activo)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.FechaCreacion)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.HasIndex(x => x.AreaEspecializacionId)
                .HasDatabaseName(
                    "IX_PaquetesServicio_AreaEspecializacionId");

            builder.HasIndex(x => x.ConsultorId)
                .HasDatabaseName(
                    "IX_PaquetesServicio_ConsultorId");

            builder.HasIndex(x => new
            {
                x.AreaEspecializacionId,
                x.Activo
            })
                .HasDatabaseName(
                    "IX_PaquetesServicio_AreaEspecializacionId_Activo");

            builder.HasIndex(x => new
            {
                x.ConsultorId,
                x.Activo
            })
                .HasDatabaseName(
                    "IX_PaquetesServicio_ConsultorId_Activo");

            builder.HasOne<AreaEspecializacion>()
                .WithMany()
                .HasForeignKey(x => x.AreaEspecializacionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_PaquetesServicio_AreasEspecializacion");

            builder.HasOne<Consultor>()
                .WithMany()
                .HasForeignKey(x => x.ConsultorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_PaquetesServicio_Consultores");

            builder.Property(paquete => paquete.RowVersion)
                .IsRowVersion();
        }
    }
}
