using Lucene.Net.Support;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using NuGet.Configuration;
using OrderingFood.Data;
using OrderingFood.Interfaces;
using OrderingFood.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace OrderingFood.Services
{
    public class JwtAuthenService : IJwtAuthenService
    {
        private readonly IConfiguration _configuration;
        private readonly FoodieContext _context;


        public JwtAuthenService(IConfiguration configuration,FoodieContext context)
        {
            _configuration = configuration;
            _context = context;

        }

        public string GenerateToken(User _user)
        {
            
                var authClaims = new List<Claim>
                {
                   new Claim(JwtRegisteredClaimNames.Name, _user.Name!),
                   new Claim(JwtRegisteredClaimNames.Email, _user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, _user.UserId.ToString()),

                };

                var token = GetToken(authClaims);

                return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddSeconds(30000),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }





    }
}
