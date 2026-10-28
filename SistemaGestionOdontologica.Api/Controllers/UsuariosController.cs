using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionOdontologica.Api.DTOs.Usuarios;
using SistemaGestionOdontologica.Api.Interfaces;

namespace SistemaGestionOdontologica.Api.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize(Roles = "admin")]
    [Produces("application/json")]
    public sealed class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // POST /api/usuarios
        [HttpPost]
        [ProducesResponseType(typeof(UsuarioRespuestaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UsuarioRespuestaDto>> Crear(
            [FromBody] CrearUsuarioDto request,
            CancellationToken cancellationToken)
        {
            var usuario = await _usuarioService.CrearAsync(
                request,
                cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                usuario);
        }
    }
}
