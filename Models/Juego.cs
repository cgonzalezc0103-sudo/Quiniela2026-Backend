namespace Quiniela.Models
{
    public class Juego
    {
        public int IdJuego { get; set; }
        public int IdRonda { get; set; }
        public string? Ronda { get; set; }
        public string? Equipo1 { get; set; }
        public string? Siglas1 { get; set; }
        public string? Equipo2 { get; set; }
        public string? Siglas2 { get; set; }
        public DateTime Fecha { get; set; }
        public byte? Pronostico1 { get; set; }
        public byte? Pronostico2 { get; set; }
        public int? IdPronostico { get; set; }
        public bool PermitePronostico { get; set; }
    }
}