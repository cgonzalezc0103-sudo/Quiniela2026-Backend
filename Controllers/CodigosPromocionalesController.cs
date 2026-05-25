using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Data;
using System.Data;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador Site")]
    public class CodigosPromocionalesController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;

        public CodigosPromocionalesController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet("rolls")]
        public async Task<IActionResult> GetRolls()
        {
            try
            {
                var rolls = await _databaseService.ExecuteStoredProcedure<RollPromocional>(
                    "quiniela.SP_ObtenerRollsPromocionales");
                return Ok(rolls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("rolls/{idRoll}/codigos")]
        public async Task<IActionResult> GetCodigosPorRoll(int idRoll)
        {
            try
            {
                var codigos = await _databaseService.ExecuteStoredProcedure<CodigoDetalle>(
                    "quiniela.SP_ObtenerCodigosPorRoll",
                    new { IdRoll = idRoll });
                return Ok(codigos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("rolls")]
        public async Task<IActionResult> CrearRoll([FromBody] CrearRollRequest request)
        {
            try
            {
                if (request == null || request.IdEmpresa <= 0 || request.Cantidad <= 0)
                    return BadRequest(new { message = "Datos inválidos" });

                var roll = await _databaseService.ExecuteStoredProcedureSingle<RollPromocional>(
                    "quiniela.SP_CrearRollPromocional",
                    new { IdEmpresa = request.IdEmpresa, Cantidad = request.Cantidad });

                if (roll == null)
                    return StatusCode(500, new { message = "Error al crear el roll" });

                return Ok(roll);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("empresas")]
        public async Task<IActionResult> GetEmpresas()
        {
            try
            {
                var empresas = await _databaseService.ExecuteStoredProcedure<EmpresaSimple>(
                    "quiniela.SP_ObtenerEmpresasActivas");
                return Ok(empresas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class RollPromocional
    {
        public int IdRoll { get; set; }
        public string NombreRoll { get; set; } = string.Empty;
        public string Empresa { get; set; } = string.Empty;
        public int CantidadTotal { get; set; }
        public int Utilizados { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool IndActivo { get; set; }

    }

    public class CodigoDetalle
    {
        public int IdDetalle { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public int Estado { get; set; } // 0 disponible, 1 usado
        public string? CedulaUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public DateTime? FechaUso { get; set; }
        public string Factura { get; set; } = string.Empty;
    }

    public class CrearRollRequest
    {
        public int IdEmpresa { get; set; }
        public int Cantidad { get; set; }
    }

    public class EmpresaSimple
    {
        public int IdEmpresa { get; set; }
        public string Empresa { get; set; } = string.Empty;
    }
}