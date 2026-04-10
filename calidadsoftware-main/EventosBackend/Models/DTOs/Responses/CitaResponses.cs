namespace EventosBackend.Models.DTOs.Responses
{
    public class HorarioDisponibleResponse
    {
        public int IdHorario { get; set; }
        public string IdUsuario { get; set; }
        public string NombreTecnico { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
        public bool DisponibleReal { get; set; }
        public string? MotivoBloqueo { get; set; }
    }

    public class CitaResponse
    {
        public int IdCita { get; set; }
        public string IdUsuarioCliente { get; set; } = string.Empty;
        public string NombreCliente { get; set; } = string.Empty;
        public string IdUsuarioTecnico { get; set; } = string.Empty;
        public string NombreTecnico { get; set; } = string.Empty;
        public DateTime FechaCita { get; set; }
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
        public string? DescripcionProblema { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? TelefonoContacto { get; set; }
    }

    public class DisponibilidadAgregadaResponse
    {
        public DateTime Fecha { get; set; }
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
        public int TecnicosDisponibles { get; set; }
        public List<TecnicoDisponibleInfo> Tecnicos { get; set; } = new List<TecnicoDisponibleInfo>();
    }

    public class TecnicoDisponibleInfo
    {
        public string IdTecnico { get; set; } = string.Empty;
        public string NombreTecnico { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
