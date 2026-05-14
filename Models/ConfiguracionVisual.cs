namespace Quiniela.Models
{
    public class ConfiguracionVisual
    {
        public int Id { get; set; }
        public string Seccion { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public string? ValorTexto { get; set; }
        public string? ValorImagen { get; set; }
        public string? Link { get; set; }
        public string? Color { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}