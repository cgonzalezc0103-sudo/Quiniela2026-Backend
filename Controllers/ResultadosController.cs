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

        [HttpPut("{idJuego}")]
        [Authorize(Roles = "Administrador Site")]
        public async Task<IActionResult> ActualizarResultado(int idJuego, [FromBody] PronosticoRequest request)
        {
            try
            {
                var affected = await _databaseService.ExecuteStoredProcedure(
                    "quiniela.SP_ActualizarResultado",
                    new
                    {
                        IdJuego = idJuego,
                        Resultado1 = request.Resultado1,
                        Resultado2 = request.Resultado2
                    });

                if (affected > 0)
                    return Ok(new { message = "Resultado actualizado exitosamente" });
                else
                    return NotFound(new { message = "Juego no encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex });
            }
        }
    }
}