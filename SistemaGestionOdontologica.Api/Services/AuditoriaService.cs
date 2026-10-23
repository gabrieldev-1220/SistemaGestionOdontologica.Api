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
            var usuario = _httpContextAccessor.HttpContext?.User;

            if (usuario?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("No se encontró un usuario autenticado");

            string? identificador = usuario.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(identificador, out int idUsuario) || idUsuario <= 0)
                throw new UnauthorizedAccessException("No se pudo identificar al usuario autenticado.");

            var registro = new Bitacora
            {
                IdUsuario = idUsuario,
                Accion = accion,
                Fecha = DateTime.UtcNow,
                Detalles = $"PacienteId={idPaciente}"
            };

            _context.Set<Bitacora>().Add(registro);
        }
    }
}
