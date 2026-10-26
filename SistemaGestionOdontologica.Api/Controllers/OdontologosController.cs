using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionOdontologica.Api.DTOs.Odontologos;
using SistemaGestionOdontologica.Api.DTOs.Paginacion;
using SistemaGestionOdontologica.Api.Interfaces;
using SistemaGestionOdontologica.Api.Security;

namespace SistemaGestionOdontologica.Api.Controllers
{
    [ApiController]
    [Route("api/odontologos")]
    [Authorize(Roles = "admin,odontologo,asistente")]
    public sealed class OdontologosController : ControllerBase
    {
        private readonly IOdontologoService _odontologoService;

        // CONSTRUCTOR
        public OdontologosController(IOdontologoService odontologoService)
        {
            _odontologoService = odontologoService;
        }

        // GET /api/odontologos.
        [HttpGet]
        [ProducesResponseType(typeof(ResultadoPaginadoDto<OdontologoRespuestaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ResultadoPaginadoDto<OdontologoRespuestaDto>>> ObtenerPaginados(
            [FromQuery] OdontologoFiltroDto filtro, CancellationToken cancellationToken)
        {
            var resultado = await _odontologoService.ObtenerPaginadosAsync(filtro, cancellationToken);

            return Ok(resultado);
        }

        // GET /api/odontologos/{idOdontologo}
        [HttpGet("{idOdontologo:int}")]
        [ProducesResponseType(typeof(OdontologoRespuestaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OdontologoRespuestaDto>> ObtenerPorId(int idOdontologo, CancellationToken cancellationToken)
        {
            var odontologo = await _odontologoService.ObtenerPorIdAsync(idOdontologo, cancellationToken);

            if (odontologo is null)
            {
                return NotFound(new
                {
                    mensaje = "El odontólogo solicitado no existe."
                });
            }

            return Ok(odontologo);
        }

        // POST /api/odontologos.
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(OdontologoRespuestaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<OdontologoRespuestaDto>> Crear([FromBody] CrearOdontologoDto request, CancellationToken cancellationToken)
        {
            var odontologo = await _odontologoService.CrearAsync(request, cancellationToken);

            return CreatedAtAction(nameof(ObtenerPorId), new { idOdontologo = odontologo.IdOdontologo}, odontologo);
        }

        // PUT /api/odontologos/{idOdontologo}
        [HttpPut("{idOdontologo:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(OdontologoRespuestaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<OdontologoRespuestaDto>> Actualizar(int idOdontologo, [FromBody] ActualizarOdontologoDto requet,
            CancellationToken cancellationToken)
        {
            var odontologo = await _odontologoService.ActualizarAsync(idOdontologo, requet,cancellationToken);

            if (odontologo is null)
            {
                return NotFound(new
                {
                    mensaje = "El odontólogo solicitado no existe."
                });
            }

            return Ok(odontologo);
        }

        // PATCH /api/odontologos/{idOdontologo}/desactivar.
        [HttpPatch("{idOdontologo:int}/desactivar")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Desactivar(int idOdontologo, CancellationToken cancellationToken)
        {
            bool encontrado = await _odontologoService.DesactivarAsync(idOdontologo, cancellationToken);

            if (!encontrado)
            {
                return NotFound(new
                {
                    mensaje = "El odontólogo solicitado no existe."
                });
            }

            return NoContent();
        }

        // PATCH /api/odontologos/{idOdontologo}/activar.
        [HttpPatch("{idOdontologo:int}/activar")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activar(int idOdontologo, CancellationToken cancellationToken)
        {
            bool encontrado = await _odontologoService.ActivarAsync(idOdontologo, cancellationToken);

            if (!encontrado)
            {
                return NotFound(new
                {
                    mensaje = "El odontólogo solicitado no existe."
                });
            }

            return NoContent();
        }
    }
}
