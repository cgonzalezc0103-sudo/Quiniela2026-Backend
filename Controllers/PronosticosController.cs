using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Quiniela.Models;
using Quiniela.Data;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PronosticosController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;

        public PronosticosController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet("activos")]
        public async Task<IActionResult> GetJuegosActivos()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Usuario no válido" });
                }

                var juegos = await _databaseService.ExecuteStoredProcedure<Juego>(
                    "quiniela.SP_ObtenerJuegosActivos",
                    new { IdUsuario = userId });

                return Ok(juegos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex });
            }
        }

        [HttpPost("guardar")]
        public async Task<IActionResult> GuardarPronostico([FromBody] PronosticoRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Usuario no válido" });
                }

                var idPronostico = await _databaseService.ExecuteStoredProcedureSingle<int>(
                    "quiniela.SP_GuardarPronostico",
                    new
                    {
                        IdUsuario = userId,
                        IdJuego = request.IdJuego,
                        Resultado1 = request.Resultado1,
                        Resultado2 = request.Resultado2
                    });

                return Ok(new { message = "Pronóstico guardado exitosamente", idPronostico });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex });
            }
        }
    }
}