using System.Security.Claims;
using System.Data;
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
        private IHttpContextAccessor _httpContextAccessor;

        // CONSTRUCTOR.
        public TurnoService(ApplicationDbContext context, IAuditoriaService auditoria, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _auditoria = auditoria;
            _httpContextAccessor = httpContextAccessor;
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

            int? idOdontologoAutenticado = await ObtenerOdontologoAutenticadoAsync(cancellationToken);

            if (idOdontologoAutenticado.HasValue)
            {
                consulta = consulta.Where(t => t.IdOdontologo == idOdontologoAutenticado.Value);
            }

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
            int? idOdontologoAutenticado =
                await ObtenerOdontologoAutenticadoAsync(cancellationToken);

            IQueryable<Turno> consulta = _context.Turnos
                .AsNoTracking()
                .Where(t => t.IdTurno == idTurno);

            if (idOdontologoAutenticado.HasValue)
            {
                consulta = consulta.Where(
                    t => t.IdOdontologo == idOdontologoAutenticado.Value);
            }

            return await ProyectarRespuesta(consulta)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TurnoRespuestaDto> CrearAsync(CrearTurnoDto request, CancellationToken cancellationToken = default)
        {
            DateTime inicio = ObtenerFechaRequerida(request.FechaHoraInicio, "inicio");

            DateTime fin = ObtenerFechaRequerida(request.FechaHoraFin, "finalizacion");

            ValidarHorario(inicio, fin);

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            try
            {
                await BloquearEscrituraTurnosAsync(cancellationToken);

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
                await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            try
            {
                // Se bloquea primero el turno que se va a modificar.
                await BloquearEscrituraTurnosAsync(cancellationToken);

                var turno = await _context.Turnos
                    .FirstOrDefaultAsync(t => t.IdTurno == idTurno, cancellationToken);

                if (turno is null)
                {
                    await transaccion.CommitAsync(cancellationToken);
                    return null;
                }

                if (turno.Estado != "pendiente" && turno.Estado != "confirmado")
                    throw new ConflictoNegocioException("El turno no puede reprogramarse en su estado actual.");

                await ValidarParticipantesAsync(request.IdPaciente, request.IdOdontologo, cancellationToken);

                await ValidarDisponibilidadAsync(request.IdOdontologo, inicio, fin, idTurno, cancellationToken);

                turno.IdPaciente = request.IdPaciente;
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

        public Task<TurnoRespuestaDto?> ConfirmarAsync(int idTurno, CancellationToken cancellationToken = default)
        {
            return CambiarEstadoAsync(
                idTurno,
                "confirmado",
                "TURNO_CONFIRMADO",
                cancellationToken);
        }

        public Task<TurnoRespuestaDto?> CancelarAsync(int idTurno, CancellationToken cancellationToken = default)
        {
            return CambiarEstadoAsync(
                idTurno,
                "cancelado",
                "TURNO_CANCELADO",
                cancellationToken);
        }

        public Task<TurnoRespuestaDto?> AtenderAsync(int idTurno, CancellationToken cancellationToken = default)
        {
            return CambiarEstadoAsync(
                idTurno,
                "realizado",
                "TURNO_REALIZADO",
                cancellationToken);
        }

        public Task<TurnoRespuestaDto?> MarcarAusenteAsync(int idTurno, CancellationToken cancellationToken = default)
        {
            return CambiarEstadoAsync(
                idTurno,
                "ausente",
                "TURNO_AUSENTE",
                cancellationToken);
        }

        private async Task<TurnoRespuestaDto?> CambiarEstadoAsync(int idTurno, string nuevoEstado, string accionAuditoria,
            CancellationToken cancellationToken)
        {
            await using var transaccion = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            try
            {
                // Impide modificacion simultáneas del mismo turno.
                await BloquearEscrituraTurnosAsync(cancellationToken);

                var turno = await _context.Turnos
                    .FirstOrDefaultAsync(t => t.IdTurno == idTurno, cancellationToken);
                   
                if (turno is null)
                {
                    await transaccion.CommitAsync(cancellationToken);
                    return null;
                }

                int? idOdontologoAutenticado =
                    await ObtenerOdontologoAutenticadoAsync(cancellationToken);

                if (idOdontologoAutenticado.HasValue &&
                    turno.IdOdontologo != idOdontologoAutenticado.Value)
                        throw new AccesoProhibidoException("No tiene permisos para modificar turnos de otro odontólogo.");

                ValidarTransicionEstado(turno, nuevoEstado);

                turno.Estado = nuevoEstado;

                _auditoria.Registrar(accionAuditoria, "Turno", turno.IdTurno);

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

        private static void ValidarTransicionEstado(Turno turno, string nuevoEstado)
        {
            string estadoActual = turno.Estado.Trim().ToLowerInvariant();

            nuevoEstado = nuevoEstado.Trim().ToLowerInvariant();

            if (estadoActual is "cancelado" or "realizado" or "ausente")
                throw new ConflictoNegocioException("El turno se encuetnra en un estado final y no puede modificarse.");

            if (estadoActual == nuevoEstado)
                throw new ConflictoNegocioException($"El turno ya se encuentra en estado '{nuevoEstado}'.");

            bool transicionPermitida =
                (estadoActual == "pendiente" &&
                 (nuevoEstado == "confirmado" ||
                  nuevoEstado == "cancelado" ||
                  nuevoEstado == "realizado" ||
                  nuevoEstado == "ausente"))
                ||
                (estadoActual == "confirmado" &&
                 (nuevoEstado == "cancelado" ||
                  nuevoEstado == "realizado" ||
                  nuevoEstado == "ausente"));

            if (!transicionPermitida)
                throw new ConflictoNegocioException($"No se permite cambiar un turno de '{estadoActual}' a '{nuevoEstado}'.");

            if (nuevoEstado is "realizado" or "ausente")
            {
                if (turno.FechaHoraInicio > DateTime.Now)
                    throw new ConflictoNegocioException("No se puede marcar un turno como realizado o ausente de su horario de inicio.");
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

        private async Task BloquearEscrituraTurnosAsync(CancellationToken cancellationToken)
        {
            var parametroRecurso = new SqlParameter(
                "@recurso",
                SqlDbType.NVarChar, 255)
            {
                Value = "Turnos:Escrituras"
            };

            var parametroResultado = new SqlParameter(
                "@resultado",
                SqlDbType.Int)
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
                new object[]
                {
                    parametroResultado,
                    parametroRecurso
                },
                cancellationToken);

                int resultado = (int)parametroResultado.Value;

            if (resultado < 0)
            {
                throw new ConflictoNegocioException(
                    "Otra operación está modificando la agenda. Intente nuevamente.");
            }
        }

        private async Task<int?> ObtenerOdontologoAutenticadoAsync(CancellationToken cancellationToken)
        {
            var principal = _httpContextAccessor.HttpContext?.User;

            if (principal?.Identity?.IsAuthenticated != true)
                throw new AccesoProhibidoException("No se pudo verificar la identidad del usuario.");

            string? identificador = principal.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(identificador, out int idUsuario) ||
                idUsuario <= 0)
            {
                throw new AccesoProhibidoException("No se pudo identificar al usuario autenticado.");
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

            if (usuario is null)
                throw new AccesoProhibidoException("El usuario no existe o se encuentra inactivo.");

            if (usuario.Rol != "odontologo")
                return null;

            if (!usuario.IdOdontologo.HasValue)
                throw new AccesoProhibidoException("El usuario odontólogo no tiene un profesional asociado.");

            return usuario.IdOdontologo.Value;
        }
    }
}
