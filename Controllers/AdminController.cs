using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Data;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador Site")]
    public class AdminController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;

        public AdminController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpPost("activar-usuario/{idUsuario}")]
        public async Task<IActionResult> ActivarUsuario(int idUsuario)
        {
            try
            {
                var affected = await _databaseService.ExecuteStoredProcedure(
                    "quiniela.SP_ActivarUsuario",
                    new { IdUsuario = idUsuario });

                if (affected > 0)
                    return Ok(new { message = "Usuario activado exitosamente" });
                else
                    return NotFound(new { message = "Usuario no encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex });
            }
        }
    }
}