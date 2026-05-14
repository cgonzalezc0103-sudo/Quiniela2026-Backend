using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiniela.Data;
using Quiniela.Models;
using System.Data;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagosController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public PagosController(IDatabaseService databaseService, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _databaseService = databaseService;
            _environment = environment;
            _configuration = configuration;
        }

        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<IActionResult> RegistrarPago([FromBody] RegistrarPagoRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Datos requeridos" });
                }

                if (string.IsNullOrEmpty(request.Nombre) || string.IsNullOrEmpty(request.Cedula) ||
                    string.IsNullOrEmpty(request.Referencia) || request.Monto <= 0)
                {
                    return BadRequest(new { message = "Todos los campos son requeridos" });
                }

                if (string.IsNullOrEmpty(request.ImagenBase64))
                {
                    return BadRequest(new { message = "La imagen del comprobante es requerida" });
                }

                // Decodificar imagen
                var imageBytes = Convert.FromBase64String(request.ImagenBase64.Split(',')[1]);

                // Crear directorio si no existe
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "pagos");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generar nombre único para el archivo
                var fileName = $"pago_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}.jpg";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Guardar archivo
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                // URL relativa para acceder a la imagen
                var imageUrl = $"/images/pagos/{fileName}";

                var parameters = new DynamicParameters();
                parameters.Add("@Nombre", request.Nombre);
                parameters.Add("@Cedula", request.Cedula);
                parameters.Add("@Telefono", request.Telefono);
                parameters.Add("@Referencia", request.Referencia);
                parameters.Add("@Monto", request.Monto);
                parameters.Add("@ImagenUrl", imageUrl);
                parameters.Add("@IdPago", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await _databaseService.ExecuteStoredProcedure("quiniela.SP_RegistrarPago", parameters);

                var idPago = parameters.Get<int>("@IdPago");

                return Ok(new
                {
                    message = "Pago registrado exitosamente. Será revisado por un administrador.",
                    idPago = idPago
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al registrar pago: " + ex.Message });
            }
        }

        [HttpGet("pendientes")]
        [Authorize(Roles = "Administrador Site")]
        public async Task<IActionResult> GetPagosPendientes()
        {
            try
            {
                var pagos = await _databaseService.ExecuteStoredProcedure<Pago>("quiniela.SP_ObtenerPagosPendientes");
                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrador Site")]
        public async Task<IActionResult> GetTodosPagos()
        {
            try
            {
                var pagos = await _databaseService.ExecuteStoredProcedure<Pago>("quiniela.SP_ObtenerTodosPagos");
                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/estado")]
        [Authorize(Roles = "Administrador Site")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoPagoRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Estado))
                {
                    return BadRequest(new { message = "Estado requerido" });
                }

                var pago = await _databaseService.ExecuteStoredProcedureSingle<Pago>(
                    "quiniela.SP_ActualizarEstadoPago",
                    new { IdPago = id, Estado = request.Estado, Observacion = request.Observacion }
                );

                if (pago == null)
                {
                    return NotFound(new { message = "Pago no encontrado" });
                }

                return Ok(new { message = $"Pago {request.Estado.ToLower()} exitosamente", pago });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}