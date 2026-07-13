using Consultoria.Application.Common.Pagination;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consultoria.Application.Validators.common
{
    public abstract class PaginationRequestValidator<TRequest>
    : AbstractValidator<TRequest>
    where TRequest : PaginationRequest
    {
        protected PaginationRequestValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("El número de página debe ser mayor o igual a 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage(
                    "La cantidad de registros por página debe estar entre 1 y 100.");

            RuleFor(x => x.SortDir)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("La dirección de ordenamiento es obligatoria.")
                .Must(EsDireccionValida)
                .WithMessage(
                    "La dirección de ordenamiento debe ser 'asc' o 'desc'.");
        }

        private static bool EsDireccionValida(string sortDir)
        {
            return sortDir.Equals(
                       "asc",
                       StringComparison.OrdinalIgnoreCase)
                   || sortDir.Equals(
                       "desc",
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}
