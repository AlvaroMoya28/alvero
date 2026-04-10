using System;

namespace EventosBackend.Models.DTOs.Requests
{
    public class ReservaRequest
    {
        public string IdUsuario { get; set; } = string.Empty;
        public int IdSala { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int? AsistentesEsperados { get; set; }
        public string? Observaciones { get; set; }
        public int? IdCatering { get; set; }
        public decimal? PrecioTotal { get; set; }
    }
}
