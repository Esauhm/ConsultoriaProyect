using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Consultoria.Api.OpenApi
{
    public sealed class BearerSecurityRequirementTransformer
    : IOpenApiOperationTransformer
    {
        public Task TransformAsync(
            OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            var metadata =
                context.Description
                    .ActionDescriptor
                    .EndpointMetadata;

            bool permiteAnonimos = metadata
                .OfType<AllowAnonymousAttribute>()
                .Any();

            bool requiereAutorizacion = metadata
                .OfType<IAuthorizeData>()
                .Any();

            if (permiteAnonimos || !requiereAutorizacion)
            {
                return Task.CompletedTask;
            }

            if (context.Document is null)
            {
                return Task.CompletedTask;
            }

            operation.Security ??= [];

            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecuritySchemeReference(
                            "Bearer",
                            context.Document)
                    ] = []
                });

            return Task.CompletedTask;
        }
    }
}
