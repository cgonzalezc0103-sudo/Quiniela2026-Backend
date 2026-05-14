namespace Quiniela.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observacion { get; set; }
    }

    public class RegistrarPagoRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string ImagenBase64 { get; set; } = string.Empty;
    }

    public class ActualizarEstadoPagoRequest
    {
        public string Estado { get; set; } = string.Empty;
        public string? Observacion { get; set; }
    }
}