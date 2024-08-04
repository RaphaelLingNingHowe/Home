using Microsoft.IdentityModel.Tokens;
using ProgramGuard.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProgramGuard.Services
{
    public class TokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly int _expires;
        public TokenService(string secretKey, string issuer, int expires)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _expires = expires;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor

            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Account),
                new Claim("visiblePrivilege", $"{(uint)user.PrivilegeRule.Visible}"),
                new Claim("operatePrivilege", $"{(uint)user.PrivilegeRule.Operate}"),
            }),
                Expires = DateTime.Now.AddMinutes(_expires),
                Issuer = _issuer,
                Audience = _issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
