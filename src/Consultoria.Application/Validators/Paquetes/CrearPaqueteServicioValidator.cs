using Consultoria.Application.DTOs.Paquetes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Validators.Paquetes
{
    public sealed class CrearPaqueteServicioValidator
      : AbstractValidator<CrearPaqueteServicioDto>
    {
        public CrearPaqueteServicioValidator()
        {
            RuleFor(x => x.Nombre)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El nombre del paquete es obligatorio.")
                .MaximumLength(150)
                .WithMessage(
                    "El nombre del paquete no puede superar los 150 caracteres.");

            RuleFor(x => x.AreaEspecializacionId)
                .GreaterThan(0)
                .WithMessage("El área de especialización es obligatoria.");

            RuleFor(x => x.ConsultorId)
                .GreaterThan(0)
                .WithMessage("El consultor es obligatorio.");

            RuleFor(x => x.DuracionHoras)
                .GreaterThan(0)
                .WithMessage("La duración debe ser mayor que cero.")
                .LessThanOrEqualTo(1000)
                .WithMessage(
                    "La duración no puede superar las 1000 horas.");

            RuleFor(x => x.Costo)
                .GreaterThan(0)
                .WithMessage("El costo debe ser mayor que cero.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500)
                .WithMessage(
                    "La descripción no puede superar los 500 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.Descripcion));
        }
    }
}
