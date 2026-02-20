using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Data;

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

        [HttpGet]
        public async Task<IActionResult> GetCodigos()
        {
            try
            {
                var codigos = await _databaseService.ExecuteStoredProcedure<CodigoPromocionalAdmin>(
                    "quiniela.SP_ObtenerCodigosPromocionales",
                    new { }
                );

                return Ok(codigos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}/usuarios")]
        public async Task<IActionResult> GetUsuariosPorCodigo(int id)
        {
            try
            {
                var usuarios = await _databaseService.ExecuteStoredProcedure<UsuarioCodigoPromocional>(
                    "quiniela.SP_ObtenerUsuariosPorCodigo",
                    new { IdCodigoPromocional = id }
                );

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearCodigo([FromBody] CrearCodigoRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Datos requeridos" });
                }

                if (request.IdEmpresa <= 0)
                {
                    return BadRequest(new { message = "Debe seleccionar una empresa" });
                }

                if (request.Cantidad <= 0)
                {
                    return BadRequest(new { message = "La cantidad debe ser mayor a 0" });
                }

                var codigo = await _databaseService.ExecuteStoredProcedureSingle<CodigoPromocionalAdmin>(
                    "quiniela.SP_CrearCodigoPromocional",
                    new { IdEmpresa = request.IdEmpresa, Cantidad = request.Cantidad }
                );

                if (codigo == null)
                {
                    return StatusCode(500, new { message = "Error al generar el código" });
                }

                return Ok(codigo);
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
                    "quiniela.SP_ObtenerEmpresasActivas",
                    new { }
                );

                return Ok(empresas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class CodigoPromocionalAdmin
    {
        public int IdCodigoPromocional { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Empresa { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public int CantidadRestante { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool IndActivo { get; set; }
        public int UsuariosRegistrados { get; set; }
    }

    public class UsuarioCodigoPromocional
    {
        public int IdUsuario { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public bool IndActivo { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string? EquipoFavorito { get; set; }
    }

    public class CrearCodigoRequest
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