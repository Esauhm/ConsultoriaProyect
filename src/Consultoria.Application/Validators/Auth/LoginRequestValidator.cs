using Consultoria.Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Validators.Auth
{
    public sealed class LoginRequestValidator
    : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress()
                .WithMessage("El formato del correo electrónico no es válido.")
                .MaximumLength(150)
                .WithMessage(
                    "El correo electrónico no puede superar los 150 caracteres.");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8)
                .WithMessage(
                    "La contraseña debe contener al menos 8 caracteres.")
                .MaximumLength(100)
                .WithMessage(
                    "La contraseña no puede superar los 100 caracteres.");
        }
    }
}
