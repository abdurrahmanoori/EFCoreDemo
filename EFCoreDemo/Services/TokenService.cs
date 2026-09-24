using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EFCoreDemo.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
namespace EFCoreDemo.Services;
public class TokenService(IConfiguration configuration)
{
    public (string Token,DateTime ExpiresAtUtc) Create(User user)
    {
        var expires=DateTime.UtcNow.AddMinutes(configuration.GetValue("Jwt:ExpiryMinutes",480));
        var claims=new[]{new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),new Claim(ClaimTypes.Name,user.Username),new Claim(ClaimTypes.GivenName,user.FullName),new Claim(ClaimTypes.Role,user.Role.ToString())};
        var key=configuration["Jwt:Key"]??throw new InvalidOperationException("Jwt:Key is not configured.");
        var jwt=new JwtSecurityToken(configuration["Jwt:Issuer"],configuration["Jwt:Audience"],claims,expires:expires,
            signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),SecurityAlgorithms.HmacSha256));
        return(new JwtSecurityTokenHandler().WriteToken(jwt),expires);
    }
}