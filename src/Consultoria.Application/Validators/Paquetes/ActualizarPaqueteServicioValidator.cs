using Consultoria.Application.DTOs.Paquetes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Validators.Paquetes
{
    public sealed class ActualizarPaqueteServicioValidator
    : AbstractValidator<ActualizarPaqueteServicioDto>
    {
        public ActualizarPaqueteServicioValidator()
        {
            RuleFor(request => request.Nombre)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(
                    "El nombre del paquete de servicio es obligatorio.")
                .MinimumLength(3)
                .WithMessage(
                    "El nombre del paquete debe tener al menos 3 caracteres.")
                .MaximumLength(150)
                .WithMessage(
                    "El nombre del paquete no puede superar los 150 caracteres.");

            RuleFor(request => request.ConsultorId)
                .GreaterThan(0)
                .WithMessage(
                    "Debe seleccionar un consultor válido.");

            RuleFor(request => request.DuracionHoras)
                .GreaterThan(0)
                .WithMessage(
                    "La duración del paquete debe ser mayor que cero.");

            RuleFor(request => request.Descripcion)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(
                    "La descripción del paquete es obligatoria.")
                .MaximumLength(500)
                .WithMessage(
                    "La descripción no puede superar los 500 caracteres.");
        }
    }
}
