// Controllers/AuthController.cs - Actualizado
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
                if (request?.Email == null || request.Password == null)
                {
                    return BadRequest(new { message = "Email y contraseña son requeridos" });
                }

                var usuario = await _databaseService.ExecuteStoredProcedureSingle<Usuario>(
                    "quiniela.SP_LoginUsuario",
                    new { request.Email, request.Password });

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
                        usuario.Empresa
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex });
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

                var idUsuario = await _databaseService.ExecuteStoredProcedureSingle<int>(
                    "quiniela.SP_RegistrarUsuario",
                    request);

                return Ok(new { message = "Usuario registrado exitosamente. Pendiente de activación por administrador.", idUsuario });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex });
            }
        }
    }
}