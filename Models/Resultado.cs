namespace Quiniela.Models
{
    public class Resultado
    {
        public int IdJuego { get; set; }
        public DateTime Fecha { get; set; }
        public string? Ronda { get; set; }
        public string? Equipo1 { get; set; }
        public string? Siglas1 { get; set; }
        public byte Resultado1 { get; set; }
        public string? Equipo2 { get; set; }
        public string? Siglas2 { get; set; }
        public byte Resultado2 { get; set; }
        public bool IndFinalizado { get; set; }
    }
}