using Microsoft.AspNetCore.Mvc;

namespace SistemaGestionOdontologica.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Estado = "Activo",
                Aplicacion = "Sistema de Gestión Odontológica",
                Version = "1.0",
                FechaServidor = DateTime.UtcNow
            });
        } 
    }
}