using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Models;
using Quiniela.Data;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultadosController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;

        public ResultadosController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetResultados(
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta,
            [FromQuery] int? idEquipo,
            [FromQuery] int? idRonda)
        {
            try
            {
                var resultados = await _databaseService.ExecuteStoredProcedure<Resultado>(
                    "quiniela.SP_ObtenerResultados",
                    new
                    {
                        FechaDesde = fechaDesde,
                        FechaHasta = fechaHasta,
                        IdEquipo = idEquipo,
                        IdRonda = idRonda
                    });

                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex });
            }
        }

        [HttpGet("{idJuego}/puntos")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPuntosPorJuego(int idJuego)
        {
            try
            {
                var puntos = await _databaseService.ExecuteStoredProcedure<PuntoJuego>(
                    "quiniela.SP_ObtenerPuntosPorJuego",
                    new { IdJuego = idJuego }
                );

                return Ok(puntos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{idJuego}")]
        [Authorize(Roles = "Administrador Site")]
        public async Task<IActionResult> EditarResultado(int idJuego, [FromBody] EditarResultadoRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Datos requeridos" });

                if (request.Resultado1 < 0 || request.Resultado2 < 0)
                    return BadRequest(new { message = "Los resultados no pueden ser negativos" });

                var juego = await _databaseService.ExecuteStoredProcedureSingle<JuegoResultado>(
                    "quiniela.SP_EditarResultadoJuego",
                    new { IdJuego = idJuego, Resultado1 = request.Resultado1, Resultado2 = request.Resultado2 });

                if (juego == null)
                    return NotFound(new { message = "Juego no encontrado" });

                return Ok(new { message = "Resultado actualizado y puntos recalculados exitosamente", juego });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Clases auxiliares (pueden estar dentro del mismo archivo)
        public class EditarResultadoRequest
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

        public class PuntoJuego
        {
            public int IdUsuario { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string Nombres { get; set; } = string.Empty;
            public int PuntosObtenidos { get; set; }
            public string TipoAcierto { get; set; } = string.Empty;
            public int Pronostico1 { get; set; }
            public int Pronostico2 { get; set; }
            public int Resultado1 { get; set; }
            public int Resultado2 { get; set; }
        }
    }
}