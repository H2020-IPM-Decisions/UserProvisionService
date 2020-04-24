using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace H2020.IPMDecisions.UPR.Tests
{
    public static class TokenGeneratorTests
    {
        private static List<Claim> GenerateClaims(Guid userId, string userRole, IList<Claim> userClaims = null)
        {
            IdentityOptions _options = new IdentityOptions();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(_options.ClaimsIdentity.UserIdClaimType, userId.ToString())
            };

            if (userClaims != null)
            {
                claims.AddRange(userClaims);
            }            

            claims.Add(new Claim(ClaimTypes.Role, userRole));

            return claims;
        }

        public static string GenerateToken(
                Guid userId, 
                string userRole = "", 
                IList<Claim> userClaims = null)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();
                
            var tokenLifetimeMinutes = "30";
            var issuerServerUrl = configuration["JwtSettings:IssuerServerUrl"];
            var jwtSecretKey = configuration["JwtSettings:SecretKey"];
            var audienceServerUrl = configuration["JwtSettings:ValidAudiencesUrls"];

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: issuerServerUrl,
                audience: audienceServerUrl,
                claims: GenerateClaims(userId, userRole, userClaims),
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(double.Parse(tokenLifetimeMinutes)),
                signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }
    }
}