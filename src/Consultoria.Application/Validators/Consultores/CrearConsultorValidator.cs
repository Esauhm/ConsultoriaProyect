using Consultoria.Application.DTOs.Consultores;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Validators.Consultores
{
    public sealed class CrearConsultorValidator
      : AbstractValidator<CrearConsultorDto>
    {
        public CrearConsultorValidator()
        {
            RuleFor(x => x.Nombre)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El nombre del consultor es obligatorio.")
                .MaximumLength(150)
                .WithMessage(
                    "El nombre del consultor no puede superar los 150 caracteres.");

            RuleFor(x => x.AreaEspecializacionId)
                .GreaterThan(0)
                .WithMessage("El área de especialización es obligatoria.");

            RuleFor(x => x.TarifaHora)
                .InclusiveBetween(30m, 200m)
                .WithMessage(
                    "La tarifa por hora debe estar entre $30 y $200.");

            RuleFor(x => x.EmailCorporativo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El correo corporativo es obligatorio.")
                .EmailAddress()
                .WithMessage("El formato del correo corporativo no es válido.")
                .MaximumLength(150)
                .WithMessage(
                    "El correo corporativo no puede superar los 150 caracteres.");
        }
    }
}
