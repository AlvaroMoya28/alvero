// EventosBackend/Models/DTOs/Requests/ChangePasswordRequest.cs
using System.ComponentModel.DataAnnotations;

namespace EventosBackend.Models.DTOs.Requests
{
    public class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres.")]
        // Considerar añadir una expresión regular para complejidad si es necesario,
        // Ejemplo:
        // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
        //    ErrorMessage = "La contraseña debe contener mayúsculas, minúsculas, números y caracteres especiales.")]
        public string NewPassword { get; set; }
    }
}