namespace EventosBackend.Models.DTOs.Requests
{
    public class CrearCitaRequest
    {
        public string? NombreCliente { get; set; }
        public string? EmailCliente { get; set; }
        public string? CedulaCliente { get; set; }
        public string IdUsuarioTecnico { get; set; } = string.Empty;
        public DateTime FechaCita { get; set; }
        public string HoraInicio { get; set; } = string.Empty;
        public string? DescripcionProblema { get; set; }
        public string? Direccion { get; set; }
        public string? TelefonoContacto { get; set; }
    }

    public class BloquearHorarioRequest
    {
        public DateTime Fecha { get; set; }
        public string HoraInicio { get; set; } = string.Empty;
        public string MotivoBloqueo { get; set; } = string.Empty;
    }

    public class GenerarHorariosRequest
    {
        public string IdTecnico { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
    }
}
