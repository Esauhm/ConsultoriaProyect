using Consultoria.Application.DTOs.Reportes;
using Consultoria.Application.Validators.common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Validators.Reportes
{
    public sealed class PaquetesPorAreaRequestValidator
     : PaginationRequestValidator<PaquetesPorAreaRequestDto>
    {
        private static readonly HashSet<string> CamposOrdenamientoPermitidos =
            new(StringComparer.OrdinalIgnoreCase)
            {
            "areaEspecializacion",
            "cantidadPaquetes",
            "totalHoras",
            "costoTotal",
            "costoPromedio"
            };

        public PaquetesPorAreaRequestValidator()
        {
            RuleFor(x => x.AreaEspecializacionId)
                .GreaterThan(0)
                .WithMessage(
                    "El identificador del área debe ser mayor que cero.")
                .When(x => x.AreaEspecializacionId.HasValue);

            RuleFor(x => x.NombrePaquete)
                .MaximumLength(150)
                .WithMessage(
                    "El nombre del paquete no puede superar los 150 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.NombrePaquete));

            RuleFor(x => x.SortBy)
                .Must(EsCampoOrdenamientoValido)
                .WithMessage(
                    "El campo de ordenamiento para el reporte de paquetes no es válido.");
        }

        private static bool EsCampoOrdenamientoValido(string? sortBy)
        {
            return string.IsNullOrWhiteSpace(sortBy)
                   || CamposOrdenamientoPermitidos.Contains(sortBy.Trim());
        }
    }
}
