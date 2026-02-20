using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Data;
using Quiniela.Models;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador Site")]
    public class UsuariosController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;

        public UsuariosController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet("pendientes")]
        public async Task<IActionResult> GetUsuariosPendientes()
        {
            try
            {
                var usuarios = await _databaseService.ExecuteStoredProcedure<UsuarioAdmin>("quiniela.SP_ObtenerUsuariosPendientes");
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener usuarios pendientes" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTodosUsuarios()
        {
            try
            {
                var usuarios = await _databaseService.ExecuteStoredProcedure<UsuarioAdmin>("quiniela.SP_ObtenerTodosUsuarios");
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener usuarios" });
            }
        }

        [HttpPut("{idUsuario}/estado")]
        public async Task<IActionResult> CambiarEstadoUsuario(int idUsuario, [FromBody] CambiarEstadoRequest request)
        {
            try
            {
                // Usar ExecuteStoredProcedureSingle para obtener el usuario actualizado
                var usuario = await _databaseService.ExecuteStoredProcedureSingle<UsuarioAdmin>(
                    "quiniela.SP_CambiarEstadoUsuario",
                    new { IdUsuario = idUsuario, Activo = request.Activo });

                if (usuario != null)
                {
                    var action = request.Activo ? "activado" : "desactivado";
                    return Ok(new
                    {
                        message = $"Usuario {action} exitosamente",
                        usuario = usuario
                    });
                }
                else
                    return NotFound(new { message = "Usuario no encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cambiar estado del usuario" });
            }
        }

        [HttpPost("{idUsuario}/activar")]
        public async Task<IActionResult> ActivarUsuario(int idUsuario)
        {
            try
            {
                // Usar ExecuteStoredProcedureSingle para obtener el usuario actualizado
                var usuario = await _databaseService.ExecuteStoredProcedureSingle<UsuarioAdmin>(
                    "quiniela.SP_ActivarUsuario",
                    new { IdUsuario = idUsuario });

                if (usuario != null)
                    return Ok(new
                    {
                        message = "Usuario activado exitosamente",
                        usuario = usuario
                    });
                else
                    return NotFound(new { message = "Usuario no encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al activar usuario" });
            }
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarUsuarios([FromQuery] string termino, [FromQuery] bool soloPendientes = false)
        {
            try
            {
                var usuarios = await _databaseService.ExecuteStoredProcedure<UsuarioAdmin>(
                    "quiniela.SP_BuscarUsuarios",
                    new { TerminoBusqueda = termino, MostrarSoloPendientes = soloPendientes }
                );
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al buscar usuarios" });
            }
        }
    }

    public class CambiarEstadoRequest
    {
        public bool Activo { get; set; }
    }
}