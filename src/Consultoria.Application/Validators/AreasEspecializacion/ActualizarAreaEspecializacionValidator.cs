using Consultoria.Application.DTOs.AreasEspecializacion;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Validators.AreasEspecializacion
{
    public sealed class ActualizarAreaEspecializacionValidator
    : AbstractValidator<ActualizarAreaEspecializacionDto>
    {
        public ActualizarAreaEspecializacionValidator()
        {
            RuleFor(request => request.Nombre)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(
                    "El nombre del área de especialización es obligatorio.")
                .MinimumLength(3)
                .WithMessage(
                    "El nombre del área debe tener al menos 3 caracteres.")
                .MaximumLength(100)
                .WithMessage(
                    "El nombre del área no puede superar los 100 caracteres.");

            RuleFor(request => request.RowVersion)
                .NotEmpty()
                .WithMessage(
                    "La versión del registro es obligatoria.");
        }
    }
}
