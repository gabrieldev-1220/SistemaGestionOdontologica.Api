using System.Data;
using System.Runtime.InteropServices;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaGestionOdontologica.Api.Data;
using SistemaGestionOdontologica.Api.DTOs.Paginacion;
using SistemaGestionOdontologica.Api.DTOs.Turnos;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Helpers;
using SistemaGestionOdontologica.Api.Interfaces;

namespace SistemaGestionOdontologica.Api.Services
{
    public class TurnoService : ITurnoService
    {
        private const int DuracionMaximaMinutos = 240;

        private readonly ApplicationDbContext _context;
        private readonly IAuditoriaService _auditoria;

        // CONSTRUCTOR.
        public TurnoService(ApplicationDbContext context, IAuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        public async Task<ResultadoPaginadoDto<TurnoRespuestaDto>> ObtenerPaginadosAsync(TurnoFiltroDto filtro,
            CancellationToken cancellationToken = default)
        {
            if (filtro.Pagina < 1 || filtro.TamanoPagina < 1 || filtro.TamanoPagina > 100)
                throw new ValidacionNegocioException("Los parámetros de paginación no son válidos.");

            if (filtro.FechaDesde.HasValue && filtro.FechaHasta.HasValue && filtro.FechaDesde > filtro.FechaHasta)
                throw new ValidacionNegocioException("La fecha inicial no puede superar la fecha final.");

            IQueryable<Turno> consulta = _context.Turnos
                .AsNoTracking();

            if (filtro.IdPaciente.HasValue)
                consulta = consulta.Where(t => t.IdPaciente == filtro.IdPaciente.Value);

            if (filtro.IdOdontologo.HasValue)
                consulta = consulta.Where(t => t.IdOdontologo == filtro.IdOdontologo.Value);

            if (!string.IsNullOrWhiteSpace(filtro.Estado))
            {
                string estado = filtro.Estado.Trim().ToLowerInvariant();

                if (!EsEstadoValido(estado))
                    throw new ValidacionNegocioException("El estado indicado no es válido");

                consulta = consulta.Where(t => t.Estado == estado);
            }

            if (filtro.FechaDesde.HasValue)
            {
                DateTime desde = filtro.FechaDesde.Value.ToDateTime(TimeOnly.MinValue);

                consulta = consulta.Where(t => t.FechaHoraInicio >= desde);
            }

            if (filtro.FechaHasta.HasValue)
            {
                DateTime hastaExclusiva;

                try
                {
                    hastaExclusiva = filtro.FechaHasta.Value
                        .AddDays(1)
                        .ToDateTime(TimeOnly.MinValue);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ValidacionNegocioException("La fecha final está fuera del rango permitido.");
                }

                consulta = consulta.Where(t => t.FechaHoraInicio < hastaExclusiva);
            }

            int totalRegistros = await consulta.CountAsync(cancellationToken);

            long desplazamiento = ((long)filtro.Pagina - 1) * filtro.TamanoPagina;

            var datos = new List<TurnoRespuestaDto>();

            if (desplazamiento < totalRegistros)
            {
                datos = await ProyectarRespuesta(consulta)
                    .OrderBy(t => t.FechaHoraInicio)
                    .ThenBy(t => t.IdTurno)
                    .Skip((int)desplazamiento)
                    .Take(filtro.TamanoPagina)
                    .ToListAsync(cancellationToken);
            }

            return new ResultadoPaginadoDto<TurnoRespuestaDto>
            {
                Datos = datos,
                Pagina = filtro.Pagina,
                TamanoPagina =filtro.TamanoPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = (int)Math.Ceiling((double)totalRegistros / filtro.TamanoPagina)
            };
        }

        public async Task<TurnoRespuestaDto?> ObtenerPorIdAsync(int idTurno, CancellationToken cancellationToken = default)
        {
            return await ProyectarRespuesta(_context.Turnos
                .AsNoTracking()
                .Where(t => t.IdTurno == idTurno))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TurnoRespuestaDto> CrearAsync(CrearTurnoDto request, CancellationToken cancellationToken = default)
        {
            DateTime inicio = ObtenerFechaRequerida(request.FechaHoraInicio, "inicio");

            DateTime fin = ObtenerFechaRequerida(request.FechaHoraFin, "finalizacion");

            ValidarHorario(inicio, fin);

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            try
            {
                await BloquearAgendaAsync(request.IdOdontologo, cancellationToken);

                await ValidarParticipantesAsync(
                    request.IdPaciente,
                    request.IdOdontologo,
                    cancellationToken);

                await ValidarDisponibilidadAsync(request.IdOdontologo, inicio, fin, null, cancellationToken);

                var turno = new Turno
                {
                    IdPaciente = request.IdPaciente,
                    IdOdontologo = request.IdOdontologo,
                    FechaHoraInicio = inicio,
                    FechaHoraFin = fin,
                    Estado = "pendiente",
                    Observaciones = request.Observaciones?.Trim()
                };

                _context.Turnos.Add(turno);

                await _context.SaveChangesAsync(cancellationToken);

                _auditoria.Registrar("TURNO_CREADO", "Turno", turno.IdTurno);

                await _context.SaveChangesAsync(cancellationToken);

                await transaccion.CommitAsync(cancellationToken);

                return (await ObtenerPorIdAsync(turno.IdTurno, cancellationToken))!;
            }
            catch
            {
                await transaccion.RollbackAsync(CancellationToken.None);

                throw;
            }
        }

        public async Task<TurnoRespuestaDto?> ActualizarAsync(int idTurno, ActualizarTurnoDto request,
            CancellationToken cancellationToken = default)
        {
            DateTime inicio = ObtenerFechaRequerida(request.FechaHoraInicio, "inicio");

            DateTime fin = ObtenerFechaRequerida(request.FechaHoraFin, "finalización");

            ValidarHorario(inicio, fin);

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            try
            {
                // Se bloquea primero el turno que se va a modificar.
                await BloquearTurnoAsync(idTurno, cancellationToken);

                var turno = await _context.Turnos
                    .FirstOrDefaultAsync(t => t.IdTurno == idTurno, cancellationToken);

                if (turno is null)
                {
                    await transaccion.CommitAsync(cancellationToken);
                    return null;
                }

                if (turno.Estado != "pendiente" && turno.Estado != "confirmado")
                    throw new ConflictoNegocioException("El turno no puede reprogramarse en su estado actual.");

                // Si se cambia de odontólogo, se protegen ambas agendas en un orden fijo para reducir bloqueos mutuos.
                foreach (int id in new[]
                    {
                       turno.IdOdontologo,
                       request.IdOdontologo
                    }.Distinct().OrderBy(x => x))
                {
                    await BloquearAgendaAsync(id, cancellationToken);
                }

                await ValidarParticipantesAsync(request.idPaciente, request.IdOdontologo, cancellationToken);

                await ValidarDisponibilidadAsync(request.IdOdontologo, inicio, fin, idTurno, cancellationToken);

                turno.IdPaciente = request.idPaciente;
                turno.IdOdontologo = request.IdOdontologo;
                turno.FechaHoraInicio = inicio;
                turno.FechaHoraFin = fin;
                turno.Observaciones = request.Observaciones?.Trim();

                _auditoria.Registrar("TURNO_ACTUALIZADO", "Turno", turno.IdTurno);

                await _context.SaveChangesAsync(cancellationToken);

                await transaccion.CommitAsync(cancellationToken);

                return await ObtenerPorIdAsync(idTurno, cancellationToken);
            }
            catch
            {
                await transaccion.RollbackAsync(CancellationToken.None);

                throw;
            }
        }

        private async Task ValidarParticipantesAsync(int idPaciente, int idOdontologo, CancellationToken cancellationToken)
        {
            if (idPaciente <= 0 || idOdontologo <= 0)
                throw new ValidacionNegocioException("Debe indicar un paciente y un odontólogo válidos.");

            bool pacienteActivo = await _context.Pacientes
                .AnyAsync(p => p.IdPaciente == idPaciente && p.Activo, cancellationToken);

            if (!pacienteActivo)
                throw new ValidacionNegocioException("El paciente no existe o está inactivo.");

            bool odontologoActivo = await _context.Odontologos
                .AnyAsync(o => o.IdOdontologo == idOdontologo && o.Activo);

            if (!odontologoActivo)
                throw new ValidacionNegocioException("El odontólogo no existe o está inactivo.");
        }

        private async Task ValidarDisponibilidadAsync(int idOdontologo, DateTime inicio, DateTime fin, int? idTurnoExcluir,
            CancellationToken cancellationToken)
        {
            bool existeConflicto = await _context.Turnos
                .AnyAsync(t => t.IdOdontologo == idOdontologo &&
                (t.Estado == "pendiente" ||
                t.Estado == "confirmado") &&
                t.FechaHoraInicio < fin &&
                t.FechaHoraFin > inicio &&
                (!idTurnoExcluir.HasValue ||
                t.IdTurno != idTurnoExcluir.Value),
                cancellationToken);

            if (existeConflicto)
                throw new ConflictoNegocioException("El odontólogo ya tiene un turno que se superpone con el horario solicitado.");
        }
        
        private async Task BloquearAgendaAsync(int idOdontologo, CancellationToken cancellationToken)
        {
            if (idOdontologo <= 0)
                throw new ValidacionNegocioException("El odontólogo indicado no es válido.");

            string recurso = $"AgendaOdontologo:{idOdontologo}";

            var parametroRecurso = new SqlParameter("@recurso", SqlDbType.NVarChar, 255)
            {
                Value = recurso
            };

            var parametroResultado = new SqlParameter("@resultado", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                """
                EXEC @resultado = sys.sp_getapplock
                    @Resource = @recurso,
                    @LockMode = 'Exclusive',
                    @LockOwner = 'Transaction',
                    @LockTimeout = 10000;
                """,
                new object[] { parametroResultado, parametroRecurso}, cancellationToken);

            int resultado = (int)parametroResultado.Value;

            if (resultado < 0)
                throw new ConflictoNegocioException("La agenda está siendo modificada. Intente nuevamente.");
        }

        private async Task BloquearTurnoAsync(int idTurno, CancellationToken cancellationToken)
        {
            if (idTurno <= 0)
                throw new ValidacionNegocioException("El identificador del turno no es válido.");

            var parametroRecurso = new SqlParameter("@recurso", SqlDbType.NVarChar, 255)
            {
                Value = $"Turno:{idTurno}"
            };

            var parametroResultado = new SqlParameter("@resultado", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                """
                EXEC @resultado = sys.sp_getapplock
                    @Resource = @recurso,
                    @LockMode = 'Exclusive',
                    @LockOwner = 'Transaction',
                    @LockTimeout = 10000;
                """,
                new object[] { parametroResultado, parametroRecurso }, cancellationToken);

            if ((int)parametroResultado.Value < 0)
                throw new ConflictoNegocioException("El turno está siendo modificado. Intente nuevamente.");
        }

        private static DateTime ObtenerFechaRequerida(DateTime? fecha, string campo)
        {
            if (!fecha.HasValue)
                throw new ValidacionNegocioException($"La fecha y hora de {campo} son obligatorias.");

            if (fecha.Value.Kind == DateTimeKind.Utc)
                throw new ValidacionNegocioException("Las fechas deben enviarse como hora local del consultorio, sin sufijo Z.");

            return DateTime.SpecifyKind(fecha.Value, DateTimeKind.Unspecified);
        }

        private static void ValidarHorario(DateTime inicio, DateTime fin)
        {
            if (fin <= inicio)
                throw new ValidacionNegocioException("La fecha de finalización debe ser posterior al inicio.");

            if (inicio <= DateTime.Now)
                throw new ValidacionNegocioException("No se pueden reservar turnos en el pasado.");

            if ((fin - inicio).TotalMinutes > DuracionMaximaMinutos)
                throw new ValidacionNegocioException($"La duración máxima de un turno es de {DuracionMaximaMinutos} minutos.");
        }

        private static bool EsEstadoValido(string estado)
        {
            return estado is
                "pendiente" or
                "confirmado" or
                "cancelado" or
                "realizado" or
                "ausente";
        }

        private static IQueryable<TurnoRespuestaDto> ProyectarRespuesta(IQueryable<Turno> consulta)
        {
            return consulta.Select(t => new TurnoRespuestaDto
            {
                IdTurno = t.IdTurno,
                IdPaciente = t.IdPaciente,
                NombrePaciente = t.Paciente.Nombre + " " + t.Paciente.Apellido,
                IdOdontologo = t.IdOdontologo,
                NombreOdontologo = t.Odontologo.Nombre + " " + t.Odontologo.Apellido,
                FechaHoraInicio = t.FechaHoraInicio,
                FechaHoraFin = t.FechaHoraFin,
                Estado = t.Estado,
                Observaciones = t.Observaciones,
                FechaCreacion = t.FechaCreacion
            });
        }
    }
}
