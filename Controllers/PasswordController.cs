using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiniela.Data;
using System.Data;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PasswordController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;

        public PasswordController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpPost("cambiar")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Datos requeridos" });
                }

                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Cedula) ||
                    string.IsNullOrEmpty(request.PasswordAnterior) || string.IsNullOrEmpty(request.PasswordNueva))
                {
                    return BadRequest(new { message = "Todos los campos son requeridos" });
                }

                if (request.PasswordNueva.Length < 6)
                {
                    return BadRequest(new { message = "La nueva contraseña debe tener al menos 6 caracteres" });
                }

                var parameters = new DynamicParameters();
                parameters.Add("@Email", request.Email);
                parameters.Add("@Cedula", request.Cedula);
                parameters.Add("@PasswordAnterior", request.PasswordAnterior);
                parameters.Add("@PasswordNueva", request.PasswordNueva);
                parameters.Add("@Mensaje", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);
                parameters.Add("@Exito", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                await _databaseService.ExecuteStoredProcedure(
                    "quiniela.SP_CambiarPasswordConAnterior",
                    parameters
                );

                var exito = parameters.Get<bool>("@Exito");
                var mensaje = parameters.Get<string>("@Mensaje");

                if (exito)
                {
                    return Ok(new { message = mensaje });
                }
                else
                {
                    return BadRequest(new { message = mensaje });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cambiar contraseña: " + ex.Message });
            }
        }

        [HttpPost("restablecer")]
        public async Task<IActionResult> RestablecerPassword([FromBody] RestablecerPasswordRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Datos requeridos" });
                }

                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Cedula))
                {
                    return BadRequest(new { message = "Email y cédula son requeridos" });
                }

                var parameters = new DynamicParameters();
                parameters.Add("@Email", request.Email);
                parameters.Add("@Cedula", request.Cedula);
                parameters.Add("@Mensaje", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);
                parameters.Add("@Exito", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                await _databaseService.ExecuteStoredProcedure(
                    "quiniela.SP_RestablecerPassword",
                    parameters
                );

                var exito = parameters.Get<bool>("@Exito");
                var mensaje = parameters.Get<string>("@Mensaje");

                if (exito)
                {
                    return Ok(new { message = mensaje });
                }
                else
                {
                    return BadRequest(new { message = mensaje });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al restablecer contraseña: " + ex.Message });
            }
        }
    }

    public class CambiarPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string PasswordAnterior { get; set; } = string.Empty;
        public string PasswordNueva { get; set; } = string.Empty;
    }

    public class RestablecerPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
    }
}