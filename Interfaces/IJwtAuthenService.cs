using Microsoft.AspNetCore.Mvc;
using OrderingFood.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OrderingFood.Interfaces
{
    public interface IJwtAuthenService
    {
        public string GenerateToken(User _user);
        public JwtSecurityToken GetToken(List<Claim> authClaims);
    }
}
