using Microsoft.AspNetCore.Mvc;
using Quiniela.Models;
using Quiniela.Services;
using Quiniela.Data;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly IJwtService _jwtService;

        public AuthController(IDatabaseService databaseService, IJwtService jwtService)
        {
            _databaseService = databaseService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "Email/Usuario y contraseña son requeridos" });
                }

                var usuario = await _databaseService.ExecuteStoredProcedureSingle<Usuario>(
                    "quiniela.SP_LoginUsuario",
                    new { EmailOrUserName = request.Email, Password = request.Password });

                if (usuario == null)
                    return Unauthorized(new { message = "Credenciales inválidas" });

                if (!usuario.IndActivo)
                    return Unauthorized(new { message = "Usuario pendiente de activación" });

                var token = _jwtService.GenerateToken(usuario);

                return Ok(new
                {
                    token,
                    usuario = new
                    {
                        usuario.IdUsuario,
                        usuario.Nombres,
                        usuario.Email,
                        usuario.Rol,
                        usuario.Empresa,
                        usuario.IdEquipo,
                        usuario.UserName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Datos de registro son requeridos" });
                }

                // Validar que todos los campos estén presentes
                if (string.IsNullOrEmpty(request.Cedula))
                {
                    return BadRequest(new { message = "La cédula es requerida" });
                }

                if (string.IsNullOrEmpty(request.UserName))
                {
                    return BadRequest(new { message = "El nombre de usuario es requerido" });
                }

                if (string.IsNullOrEmpty(request.Nombres))
                {
                    return BadRequest(new { message = "El nombre completo es requerido" });
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { message = "El email es requerido" });
                }

                if (string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "La contraseña es requerida" });
                }

                if (string.IsNullOrEmpty(request.CodigoPromocional))
                {
                    return BadRequest(new { message = "El código promocional es obligatorio" });
                }

                if (!request.IdEquipo.HasValue || request.IdEquipo.Value <= 0)
                {
                    return BadRequest(new { message = "Debe seleccionar un equipo favorito" });
                }

                var result = await _databaseService.ExecuteStoredProcedureSingle<dynamic>(
                    "quiniela.SP_RegistrarUsuario",
                    new
                    {
                        UserName = request.UserName,
                        Nombres = request.Nombres,
                        Email = request.Email,
                        Password = request.Password,
                        Cedula = request.Cedula,
                        CodigoPromocional = request.CodigoPromocional,
                        IdEquipo = request.IdEquipo.Value,
                        Factura = request.Factura,
                    });

                if (result == null)
                {
                    return StatusCode(500, new { message = "Error al procesar el registro" });
                }

                dynamic dynamicResult = result;
                var idUsuario = (int)dynamicResult.IdUsuario;
                var indActivo = (bool)dynamicResult.IndActivo;

                var message = "¡Registro exitoso! Tu cuenta ha sido activada automáticamente.";

                return Ok(new
                {
                    message = message,
                    idUsuario,
                    indActivo
                });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;

                if (errorMessage.Contains("El nombre de usuario ya está registrado"))
                    return BadRequest(new { message = "El nombre de usuario ya está registrado" });

                if (errorMessage.Contains("La cédula ya está registrada"))
                    return BadRequest(new { message = "La cédula ya está registrada" });

                if (errorMessage.Contains("El email ya está registrado"))
                    return BadRequest(new { message = "El email ya está registrado" });

                if (errorMessage.Contains("Código promocional"))
                    return BadRequest(new { message = errorMessage });

                if (errorMessage.Contains("equipo") || errorMessage.Contains("Equipo"))
                    return BadRequest(new { message = "El equipo seleccionado no es válido" });

                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}