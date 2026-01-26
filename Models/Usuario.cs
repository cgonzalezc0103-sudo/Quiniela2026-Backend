namespace Quiniela.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string? UserName { get; set; }
        public string? Nombres { get; set; }
        public string? Email { get; set; }
        public bool IndActivo { get; set; }
        public string? Rol { get; set; }
        public string? Empresa { get; set; }
    }
}