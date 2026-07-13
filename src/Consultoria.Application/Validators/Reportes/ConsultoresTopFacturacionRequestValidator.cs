using Consultoria.Application.DTOs.Reportes;
using Consultoria.Application.Validators.common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Validators.Reportes
{
    public sealed class ConsultoresTopFacturacionRequestValidator
    : PaginationRequestValidator<ConsultoresTopFacturacionRequestDto>
    {
        private static readonly HashSet<string> CamposOrdenamientoPermitidos =
            new(StringComparer.OrdinalIgnoreCase)
            {
            "nombreConsultor",
            "areaEspecializacion",
            "tarifaHora",
            "cantidadPaquetes",
            "totalHoras",
            "totalFacturado"
            };

        public ConsultoresTopFacturacionRequestValidator()
        {
            RuleFor(x => x.AreaEspecializacionId)
                .GreaterThan(0)
                .WithMessage(
                    "El identificador del área debe ser mayor que cero.")
                .When(x => x.AreaEspecializacionId.HasValue);

            RuleFor(x => x.NombreConsultor)
                .MaximumLength(150)
                .WithMessage(
                    "El nombre del consultor no puede superar los 150 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.NombreConsultor));

            RuleFor(x => x.SortBy)
                .Must(EsCampoOrdenamientoValido)
                .WithMessage(
                    "El campo de ordenamiento para el reporte de consultores no es válido.");
        }

        private static bool EsCampoOrdenamientoValido(string? sortBy)
        {
            return string.IsNullOrWhiteSpace(sortBy)
                   || CamposOrdenamientoPermitidos.Contains(sortBy.Trim());
        }
    }
}
