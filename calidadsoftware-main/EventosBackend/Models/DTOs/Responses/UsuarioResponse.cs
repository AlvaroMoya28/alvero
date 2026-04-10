// UsuarioResponse.cs
using System;

namespace EventosBackend.Models.DTOs.Responses
{
    public class UsuarioResponse
    {
        public string IdUnico { get; set; }
        public string IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido1 { get; set; }
        public string? Apellido2 { get; set; }
        public string Email { get; set; }
        public string? Telefono { get; set; }
        public string TipoUsuario { get; set; }
        public string Estado { get; set; }
    }
}