using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProgramGuard.Config;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProgramGuard.Common.Helper
{
    public class JwtHelper
    {
        private readonly JwtSettings _jwtSettings;
        private readonly SymmetricSecurityKey _securityKey;

        public JwtHelper(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
        }

        public string GenerateToken(User user)
        {
            if (user.PrivilegeRule == null)
            {
                throw new InvalidOperationException("User's PrivilegeRule cannot be null.");
            }
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, user.Name),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Account),
                    new Claim("visiblePrivilege", $"{(uint)user.PrivilegeRule.Visible}"),
                    new Claim("operatePrivilege", $"{(uint)user.PrivilegeRule.Operate}")
                }),

                Expires = DateTime.Now.AddMinutes(_jwtSettings.ExpiresInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateChangePasswordOnlyToken(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, user.Name),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Account),
                }),
                Expires = DateTime.Now.AddMinutes(_jwtSettings.ExpiresInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
