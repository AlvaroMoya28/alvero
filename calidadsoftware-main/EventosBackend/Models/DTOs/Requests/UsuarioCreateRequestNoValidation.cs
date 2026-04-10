using System.ComponentModel.DataAnnotations;

namespace EventosBackend.Models.DTOs.Requests
{
    // DTO específico para creación de técnicos desde el admin.
    // Evita la validación por regex/Compare en la contraseña que aplica al DTO general.
    public class UsuarioCreateRequestNoValidation
    {
        [Required(ErrorMessage = "El ID de usuario es obligatorio")]
        [StringLength(50, ErrorMessage = "El ID de usuario no puede exceder los 50 caracteres")]
        public string IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        [StringLength(150, ErrorMessage = "El apellido no puede exceder los 150 caracteres")]
        public string Apellido1 { get; set; }

        [StringLength(150, ErrorMessage = "El apellido no puede exceder los 150 caracteres")]
        public string? Apellido2 { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        // Mantener la contraseña requerida pero sin la regex/Compare que provocaba 400 para la contraseña temporal
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "La contraseña debe tener entre 1 y 100 caracteres")]
        public string Contrasena { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        public string ConfirmarContrasena { get; set; }

        // El rol será forzado a 'TECNICO' por el controlador
        public string Rol { get; set; }
    }
}
