using System.ComponentModel.DataAnnotations;

namespace EventosBackend.Models.DTOs.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El ID de usuario es obligatorio")]
        [StringLength(50, ErrorMessage = "El ID de usuario no puede exceder los 50 caracteres")]
        public string Id { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}