using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Data;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador Site")]
    public class EmpresasController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;

        public EmpresasController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmpresas()
        {
            try
            {
                var empresas = await _databaseService.ExecuteStoredProcedure<EmpresaAdmin>(
                    "quiniela.SP_ObtenerEmpresas",
                    new { }
                );

                return Ok(empresas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarEmpresas([FromQuery] string termino)
        {
            try
            {
                if (string.IsNullOrEmpty(termino) || termino.Length < 2)
                {
                    return BadRequest(new { message = "El término de búsqueda debe tener al menos 2 caracteres" });
                }

                var empresas = await _databaseService.ExecuteStoredProcedure<EmpresaAdmin>(
                    "quiniela.SP_BuscarEmpresas",
                    new { TerminoBusqueda = termino }
                );

                return Ok(empresas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmpresa(int id)
        {
            try
            {
                var empresa = await _databaseService.ExecuteStoredProcedureSingle<EmpresaAdmin>(
                    "quiniela.SP_ObtenerEmpresaPorId",
                    new { IdEmpresa = id }
                );

                if (empresa == null)
                {
                    return NotFound(new { message = "Empresa no encontrada" });
                }

                return Ok(empresa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearEmpresa([FromBody] CrearEmpresaRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Datos requeridos" });
                }

                if (string.IsNullOrEmpty(request.Empresa))
                {
                    return BadRequest(new { message = "El nombre de la empresa es requerido" });
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { message = "El email es requerido" });
                }

                var empresa = await _databaseService.ExecuteStoredProcedureSingle<EmpresaAdmin>(
                    "quiniela.SP_CrearEmpresa",
                    new
                    {
                        Empresa = request.Empresa,
                        Responsable = request.Responsable ?? string.Empty,
                        Telefono = request.Telefono ?? string.Empty,
                        Email = request.Email
                    }
                );

                return Ok(empresa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarEmpresa(int id, [FromBody] ActualizarEmpresaRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Datos requeridos" });
                }

                if (string.IsNullOrEmpty(request.Empresa))
                {
                    return BadRequest(new { message = "El nombre de la empresa es requerido" });
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { message = "El email es requerido" });
                }

                var empresa = await _databaseService.ExecuteStoredProcedureSingle<EmpresaAdmin>(
                    "quiniela.SP_ActualizarEmpresa",
                    new
                    {
                        IdEmpresa = id,
                        Empresa = request.Empresa,
                        Responsable = request.Responsable ?? string.Empty,
                        Telefono = request.Telefono ?? string.Empty,
                        Email = request.Email
                    }
                );

                if (empresa == null)
                {
                    return NotFound(new { message = "Empresa no encontrada" });
                }

                return Ok(empresa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/estado")]
        public async Task<IActionResult> CambiarEstadoEmpresa(int id, [FromBody] CambiarEstadoEmpresaRequest request)
        {
            try
            {
                var empresa = await _databaseService.ExecuteStoredProcedureSingle<EmpresaAdmin>(
                    "quiniela.SP_CambiarEstadoEmpresa",
                    new { IdEmpresa = id, Activo = request.Activo }
                );

                if (empresa == null)
                {
                    return NotFound(new { message = "Empresa no encontrada" });
                }

                var action = request.Activo ? "activada" : "desactivada";
                return Ok(new
                {
                    message = $"Empresa {action} exitosamente",
                    empresa = empresa
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class EmpresaAdmin
    {
        public int IdEmpresa { get; set; }
        public string Empresa { get; set; } = string.Empty;
        public string? Responsable { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool IndActivo { get; set; }
    }

    public class CrearEmpresaRequest
    {
        public string Empresa { get; set; } = string.Empty;
        public string? Responsable { get; set; }
        public string? Telefono { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    public class ActualizarEmpresaRequest
    {
        public string Empresa { get; set; } = string.Empty;
        public string? Responsable { get; set; }
        public string? Telefono { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    public class CambiarEstadoEmpresaRequest
    {
        public bool Activo { get; set; }
    }
}