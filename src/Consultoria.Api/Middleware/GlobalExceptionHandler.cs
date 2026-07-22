using System.Diagnostics;
using Consultoria.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Consultoria.Api.Middleware
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            ErrorDetails errorDetails = ObtenerDetalles(exception);

            RegistrarError(
                exception,
                errorDetails.StatusCode,
                httpContext.TraceIdentifier);

            var problemDetails = new ProblemDetails
            {
                Status = errorDetails.StatusCode,
                Title = errorDetails.Title,
                Detail = errorDetails.Detail,
                Instance = httpContext.Request.Path
            };

            string traceId =
                Activity.Current?.Id ??
                httpContext.TraceIdentifier;

            problemDetails.Extensions["traceId"] = traceId;

            if (errorDetails.Errors is not null)
            {
                problemDetails.Extensions["errors"] =
                    errorDetails.Errors;
            }

            httpContext.Response.StatusCode =
                errorDetails.StatusCode;

            httpContext.Response.ContentType =
                "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);

            return true;
        }

        private ErrorDetails ObtenerDetalles(Exception exception)
        {
            return exception switch
            {
                ValidationException validationException =>
                    new ErrorDetails(
                        StatusCodes.Status400BadRequest,
                        "Error de validación",
                        "Uno o más campos contienen valores inválidos.",
                        ObtenerErroresValidacion(validationException)),

                NotFoundException =>
                    new ErrorDetails(
                        StatusCodes.Status404NotFound,
                        "Recurso no encontrado",
                        exception.Message),

                ConflictException =>
                    new ErrorDetails(
                        StatusCodes.Status409Conflict,
                        "Conflicto",
                        exception.Message),

                ConcurrencyException =>
                    new ErrorDetails(
                        StatusCodes.Status409Conflict,
                        "Conflicto de concurrencia",
                    exception.Message),

                UnauthorizedException =>
                    new ErrorDetails(
                        StatusCodes.Status401Unauthorized,
                        "No autorizado",
                        exception.Message),

                BusinessException =>
                    new ErrorDetails(
                        StatusCodes.Status422UnprocessableEntity,
                        "Regla de negocio no cumplida",
                        exception.Message),

                ArgumentException =>
                    new ErrorDetails(
                        StatusCodes.Status400BadRequest,
                        "Solicitud inválida",
                        exception.Message),

                _ =>
                    new ErrorDetails(
                        StatusCodes.Status500InternalServerError,
                        "Error interno del servidor",
                        ObtenerMensajeErrorInterno(exception))
            };
        }

        private string ObtenerMensajeErrorInterno(
            Exception exception)
        {
            return _environment.IsDevelopment()
                ? exception.Message
                : "Ocurrió un error inesperado al procesar la solicitud.";
        }

        private static Dictionary<string, string[]>
            ObtenerErroresValidacion(
                ValidationException exception)
        {
            return exception.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.ErrorMessage)
                        .Distinct()
                        .ToArray());
        }

        private void RegistrarError(
            Exception exception,
            int statusCode,
            string traceIdentifier)
        {
            if (statusCode >= 500)
            {
                _logger.LogError(
                    exception,
                    "Error no controlado. TraceId: {TraceId}",
                    traceIdentifier);

                return;
            }

            _logger.LogWarning(
                exception,
                "Solicitud rechazada con código {StatusCode}. TraceId: {TraceId}",
                statusCode,
                traceIdentifier);
        }

        private sealed record ErrorDetails(
            int StatusCode,
            string Title,
            string Detail,
            Dictionary<string, string[]>? Errors = null);
    }
}
