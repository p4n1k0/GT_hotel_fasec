using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Services
{
    public class TokenGenerator
    {
        private readonly TokenOptions _tokenOptions;
        public TokenGenerator()
        {
            _tokenOptions = new TokenOptions
            {
                Secret = "4d82a63bbdc67c1e4784ed6587f3730c",
                ExpiresDay = 1
            };

        }
        public string Generate(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Subject = AddClaims(user),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey
                (Encoding.ASCII.GetBytes(_tokenOptions.Secret!)), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.Now.AddDays(_tokenOptions.ExpiresDay),
            });
            return tokenHandler.WriteToken(token);
        }

        private static ClaimsIdentity AddClaims(UserDto user)
        {
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new (ClaimTypes.Email, user.Email!));

            if ("admin" == user.UserType)
            {
                claimsIdentity.AddClaim(new (ClaimTypes.Role, "admin"));
            }
            return claimsIdentity;
        }
    }
}
