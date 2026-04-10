using System;

namespace EventosBackend.Models.DTOs.Responses
{
    public class ReservaResponse
    {
        public int IdReserva { get; set; }
        public string IdUsuario { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        // Información de la sala
        public int IdSala { get; set; }
        public string? SalaNombre { get; set; }
        public string? SalaDescripcionCorta { get; set; }
        public decimal SalaPrecioBase { get; set; }

        // Nuevo: indica si el usuario ya dejó reseña para esta reserva
        public bool TieneResena { get; set; }
        public decimal? PrecioTotal { get; set; }
    }
}