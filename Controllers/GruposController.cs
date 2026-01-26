using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace Quiniela.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GruposController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public GruposController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("estadisticas")]
        public async Task<ActionResult> GetEstadisticasGrupos([FromQuery] int? idGrupo = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var parameters = new { IdGrupo = idGrupo };
                    var result = await connection.QueryAsync<EstadisticasGrupo>(
                        "quiniela.SP_ObtenerEstadisticasGrupos",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener estadísticas: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetGrupos()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = "SELECT IdGrupo, Grupo FROM quiniela.Grupos ORDER BY IdGrupo";
                    var result = await connection.QueryAsync<GrupoDto>(query);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener grupos: {ex.Message}");
            }
        }
    }

    public class EstadisticasGrupo
    {
        public int IdGrupo { get; set; }
        public string Grupo { get; set; } = string.Empty;
        public int IdEquipo { get; set; }
        public string Equipo { get; set; } = string.Empty;
        public string Siglas { get; set; } = string.Empty;
        public int PJ { get; set; }  // Partidos Jugados
        public int PG { get; set; }  // Partidos Ganados
        public int PE { get; set; }  // Partidos Empatados
        public int PP { get; set; }  // Partidos Perdidos
        public int GF { get; set; }  // Goles a Favor
        public int GC { get; set; }  // Goles en Contra
        public int DG { get; set; }  // Diferencia de Goles
        public int Pts { get; set; } // Puntos
    }

    public class GrupoDto
    {
        public int IdGrupo { get; set; }
        public string Grupo { get; set; } = string.Empty;
    }
}