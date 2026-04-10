using System.ComponentModel.DataAnnotations;

namespace EventosBackend.Models.DTOs.Requests
{
    public class ResenaCreateRequest
    {
        [Required]
        public int IdReserva { get; set; }
        [Required]
        [MaxLength(50)]
        public string IdUsuario { get; set; } = string.Empty;
        [Required]
        [Range(1, 5)]
        public int Calificacion { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Comentario { get; set; } = string.Empty;
    }
}
