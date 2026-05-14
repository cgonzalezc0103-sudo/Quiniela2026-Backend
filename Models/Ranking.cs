namespace Quiniela.Models
{
    public class Ranking
    {
        public int Posicion { get; set; }
        public int IdUsuario { get; set; }
        public string? Nombres { get; set; }
        public string? Empresa { get; set; }
        public int PuntosTotales { get; set; }
        public string? Alias { get; set; }

        public string? UltimaActualizacion { get; set; }
    }

    public class HistorialPunto
    {
        public string Tipo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Ronda { get; set; }
        public int? Pronostico1 { get; set; }
        public int? Pronostico2 { get; set; }
        public int? Resultado1 { get; set; }
        public int? Resultado2 { get; set; }
        public int PuntosObtenidos { get; set; }
        public string TipoAcierto { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }
}