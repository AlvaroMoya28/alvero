public class LoginResponse
{
    public string Token { get; set; }
    public UserInfoResponse UserInfo { get; set; }
}

public class UserInfoResponse
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; }
    public string Apellido1 { get; set; }
    public string Apellido2 { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
    public string TipoUsuario { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Estado { get; set; }
}