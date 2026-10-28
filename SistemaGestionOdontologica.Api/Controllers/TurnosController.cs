using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionOdontologica.Api.DTOs.Paginacion;
using SistemaGestionOdontologica.Api.DTOs.Turnos;
using SistemaGestionOdontologica.Api.Interfaces;

namespace SistemaGestionOdontologica.Api.Controllers
{
    [ApiController]
    [Route("api/turnos")]
    [Authorize(Roles = "admin,odontologo,asistente")]
    public sealed class TurnosController : ControllerBase
    {
        private readonly ITurnoService _turnoService;

        // CONSTRUCTOR.
        public TurnosController(ITurnoService turnoService)
        {
            _turnoService = turnoService;
        }

        // GET /api/turnos.
        [HttpGet]
        [ProducesResponseType(typeof(ResultadoPaginadoDto<TurnoRespuestaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ResultadoPaginadoDto<TurnoRespuestaDto>>> ObtenerPaginados(
            [FromQuery] TurnoFiltroDto filtro, CancellationToken cancellationToken)
        {
            var resultado = await _turnoService.ObtenerPaginadosAsync(filtro, cancellationToken);

            return Ok(resultado);
        }

        // GET /api/turnos/{idTurno}
        [HttpGet("{idTurno:int}")]
        [ProducesResponseType(typeof(TurnoRespuestaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TurnoRespuestaDto>> ObtenerPorId(int idTurno, CancellationToken cancellationToken)
        {
            var turno = await _turnoService.ObtenerPorIdAsync(idTurno, cancellationToken);

            if (turno is null)
            {
                return NotFound(new
                {
                    mensaje = "El turno solicitado no existe."
                });
            }

            return Ok(turno);
        }

        // POST /api/turnos
        [HttpPost]
        [Authorize(Roles = "admin,asistente")]
        [ProducesResponseType(typeof(TurnoRespuestaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<TurnoRespuestaDto>> Crear([FromBody] CrearTurnoDto request, CancellationToken cancellationToken)
        {
            var turno = await _turnoService.CrearAsync(request, cancellationToken);

            return CreatedAtAction(nameof(ObtenerPorId), new { idTurno = turno.IdTurno }, turno);
        }

        // PUT /api/turnos/{idTurno}
        [HttpPut("{idTurno:int}")]
        [Authorize(Roles = "admin,asistente")]
        [ProducesResponseType(typeof(TurnoRespuestaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<TurnoRespuestaDto>> Actualizar(int idTurno, [FromBody] ActualizarTurnoDto request,
            CancellationToken cancellationToken)
        {
            var turno = await _turnoService.ActualizarAsync(idTurno, request, cancellationToken);

            if (turno is null)
            {
                return NotFound(new
                {
                    mensaje = "El turno solicitado no existe."
                });
            }

            return Ok(turno);
        }

        // PATCH /api/turnos/{idTurno}/confirmar
        [HttpPatch("{idTurno:int}/confirmar")]
        [Authorize(Roles = "admin,asistente")]
        public async Task<ActionResult<TurnoRespuestaDto>> Confirmar(int idTurno, CancellationToken cancellationToken)
        {
            var turno = await _turnoService.ConfirmarAsync(idTurno, cancellationToken);

            if (turno is null)
            {
                return NotFound(new
                {
                    mensaje = "El turno solicitado no existe."
                });
            }

            return Ok(turno);
        }

        // PATCH /api/turnos/{idTurno}/cancelar
        [HttpPatch("{idTurno:int}/cancelar")]
        [Authorize(Roles = "admin,asistente")]
        public async Task<ActionResult<TurnoRespuestaDto>> Cancelar(int idTurno, CancellationToken cancellationToken)
        {
            var turno = await _turnoService.CancelarAsync(idTurno, cancellationToken);

            if (turno is null)
            {
                return NotFound(new
                {
                    mensaje = "El turno solicitado no existe."
                });
            }

            return Ok(turno);
        }

        // PATCH /api/turnos/{idTurno}/atender
        [HttpPatch("{idTurno:int}/atender")]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<ActionResult<TurnoRespuestaDto>> Atender(int idTurno, CancellationToken cancellationToken)
        {
            var turno = await _turnoService.AtenderAsync(idTurno, cancellationToken);

            if (turno is null)
            {
                return NotFound(new
                {
                    mensaje = "El turno solicitado no existe."
                });
            }

            return Ok(turno);
        }

        // PATCH /api/turnos/{idTurno}/ausente
        [HttpPatch("{idTurno:int}/ausente")]
        [Authorize(Roles = "admin,odontologo,asistente")]
        public async Task<ActionResult<TurnoRespuestaDto>> MarcarAusente(int idTurno, CancellationToken cancellationToken)
        {
            var turno = await _turnoService.MarcarAusenteAsync(idTurno, cancellationToken);

            if (turno is null)
            {
                return NotFound(new
                {
                    mensaje = "El turno solicitado no existe."
                });
            }

            return Ok(turno);
        }
    }
}
