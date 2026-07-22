using Consultoria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Infrastructure.Persistence.Configurations
{
    public sealed class ConsultorConfiguration
    : IEntityTypeConfiguration<Consultor>
    {
        public void Configure(EntityTypeBuilder<Consultor> builder)
        {
            builder.ToTable(
                "Consultores",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_Consultores_TarifaHora",
                        "[TarifaHora] >= 30 AND [TarifaHora] <= 200");
                });

            builder.HasKey(x => x.ConsultorId);

            builder.Property(x => x.ConsultorId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.AreaEspecializacionId)
                .IsRequired();

            builder.Property(x => x.TarifaHora)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(x => x.EmailCorporativo)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Activo)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.FechaIngreso)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.HasIndex(x => x.EmailCorporativo)
                .IsUnique()
                .HasDatabaseName(
                    "UX_Consultores_EmailCorporativo");

            builder.HasIndex(x => new
            {
                x.Nombre,
                x.AreaEspecializacionId
            })
                .IsUnique()
                .HasDatabaseName(
                    "UX_Consultores_Nombre_AreaEspecializacionId");

            builder.HasIndex(x => x.AreaEspecializacionId)
                .HasDatabaseName(
                    "IX_Consultores_AreaEspecializacionId");

            builder.HasOne<AreaEspecializacion>()
                .WithMany()
                .HasForeignKey(x => x.AreaEspecializacionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_Consultores_AreasEspecializacion");

            builder.Property(consultor => consultor.RowVersion)
                .IsRowVersion();
        }
    }
}
