using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Consultoria.Api.OpenApi
{
    public sealed class BearerSecuritySchemeTransformer(
     IAuthenticationSchemeProvider authenticationSchemeProvider)
     : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            IEnumerable<AuthenticationScheme> authenticationSchemes =
                await authenticationSchemeProvider.GetAllSchemesAsync();

            bool existeBearer = authenticationSchemes.Any(
                scheme =>
                    scheme.Name ==
                    JwtBearerDefaults.AuthenticationScheme);

            if (!existeBearer)
            {
                return;
            }

            document.Components ??= new OpenApiComponents();

            document.Components.SecuritySchemes ??=
                new Dictionary<string, IOpenApiSecurityScheme>();

            document.Components.SecuritySchemes["Bearer"] =
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "Ingresa el token JWT obtenido en el endpoint de login."
                };
        }
    }
}
