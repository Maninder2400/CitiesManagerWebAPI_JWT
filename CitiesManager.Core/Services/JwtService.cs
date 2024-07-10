using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.Services
{
    public class JwtService : IJwtService
    {
        IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AuthenticationResponse CreateJwtToken(ApplicationUser user)
        {
            DateTime expirationTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

            Claim[] claims = new Claim[] // payload
            {
                new Claim(JwtRegisteredClaimNames.Sub , user.Id.ToString()),//subject user id
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()), // unique id for a particular token
                new Claim(JwtRegisteredClaimNames.Iat , DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()), // Issued at (date and time of token generation)
                new Claim(ClaimTypes.NameIdentifier , user.Email),//Unique name identifier for the user
                new Claim(ClaimTypes.Name , user.PersonName),//Name of the user
                new Claim(ClaimTypes.Email , user.Email),//Name of the user

            };

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(  //secret key
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            SigningCredentials signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);  // hashing

            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"], claims, expires: expirationTime, signingCredentials: signingCredentials);

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(tokenGenerator);

            return new AuthenticationResponse()
            {
                Token = token,
                Email = user.Email,
                PersonName = user.PersonName,
                Expiration = expirationTime,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationDateTime = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["RefreshToken:EXPIRATION_MINUTES"]))
            };
        }

        //Creates a refresh token (base 64 string of random numbers)
        private string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
            var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }


        public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),

                ValidateLifetime = false //should be false
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
