using Microsoft.AspNetCore.Mvc;
using Quiniela.Data;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquiposController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;

        public EquiposController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEquipos()
        {
            try
            {
                // Usar ExecuteStoredProcedure<T> que devuelve IEnumerable<T>
                var equipos = await _databaseService.ExecuteStoredProcedure<dynamic>(
                    "quiniela.SP_ObtenerEquipos",
                    new { }
                );

                return Ok(equipos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error al obtener equipos",
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEquipo(int id)
        {
            try
            {
                var equipo = await _databaseService.ExecuteStoredProcedureSingle<dynamic>(
                    "quiniela.SP_ObtenerEquipoPorId",
                    new { IdEquipo = id }
                );

                if (equipo == null)
                {
                    return NotFound(new { message = "Equipo no encontrado" });
                }

                return Ok(equipo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = $"Error al obtener equipo con ID: {id}",
                    message = ex.Message
                });
            }
        }
    }
}