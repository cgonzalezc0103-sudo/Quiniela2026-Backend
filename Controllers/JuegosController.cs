using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Data;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JuegosController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;

        public JuegosController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Administrador Site")]
        public async Task<IActionResult> GetJuegosAdmin()
        {
            try
            {
                var juegos = await _databaseService.ExecuteStoredProcedure<JuegoAdmin>(
                    "quiniela.SP_ObtenerJuegosAdmin",
                    new { }
                );
                return Ok(juegos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("resultado/{idJuego}")]
        [Authorize(Roles = "Administrador Site")]
        public async Task<IActionResult> ActualizarResultado(int idJuego, [FromBody] ActualizarResultadoRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Datos requeridos" });
                }

                if (request.Resultado1 < 0 || request.Resultado2 < 0)
                {
                    return BadRequest(new { message = "Los resultados no pueden ser negativos" });
                }

                var juego = await _databaseService.ExecuteStoredProcedureSingle<JuegoResultado>(
                    "quiniela.SP_ActualizarResultadoAdmin",
                    new { IdJuego = idJuego, Resultado1 = request.Resultado1, Resultado2 = request.Resultado2 }
                );

                return Ok(new
                {
                    message = "Resultado actualizado exitosamente",
                    juego = juego
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class JuegoAdmin
    {
        public int IdJuego { get; set; }
        public int IdRonda { get; set; }
        public string Ronda { get; set; } = string.Empty;
        public string Equipo1 { get; set; } = string.Empty;
        public string Siglas1 { get; set; } = string.Empty;
        public string Equipo2 { get; set; } = string.Empty;
        public string Siglas2 { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int Resultado1 { get; set; }
        public int Resultado2 { get; set; }
        public bool IndFinalizado { get; set; }
        public bool PuedeIngresarResultado { get; set; }
    }

    public class ActualizarResultadoRequest
    {
        public int Resultado1 { get; set; }
        public int Resultado2 { get; set; }
    }

    public class JuegoResultado
    {
        public int IdJuego { get; set; }
        public int Resultado1 { get; set; }
        public int Resultado2 { get; set; }
        public bool IndFinalizado { get; set; }
    }
}