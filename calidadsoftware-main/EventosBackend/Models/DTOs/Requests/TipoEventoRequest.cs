namespace EventosBackend.Models.DTOs.Requests
{
    public class TipoEventoRequest
    {
        public int? IdTipoEvento { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }
    }
}