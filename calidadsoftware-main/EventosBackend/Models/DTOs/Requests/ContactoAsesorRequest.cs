namespace EventosBackend.Models.DTOs.Requests
{
    public class ContactoAsesorRequest
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Mensaje { get; set; }
        public int SalaId { get; set; }
    }
}