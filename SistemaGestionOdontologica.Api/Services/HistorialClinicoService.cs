using Microsoft.EntityFrameworkCore;
using SistemaGestionOdontologica.Api.Data;
using SistemaGestionOdontologica.Api.DTOs.HistorialesClinicos;
using SistemaGestionOdontologica.Api.DTOs.Paginacion;
using SistemaGestionOdontologica.Api.Helpers;
using SistemaGestionOdontologica.Api.Interfaces;
using SistemaGestionOdontologica.Api.Security;
using System.Security.Claims;

namespace SistemaGestionOdontologica.Api.Services
{
    public sealed class HistorialClinicoService : IHistorialClinicoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditoriaService _auditoria;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HistorialClinicoService(ApplicationDbContext context, IAuditoriaService auditoria,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _auditoria = auditoria;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<int> ObtenerOdontologoAutenticadoAsync(
            CancellationToken cancellationToken)
        {
            var principal = _httpContextAccessor.HttpContext?.User;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                throw new AccesoProhibidoException(
                    "No se pudo verificar la identidad del usuario.");
            }

            var identificador = principal.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(identificador, out int idUsuario) ||
                idUsuario <= 0)
            {
                throw new AccesoProhibidoException(
                    "No se pudo identificar al usuario autenticado.");
            }

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.IdUsuario == idUsuario && u.Activo)
                .Select(u => new
                {
                    u.Rol,
                    u.IdOdontologo
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (usuario is null ||
                usuario.Rol != Roles.Odontologo ||
                !usuario.IdOdontologo.HasValue)
            {
                throw new AccesoProhibidoException(
                    "La operación requiere una cuenta de odontólogo activa.");
            }

            return usuario.IdOdontologo.Value;
        }

        private async Task<bool> TieneRelacionAsistencialAsync(int idPaciente, int idOdontologo,
            CancellationToken cancellationToken)
        {
            return await _context.Turnos
                .AsNoTracking()
                .AnyAsync(
                    t => t.IdPaciente == idPaciente &&
                         t.IdOdontologo == idOdontologo &&
                         t.Estado == "realizado",
                    cancellationToken);
        }

        public Task<ResultadoPaginadoDto<HistorialClinicoRespuestaDto>>
            ObtenerPaginadosAsync(HistorialClinicoFiltroDto filtro, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<HistorialClinicoRespuestaDto?> ObtenerPorIdAsync(int idHistorial,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<HistorialClinicoRespuestaDto>
            CrearAsync(CrearHistorialClinicoDto request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
