using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventosBackend.Models.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using EventosBackend.Models.Configuration;

public interface IJwtService
{
    string GenerateToken(Usuario usuario);
}

// JwtService.cs
public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(Usuario usuario)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Crear claims con todos los datos necesarios
        var claims = new List<Claim>
            {
                new Claim("idUsuario", usuario.IdUsuario),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("nombre", usuario.Nombre),
                new Claim("apellido1", usuario.Apellido1),
                new Claim("apellido2", usuario.Apellido2 ?? string.Empty),
                new Claim("telefono", usuario.Telefono ?? string.Empty),
                new Claim("tipoUsuario", usuario.TipoUsuario),
                new Claim("estado", usuario.Estado),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Claims estándar
                new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtSettings.ValidIssuer),
                new Claim(JwtRegisteredClaimNames.Aud, _jwtSettings.ValidAudience),
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUnico.ToString()),
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}