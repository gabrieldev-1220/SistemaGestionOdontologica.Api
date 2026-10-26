using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaGestionOdontologica.Api.Data;
using SistemaGestionOdontologica.Api.DTOs.Odontologos;
using SistemaGestionOdontologica.Api.DTOs.Paginacion;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Helpers;
using SistemaGestionOdontologica.Api.Interfaces;

namespace SistemaGestionOdontologica.Api.Services
{
    public class OdontologoService : IOdontologoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditoriaService _auditoria;

        // CONSTRUCTOR.
        public OdontologoService(ApplicationDbContext context, IAuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        public async Task<ResultadoPaginadoDto<OdontologoRespuestaDto>> ObtenerPaginadosAsync(
            OdontologoFiltroDto filtro,
            CancellationToken cancellationToken = default)
        {
            if (filtro.Pagina < 1 ||
                filtro.TamanoPagina < 1 ||
                filtro.TamanoPagina > 100)
            {
                throw new ValidacionNegocioException(
                    "Los parámetros de paginación no son válidos.");
            }

            IQueryable<Odontologo> consulta = _context.Odontologos.AsNoTracking();

            if (filtro.Activo.HasValue)
            {
                consulta = consulta.Where(
                    o => o.Activo == filtro.Activo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
            {
                string busqueda = filtro.Busqueda.Trim();

                consulta = consulta.Where(o =>
                    o.Nombre.Contains(busqueda) ||
                    o.Apellido.Contains(busqueda) ||
                    o.Matricula.Contains(busqueda) ||
                    o.Especialidad.Contains(busqueda));
            }

            int totalRegistros = await consulta.CountAsync(
                cancellationToken);

            long desplazamiento =
                ((long)filtro.Pagina - 1) * filtro.TamanoPagina;

            var datos = new List<OdontologoRespuestaDto>();

            if (desplazamiento < totalRegistros)
            {
                datos = await consulta
                    .OrderBy(o => o.Apellido)
                    .ThenBy(o => o.Nombre)
                    .ThenBy(o => o.IdOdontologo)
                    .Skip((int)desplazamiento)
                    .Take(filtro.TamanoPagina)
                    .Select(o => new OdontologoRespuestaDto
                    {
                        IdOdontologo = o.IdOdontologo,
                        Nombre = o.Nombre,
                        Apellido = o.Apellido,
                        Matricula = o.Matricula,
                        Telefono = o.Telefono,
                        Email = o.Email,
                        Especialidad = o.Especialidad,
                        Activo = o.Activo
                    })
                    .ToListAsync(cancellationToken);
            }

            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / filtro.TamanoPagina);

            return new ResultadoPaginadoDto<OdontologoRespuestaDto>
            {
                Datos = datos,
                Pagina = filtro.Pagina,
                TamanoPagina = filtro.TamanoPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas
            };
        }

        public async Task<OdontologoRespuestaDto?> ObtenerPorIdAsync(int idOdontologo, CancellationToken cancellationToken = default)
        {
            return await _context.Odontologos
                .AsNoTracking()
                .Where(o => o.IdOdontologo == idOdontologo)
                .Select(o => new OdontologoRespuestaDto
                {
                    IdOdontologo = o.IdOdontologo,
                    Nombre = o.Nombre,
                    Apellido = o.Apellido,
                    Matricula = o.Matricula,
                    Telefono = o.Telefono,
                    Email = o.Email,
                    Especialidad = o.Especialidad,
                    Activo = o.Activo
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<OdontologoRespuestaDto> CrearAsync(CrearOdontologoDto request, CancellationToken cancellationToken = default)
        {
            ValidarDatos(
                request.Nombre,
                request.Apellido,
                request.Matricula);

            string matricula = request.Matricula.Trim();

            bool existe = await _context.Odontologos.AnyAsync(
                o => o.Matricula == matricula,
                cancellationToken);

            if (existe)
                throw new ConflictoNegocioException(
                    "Ya existe un odontólogo con esa matrícula.");

            var odontologo = new Odontologo
            {
                Nombre = request.Nombre.Trim(),
                Apellido = request.Apellido.Trim(),
                Matricula = matricula,
                Telefono = request.Telefono?.Trim() ?? string.Empty,
                Email = request.Email?.Trim() ?? string.Empty,
                Especialidad = request.Especialidad?.Trim() ?? string.Empty,
                Activo = true
            };

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                _context.Odontologos.Add(odontologo);

                await GuardarCambiosAsync(cancellationToken);

                _auditoria.Registrar(
                    "ODONTOLOGO_CREADO",
                    "Odontologo",
                    odontologo.IdOdontologo);

                await GuardarCambiosAsync(cancellationToken);

                await transaccion.CommitAsync(cancellationToken);
                
                return MapearRespuesta(odontologo);

            }
            catch
            {
                await transaccion.RollbackAsync(cancellationToken);

                throw;
            }
        }

        public async Task<OdontologoRespuestaDto?> ActualizarAsync(
            int idOdontologo,
            ActualizarOdontologoDto request,
            CancellationToken cancellationToken = default)
        {
            var odontologo = await _context.Odontologos
                .FirstOrDefaultAsync(
                o => o.IdOdontologo == idOdontologo,
                cancellationToken);

            if (odontologo is null)
                return null;

            ValidarDatos(
                request.Nombre,
                request.Apellido,
                request.Matricula);

            string matricula = request.Matricula.Trim();

            bool ocupado = await _context.Odontologos.AnyAsync(
                o => o.Matricula == matricula &&
                     o.IdOdontologo != idOdontologo,
                cancellationToken);

            if (ocupado)
                throw new ConflictoNegocioException("La matrícula pertenece a otro odontólogo.");

            odontologo.Nombre = request.Nombre.Trim();
            odontologo.Apellido = request.Apellido.Trim();
            odontologo.Matricula = matricula;
            odontologo.Telefono = request.Telefono?.Trim() ?? string.Empty;
            odontologo.Email = request.Email?.Trim() ?? string.Empty;
            odontologo.Especialidad = request.Especialidad?.Trim() ?? string.Empty;

            _auditoria.Registrar(
                "ODONTOLOGO_ACTUALIZADO",
                "Odontologo",
                odontologo.IdOdontologo);

            await GuardarCambiosAsync(cancellationToken);

            return MapearRespuesta(odontologo);
        }

        public async Task<bool> DesactivarAsync(
            int idOdontologo,
            CancellationToken cancellationToken = default)
        {
            var odontologo = await _context.Odontologos
                .FirstOrDefaultAsync(
                o => o.IdOdontologo == idOdontologo,
                cancellationToken);

            if (odontologo is null)
                return false;

            if (!odontologo.Activo)
                return true;

            bool tieneUsuariosActivos =
                await _context.Usuarios.AnyAsync(
                    u => u.IdOdontologo == idOdontologo &&
                         u.Activo,
                    cancellationToken);

            if (tieneUsuariosActivos)
                throw new ConflictoNegocioException("No se puede desactivar el odontólogo porque tiene cuentas de usuarios activas vinculadas.");

            odontologo.Activo = false;

            _auditoria.Registrar(
                "ODONTOLOGO_DESACTIVADO",
                "Odontologo",
                odontologo.IdOdontologo);

            await GuardarCambiosAsync(cancellationToken);

            return true;
        }

        public async Task<bool> ActivarAsync(
            int idOdontologo,
            CancellationToken cancellationToken = default)
        {
            var odontologo = await _context.Odontologos
                .FirstOrDefaultAsync(
                o => o.IdOdontologo == idOdontologo,
                cancellationToken);

            if (odontologo is null)
                return false;

            if (odontologo.Activo)
                return true;

            odontologo.Activo = true;

            _auditoria.Registrar(
                "ODONTOLOGO_ACTIVADO",
                "Odontologo",
                odontologo.IdOdontologo);

            await GuardarCambiosAsync(cancellationToken);

            return true;
        }

        private static void ValidarDatos(string nombre, string apellido, string matricula)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(matricula))
                throw new ValidacionNegocioException("Nombre, apellido y matrícula son obligatorios.");

            if (nombre.Trim().Length > 50 ||
                apellido.Trim().Length > 50 ||
                matricula.Trim().Length > 30)
                throw new ValidacionNegocioException("Uno o más campos superan la longitud permitida.");
        }

        private async Task GuardarCambiosAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is SqlException sqlEx &&
                     (sqlEx.Number == 2601 ||
                      sqlEx.Number == 2627))
            {
                throw new ConflictoNegocioException(
                    "No se pudo guardar el odontólogo porque existe un valor único duplicado.");
            }
        }

        private static OdontologoRespuestaDto MapearRespuesta(Odontologo odontologo)
        {
            return new OdontologoRespuestaDto
            {
                IdOdontologo = odontologo.IdOdontologo,
                Nombre = odontologo.Nombre,
                Apellido = odontologo.Apellido,
                Matricula = odontologo.Matricula,
                Telefono = odontologo.Telefono,
                Email = odontologo.Email,
                Especialidad = odontologo.Especialidad,
                Activo = odontologo.Activo
            };
        }
    }
}
