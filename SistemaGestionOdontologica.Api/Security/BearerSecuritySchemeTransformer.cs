using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SistemaGestionOdontologica.Api.Security
{
    public sealed class BearerSecuritySchemeTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var scheme = await authenticationSchemeProvider.GetAllSchemesAsync();

            if (!scheme.Any(s => s.Name == "Bearer"))
            {
                return;
            }

            document.Components ??= new OpenApiComponents();

            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Introduce al accessToken JWT."
            };

            // Se aplica el esquema como requisito predeterminado de las operaciones documentadas.
            foreach (var path in document.Paths.Values)
            {
                if (path?.Operations is null)
                {
                    continue;
                }

                foreach (var operation in path.Operations.Values)
                {
                    if (operation is null)
                    {
                        continue;
                    }

                    operation.Security ??= new List<OpenApiSecurityRequirement>();

                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
                }
            }
        }
    }
}
