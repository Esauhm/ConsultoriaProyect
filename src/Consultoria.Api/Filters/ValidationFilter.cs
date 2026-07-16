using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Consultoria.Api.Filters
{
    public sealed class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var errores = new List<ValidationFailure>();

            foreach (object? argumento in context.ActionArguments.Values)
            {
                if (argumento is null)
                {
                    continue;
                }

                Type tipoModelo = argumento.GetType();

                Type tipoValidador =
                    typeof(IValidator<>).MakeGenericType(tipoModelo);

                object? servicioValidador =
                    context.HttpContext.RequestServices
                        .GetService(tipoValidador);

                if (servicioValidador is not IValidator validador)
                {
                    continue;
                }

                Type tipoContexto =
                    typeof(ValidationContext<>).MakeGenericType(tipoModelo);

                var contextoValidacion =
                    (IValidationContext?)Activator.CreateInstance(
                        tipoContexto,
                        argumento);

                if (contextoValidacion is null)
                {
                    continue;
                }

                ValidationResult resultado =
                    await validador.ValidateAsync(
                        contextoValidacion,
                        context.HttpContext.RequestAborted);

                if (!resultado.IsValid)
                {
                    errores.AddRange(resultado.Errors);
                }
            }

            if (errores.Count > 0)
            {
                throw new ValidationException(errores);
            }

            await next();
        }
    }
}
