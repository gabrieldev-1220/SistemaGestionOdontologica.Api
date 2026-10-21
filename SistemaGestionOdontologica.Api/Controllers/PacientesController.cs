using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionOdontologica.Api.DTOs.Pacientes;
using SistemaGestionOdontologica.Api.DTOs.Paginacion;
using SistemaGestionOdontologica.Api.Interfaces;
using SistemaGestionOdontologica.Api.Security;

namespace SistemaGestionOdontologica.Api.Controllers
{
    [ApiController]
    [Route("api/pacientes")]
    [Authorize(Roles =
        Roles.Admin + "," +
        Roles.Odontologo + "," +
        Roles.Asistente)]
    public sealed class PacientesController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;

        public PacientesController(IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        // GET /api/pacientes
        [HttpGet]
        [ProducesResponseType(typeof(ResultadoPaginadoDto<PacienteRespuestaDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResultadoPaginadoDto<PacienteRespuestaDto>>> ObtenerTodos(
            [FromQuery] PacienteFiltroDto filtro, CancellationToken cancellationToken)
        {
            var resultado = await _pacienteService.ObtenerPaginadosAsync(filtro, cancellationToken);

            return Ok(resultado);
        }

        // GET /api/paciente/5
        [HttpGet("{idPaciente:int}")]
        [ProducesResponseType(typeof(PacienteRespuestaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PacienteRespuestaDto>> ObtenerPorId(int idPaciente, CancellationToken cancellationToken)
        {
            var paciente = await _pacienteService.ObtenerPorIdAsync(idPaciente, cancellationToken);

            if (paciente is null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Paciente no encontrado",
                    Detail = "No existe un paciente con ese identificador."
                });
            }
            
            return Ok(paciente);
        }

        // POST /api/pacientes
        [HttpPost]
        [Authorize(Roles = Roles.Admin + "," + Roles.Asistente)]
        [ProducesResponseType(typeof(PacienteRespuestaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PacienteRespuestaDto>> Crear([FromBody] CrearPacienteDto request, CancellationToken cancellationToken)
        {
            var paciente = await _pacienteService.CrearAsync(request, cancellationToken);

            return CreatedAtAction(nameof(ObtenerPorId), new {idPaciente = paciente.IdPaciente}, paciente);
        }

        // PUT /api/pacientes/5
        [HttpPut("{idPaciente:int}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Asistente)]
        [ProducesResponseType(typeof(PacienteRespuestaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PacienteRespuestaDto>> Actualizar(
            int idPaciente, [FromBody] ActualizarPacienteDto request, CancellationToken cancellationToken)
        {
            var paciente = await _pacienteService.ActualizarAsync(idPaciente, request, cancellationToken);

            if (paciente is null)
                return NotFound();

            return Ok(paciente);
        }

        // Patch /api/pacientes/5/desactivar
        [HttpPatch("{idPaciente:int}/desactivar")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Desactivar(int idPaciente, CancellationToken cancellationToken)
        {
            bool resultado = await _pacienteService.DesactivarAsync(idPaciente, cancellationToken);

            if (!resultado)
                return NotFound();

            return NoContent();
        }

        // PATCH /api/pacientes/5/activar
        [HttpPatch("{idPaciente:int}/activar")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activar(int idPaciente, CancellationToken cancellationToken)
        {
            bool resultado = await _pacienteService.ActivarAsync(idPaciente, cancellationToken);

            if (!resultado)
                return NoContent();

            return NoContent();
        }
    }
}
