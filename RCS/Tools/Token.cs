using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Tools
{
    public class Token
    {
        public static JwtSecurityToken GenerateToken(string key, int expires, string username)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var token = new JwtSecurityToken(
                    expires: DateTime.Now.AddDays(expires),
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                    issuer: username,
                    audience: username
                );

            return token;
        }
    }
}
