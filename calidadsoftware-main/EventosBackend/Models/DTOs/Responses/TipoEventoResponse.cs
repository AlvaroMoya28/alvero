namespace EventosBackend.Models.DTOs.Responses
{
    public class TipoEventoResponse
    {
        public int IdTipoEvento { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }
    }
}