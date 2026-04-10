namespace EventosBackend.Models.Entities
{
    public class Cita
    {
        public int IdCita { get; set; }
        
        // Usuario cliente (opcional - solo si tiene cuenta)
        public string? IdUsuarioCliente { get; set; }
        
        // Información del cliente (para reservas públicas)
        public string? NombreCliente { get; set; }
        public string? EmailCliente { get; set; }
        public string? CedulaCliente { get; set; }
        
        public string IdUsuarioTecnico { get; set; } = string.Empty;
        public DateTime FechaCita { get; set; }
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
        public string? DescripcionProblema { get; set; }
        public string Estado { get; set; } = "PENDIENTE"; // PENDIENTE, CONFIRMADA, COMPLETADA, CANCELADA
        public string? Direccion { get; set; }
        public string? TelefonoContacto { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        // Navegación
        public Usuario? Cliente { get; set; }
        public Usuario? Tecnico { get; set; }
    }
}
