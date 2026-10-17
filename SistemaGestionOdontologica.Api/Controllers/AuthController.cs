using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionOdontologica.Api.DTOs.Auth;
using SistemaGestionOdontologica.Api.Interfaces;

namespace SistemaGestionOdontologica.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            var resultado = await _authService.LoginAsync(request, cancellationToken);

            if (resultado is null)
            {
                return Unauthorized(new
                {
                    mensaje = "Usuario o contraseñña incorrectos."
                });
            }

            return Ok(resultado);
        }

        // POST: api/auth/refresh
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDto>> Refresh([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
        {
            var resultado = await _authService.RenovarTokenAsync(request, cancellationToken);

            if (resultado is null)
            {
                return Unauthorized(new
                {
                    mensaje = "Refresh token inválido o vencido."
                });
            }

            return Ok(resultado);
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
        {
            var idUsuario = ObtenerIdUsuario();

            if (idUsuario is null)
            {
                return Unauthorized();
            }

            var resultado = await _authService.CerrarSesionAsync(request.RefreshToken, idUsuario.Value, cancellationToken);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No se pudo cerrar la sesión indicada."
                });
            }

            return NoContent();
        }

        // GET: api/auth/me
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UsuarioAutenticadoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UsuarioAutenticadoDto>> Me(CancellationToken cancellationToken)
        {
            var idUsuario = ObtenerIdUsuario();

            if (idUsuario is null)
            {
                return Unauthorized();
            }

            var usuario = await _authService.ObtenerUsuarioAsync(idUsuario.Value, cancellationToken);

            if (usuario is null)
            {
                return Unauthorized();
            }

            return Ok(usuario);
        }

        private int? ObtenerIdUsuario()
        {
            var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(valor, out var idUsuario))
            {
                return idUsuario;
            }

            return null;
        }
    }
}
