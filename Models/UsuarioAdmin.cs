namespace Quiniela.Models
{
    public class UsuarioAdmin
    {
        public int IdUsuario { get; set; }
        public string? UserName { get; set; }
        public string? Nombres { get; set; }
        public string? Email { get; set; }
        public bool IndActivo { get; set; }
        public string? Rol { get; set; }
        public string? Empresa { get; set; }
        public string? Cedula { get; set; }
        public int? IdRol { get; set; }
        public int? IdEmpresa { get; set; }
        public int? IdEquipo { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}