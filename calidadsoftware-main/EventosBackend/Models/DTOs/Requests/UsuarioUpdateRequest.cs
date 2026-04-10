using System;
using System.ComponentModel.DataAnnotations;

namespace EventosBackend.Models.DTOs.Requests
{
    public class UsuarioUpdateRequest
    {
        [Required(ErrorMessage = "El Id de usuario es obligatorio")]
        public int IdUsuario { get; set; }

        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string? Nombre { get; set; }

        [StringLength(150, ErrorMessage = "El apellido no puede exceder los 150 caracteres")]
        public string? Apellido1 { get; set; }

        [StringLength(150, ErrorMessage = "El apellido no puede exceder los 150 caracteres")]
        public string? Apellido2 { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string? Telefono { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "La contraseña debe contener mayúsculas, minúsculas, números y caracteres especiales")]
        public string? Contrasena { get; set; }

        [DataType(DataType.Password)]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmarContrasena { get; set; }

        [StringLength(20, ErrorMessage = "El tipo de usuario no puede exceder los 20 caracteres")]
        public string? TipoUsuario { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaRegistro { get; set; }

        public DateTime? UltimoLogin { get; set; }

        [StringLength(10, ErrorMessage = "El estado no puede exceder los 10 caracteres")]
        public string? Estado { get; set; }
    }
}