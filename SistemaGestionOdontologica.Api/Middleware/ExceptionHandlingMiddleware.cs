using Microsoft.AspNetCore.Mvc;
using SistemaGestionOdontologica.Api.Helpers;

namespace SistemaGestionOdontologica.Api.Middleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException)
                when (context.RequestAborted.IsCancellationRequested)
            {
                _logger.LogInformation("El cliente canceló la solicitud.");
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                int statusCode;
                string titulo;
                string detalle;

                switch (ex)
                {
                    case ValidacionNegocioException:
                        statusCode = StatusCodes.Status400BadRequest;
                        titulo = "Error de validacion";
                        detalle = ex.Message;
                        break;

                    case ConflictoNegocioException:
                        statusCode = StatusCodes.Status409Conflict;
                        titulo = "Conflicto de datos";
                        detalle = ex.Message;
                        break;

                    default:
                        statusCode = StatusCodes.Status500InternalServerError;
                        titulo = "Error interno del servidor";
                        detalle = "Ocurrió un error inesperado";
                        break;
                }

                if (statusCode == 500)
                {
                    _logger.LogError(
                        ex, "Error inesperado al procesar {Metodo} {Ruta}", context.Request.Method, context.Request.Path);
                }
                else
                {
                    _logger.LogWarning("Error de negocio {Codigo}: {Mensaje}", statusCode, ex.Message);
                }

                context.Response.Clear();
                context.Response.StatusCode = statusCode;

                var problema = new ProblemDetails
                {
                    Status = statusCode,
                    Title = titulo,
                    Detail = detalle,
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(
                    problema,
                    options: null,
                    contentType: "application/problem+json",
                    cancellationToken: context.RequestAborted);
            }
        }
    }
}
