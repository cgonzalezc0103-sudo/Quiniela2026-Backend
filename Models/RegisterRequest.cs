namespace Quiniela.Models
{
    public class RegisterRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty; // NUEVO: Cédula (requerido)
        public string? CodigoPromocional { get; set; } // NUEVO: Código promocional (opcional)
        public int? IdEquipo { get; set; } // Equipo favorito (opcional)
    }
}