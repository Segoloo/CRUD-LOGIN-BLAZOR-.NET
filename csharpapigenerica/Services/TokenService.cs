using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace csharpapigenerica.Services
{
    public interface ITokenService
    {
        string GenerarToken(string usuario, List<string> roles);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuracion;

        public TokenService(IConfiguration configuracion)
        {
            _configuracion = configuracion ?? throw new ArgumentNullException(nameof(configuracion));
        }

        public string GenerarToken(string usuario, List<string> roles)
        {
            var claveJwt = _configuracion["Jwt:Key"]
                ?? throw new InvalidOperationException("La clave JWT no está configurada correctamente.");

            var claveSecreta = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveJwt));
            var credenciales = new SigningCredentials(claveSecreta, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, usuario)
            };

            // Agrega todos los roles como claims
            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuracion["Jwt:Issuer"],
                audience: _configuracion["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credenciales
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
