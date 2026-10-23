using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaGestionOdontologica.Api.Data;
using SistemaGestionOdontologica.Api.DTOs.Pacientes;
using SistemaGestionOdontologica.Api.DTOs.Paginacion;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Helpers;
using SistemaGestionOdontologica.Api.Interfaces;

namespace SistemaGestionOdontologica.Api.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditoriaService _auditoriaService;

        public PacienteService(ApplicationDbContext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        // Obtener pcientes con filtros y paginación.
        public async Task<ResultadoPaginadoDto<PacienteRespuestaDto>> ObtenerPaginadosAsync(
            PacienteFiltroDto filtro, CancellationToken cancellationToken = default)
        {
            if (filtro.Pagina < 1 || filtro.TamanoPagina < 1 || filtro.TamanoPagina > 100)
            {
                throw new ValidacionNegocioException("Los parámetros depaginación no son válidos.");
            }

            IQueryable<Paciente> consulta = _context.Pacientes
                .AsNoTracking();

            if (filtro.Activo.HasValue)
            {
                consulta = consulta.Where(p => p.Activo == filtro.Activo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
            {
                string busqueda = filtro.Busqueda.Trim();

                consulta = consulta.Where(p => p.Nombre.Contains(busqueda) ||
                p.Apellido.Contains(busqueda) ||
                p.Dni.Contains(busqueda));
            }

            int totalRegistros = await consulta.CountAsync(cancellationToken);

            // Se utiliza long para evitar desbordamientos al calcular desplazamientos de páginas grandes.
            long desplazamiento = ((long)filtro.Pagina - 1) * filtro.TamanoPagina;

            var datos = new List<PacienteRespuestaDto>();

            if (desplazamiento < totalRegistros)
            {
                datos = await consulta
                    .OrderBy(p => p.Apellido)
                    .ThenBy(p => p.Nombre)
                    .ThenBy(p => p.IdPaciente)
                    .Skip((int)desplazamiento)
                    .Take(filtro.TamanoPagina)
                    .Select(p => new PacienteRespuestaDto
                    {
                        IdPaciente = p.IdPaciente,
                        Nombre = p.Nombre,
                        Apellido = p.Apellido,
                        Dni = p.Dni,
                        FechaNacimiento = p.FechaNacimiento,
                        Telefono = p.Telefono,
                        Email = p.Email,
                        Direccion = p.Direccion,
                        FechaRegistro = p.FechaRegistro,
                        Activo = p.Activo
                    })
                    .ToListAsync(cancellationToken);
            }

            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / filtro.TamanoPagina);

            return new ResultadoPaginadoDto<PacienteRespuestaDto>
            {
                Datos = datos,
                Pagina = filtro.Pagina,
                TamanoPagina = filtro.TamanoPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas
            };
        }

        // Obtener un paciente por identificador.
        public async Task<PacienteRespuestaDto?> ObtenerPorIdAsync(int idPaciente, CancellationToken cancellationToken = default)
        {
            return await _context.Pacientes
                .AsNoTracking()
                .Where(p => p.IdPaciente == idPaciente)
                .Select(p => new PacienteRespuestaDto
                {
                    IdPaciente = p.IdPaciente,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    Dni = p.Dni,
                    FechaNacimiento = p.FechaNacimiento,
                    Telefono = p.Telefono,
                    Email = p.Email,
                    Direccion = p.Direccion,
                    FechaRegistro = p.FechaRegistro,
                    Activo = p.Activo
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        // Registrar un paciente.
        public async Task<PacienteRespuestaDto> CrearAsync(CrearPacienteDto request, CancellationToken cancellationToken = default)
        {
            ValidarDatos(
                request.Nombre,
                request.Apellido,
                request.Dni,
                request.FechaNacimiento,
                request.Direccion);

            string dni = request.Dni.Trim();

            bool existeDni = await _context.Pacientes
                .AnyAsync(p => p.Dni == dni, cancellationToken);

            if (existeDni)
            {
                throw new ConflictoNegocioException("Ya existe un paciente registrado con ese DNI.");
            }

            var paciente = new Paciente
            {
                Nombre = request.Nombre.Trim(),
                Apellido = request.Apellido.Trim(),
                Dni = dni,
                FechaNacimiento = request.FechaNacimiento!.Value,
                Telefono = request.Telefono?.Trim() ?? string.Empty,
                Email = request.Email?.Trim() ?? string.Empty,
                Direccion = NormalizarOpcional(request.Direccion),
                FechaRegistro = DateTime.UtcNow,
                Activo = true
            };

            // Se inicia una transacción para que el paciente y su auditoría se guarden de forma atómica.
            await using var transaccion = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Se guarda el paciente para obtener el ID generado automáticamente por SQL Server.
                _context.Pacientes.Add(paciente);

                await GuardarCambiosAsync(cancellationToken);

                // Se registra la uditoría dentrod e la misma transacción.
                _auditoriaService.Registrar("PACIENTE_CREADO", paciente.IdPaciente);

                await GuardarCambiosAsync(cancellationToken);

                // Se confirma solamente cuando ambas operaciones terminaron correctamente.
                await transaccion.CommitAsync(cancellationToken);

                return MapearRespuesta(paciente);
            }
            catch
            {
                // Si algo falla, se revierten los cambios.
                await transaccion.RollbackAsync(CancellationToken.None);

                throw;
            }
        }

        // Actualizar un paciente existente.
        public async Task<PacienteRespuestaDto?> ActualizarAsync(int idPaciente, ActualizarPacienteDto request,
            CancellationToken cancellationToken = default)
        {
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.IdPaciente == idPaciente, cancellationToken);

            if (paciente is null)
            {
                return null;
            }

            ValidarDatos(
               request.Nombre,
               request.Apellido,
               request.Dni,
               request.FechaNacimiento,
               request.Direccion);

            string dni = request.Dni.Trim();

            bool dniOcupado = await _context.Pacientes
                .AnyAsync(p => p.Dni == dni && p.IdPaciente != idPaciente, cancellationToken);

            if (dniOcupado)
            {
                throw new ConflictoNegocioException("El DNI ya pertenece a otro paciente.");
            }

            paciente.Nombre = request.Nombre.Trim();
            paciente.Apellido = request.Apellido.Trim();
            paciente.Dni = dni;
            paciente.FechaNacimiento = request.FechaNacimiento!.Value;
            paciente.Telefono = request.Telefono?.Trim() ?? string.Empty;
            paciente.Email = request.Email?.Trim() ?? string.Empty;
            paciente.Direccion = NormalizarOpcional(request.Direccion);

            _auditoriaService.Registrar("PACIENTE_ACTUALIZADO", paciente.IdPaciente);

            await GuardarCambiosAsync(cancellationToken);

            return MapearRespuesta(paciente);
        }

        public async Task<bool> DesactivarAsync(int idPaciente, CancellationToken cancellationToken = default)
        {
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.IdPaciente == idPaciente, cancellationToken);

            if (paciente is null)
                return false;

            if (!paciente.Activo)
                return true;

            paciente.Activo = false;

            _auditoriaService.Registrar("PACIENTE_DESACTIVADO", paciente.IdPaciente);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        // Reactivación.
        public async Task<bool> ActivarAsync(int idPaciente, CancellationToken cancellationToken = default)
        {
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.IdPaciente == idPaciente, cancellationToken);

            if (paciente is null)
                return false;

            if (paciente.Activo)
                return true;

            paciente.Activo = true;

            _auditoriaService.Registrar("PACIENTE_ACTIVADO", paciente.IdPaciente);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        // Validaciones compartidas por creaciñon y actualización.
        private static void ValidarDatos(string nombre, string apellido, string dni, DateOnly? fechaNacimiento, string? direccion)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(dni))
                throw new ValidacionNegocioException("Nombre, Apellido y DNI son obligatorios.");

            if (!fechaNacimiento.HasValue)
                throw new ValidacionNegocioException("La fecha de nacimiento es obligatoria.");

            if (fechaNacimiento.Value > DateOnly.FromDateTime(DateTime.Today))
                throw new ValidacionNegocioException("La fecha de nacimiento no puede ser futura.");

            // La columna direccion se configuró inicialmente como VARCHAR(100), aunque el DTO admite 200.
            if (direccion?.Trim().Length > 100)
                throw new ValidacionNegocioException("La dirección no puede superar los 100 caracteres.");
        }

        private static string? NormalizarOpcional(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? null
                : valor.Trim();
        }

        // Captura conflictos de claves únicasque puedan ocurrir, incluso después de la comprobación previa del DNI.
        private async Task GuardarCambiosAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                throw new ConflictoNegocioException("No se puedo guardar el paciente porque existe un valor único duplucado.");
            }
        }

        private static PacienteRespuestaDto MapearRespuesta(Paciente paciente)
        {
            return new PacienteRespuestaDto
            {
                IdPaciente = paciente.IdPaciente,
                Nombre = paciente.Nombre,
                Apellido = paciente.Apellido,
                Dni = paciente.Dni,
                FechaNacimiento = paciente.FechaNacimiento,
                Telefono = paciente.Telefono,
                Email = paciente.Email,
                Direccion = paciente.Direccion,
                FechaRegistro = paciente.FechaRegistro,
                Activo = paciente.Activo
            };
        }
    }
}
