using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quiniela.Data;
using Quiniela.Models;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador Site")]
    public class ConfiguracionController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly IWebHostEnvironment _environment;

        public ConfiguracionController(IDatabaseService databaseService, IWebHostEnvironment environment)
        {
            _databaseService = databaseService;
            _environment = environment;
        }

        [HttpGet("seccion/{seccion}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySeccion(string seccion)
        {
            var configs = await _databaseService.ExecuteStoredProcedure<ConfiguracionVisual>(
                "quiniela.SP_ObtenerConfiguracionPorSeccion",
                new { Seccion = seccion });
            return Ok(configs);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImagen(IFormFile file, string seccion)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No se recibió archivo" });

            var extension = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".avif" };
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Formato no permitido" });

            var fileName = $"{seccion}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
            var uploadPath = Path.Combine(_environment.WebRootPath, "images", "configuracion", seccion);
            Directory.CreateDirectory(uploadPath);
            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);
            
            var url = $"https://quiniela.sigo.com.ve:8443/images/configuracion/{seccion}/{fileName}"; 
            /*var url = $"http://localhost:5000/images/configuracion/{seccion}/{fileName}";*/
            return Ok(new { url });
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] ConfiguracionVisual config)
        {
            if (config == null)
                return BadRequest(new { message = "Datos inválidos" });

            var result = await _databaseService.ExecuteStoredProcedureSingle<ConfiguracionVisual>(
                "quiniela.SP_GuardarConfiguracionVisual",
                new
                {
                    Id = config.Id,
                    Seccion = config.Seccion,
                    Tipo = config.Tipo,
                    Clave = config.Clave,
                    ValorTexto = config.ValorTexto,
                    ValorImagen = config.ValorImagen,
                    Link = config.Link,
                    Color = config.Color,
                    Orden = config.Orden,
                    Activo = config.Activo
                });
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _databaseService.ExecuteStoredProcedure("quiniela.SP_EliminarConfiguracionVisual", new { Id = id });
            return Ok(new { message = "Eliminado" });
        }

        [HttpPost("orden")]
        public async Task<IActionResult> ActualizarOrden([FromBody] List<OrdenItem> items)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(items);
            await _databaseService.ExecuteStoredProcedure("quiniela.SP_ActualizarOrdenConfiguracion", new { Items = json });
            return Ok();
        }
    }

    public class OrdenItem
    {
        public int Id { get; set; }
        public int Orden { get; set; }
    }
}