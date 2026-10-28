using System.Security.Claims;
using SistemaGestionOdontologica.Api.Data;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Interfaces;

namespace SistemaGestionOdontologica.Api.Services
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditoriaService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Registrar(string accion, int idPaciente)
        {
            Registrar(accion, "Paciente", idPaciente);
        }

        public void Registrar(string accion, string entidad, int idEntidad)
        {
            var usuario = _httpContextAccessor.HttpContext?.User;

            if (usuario?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("No se encontró un usuario autenticado");

            string? identificador = usuario.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(identificador, out int idUsuario) || idUsuario <= 0)
                throw new UnauthorizedAccessException("No se pudo identificar al usuario autenticado.");

            // Se evita que cualquier texto arbitrario termine almacenado como nombre de entidad.
            if (entidad != "Paciente" && entidad != "Odontologo" && entidad != "Turno" && entidad != "Usuario")
            {
                throw new ArgumentException(
                    "Entidad de auditoría no permitida.",
                    nameof(entidad));
            }

            if (idEntidad <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(idEntidad));
            }

            _context.Set<Bitacora>().Add(new Bitacora
            {
                IdUsuario = idUsuario,
                Accion = accion,
                Fecha = DateTime.UtcNow,
                Detalles = $"{entidad}Id={idEntidad}"
            });
        }
    }
}
