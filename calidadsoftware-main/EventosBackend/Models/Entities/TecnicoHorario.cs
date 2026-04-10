namespace EventosBackend.Models.Entities
{
    public class TecnicoHorario
    {
        public int IdHorario { get; set; }
        public string IdUsuario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string HoraInicio { get; set; } = string.Empty; // "08:00"
        public string HoraFin { get; set; } = string.Empty; // "09:00"
        public bool Disponible { get; set; } = true;
        public string? MotivoBloqueo { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Navegación
        public Usuario? Usuario { get; set; }
    }
}
