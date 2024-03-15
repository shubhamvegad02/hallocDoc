using halloDocEntities.DataContext;
using halloDocEntities.DataModels;
using halloDocLogic.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using halloDocEntities.ViewDataModels;

namespace halloDocLogic.Repository
{
    public class JwtService : IJwtService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        


        public JwtService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration; 
        }

        public string GenerateJwtToken(Aspnetuser aspuser)
        {
            string userRole = "";
            var dbasprole = _context.Aspnetuserroles.FirstOrDefault(m => m.UserId == aspuser.Id);
            if (dbasprole != null)
            {
                var dbuserRole = _context.Aspnetroles.FirstOrDefault(m => m.AspNetRoleId == dbasprole.RoleId);
                userRole = dbuserRole.Name;
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, aspuser.Email),
                new Claim("aspId", aspuser.Id),
                new Claim(ClaimTypes.Role, userRole),
                /*new Claim("userId", user.UserId.ToString()),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),*/

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(6);
            var token = new JwtSecurityToken(
                Convert.ToString(_configuration["Jwt:Issuer"]),
                Convert.ToString(_configuration["Jwt:Audience"]),
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken)
        {
            jwtSecurityToken = null;

            if (token == null)
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                jwtSecurityToken = (JwtSecurityToken)validatedToken;

                if (jwtSecurityToken != null)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }



        public JwtClaimsModel GetClaimsFromJwtToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // Ensure the token is signed with a valid key
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])), // Use the same key for signing
                    ValidateIssuer = true, // Validate the issuer against configuration
                    ValidIssuer = Convert.ToString(_configuration["Jwt:Issuer"]),
                    ValidateAudience = true, // Validate the audience against configuration
                    ValidAudiences = new List<string> { Convert.ToString(_configuration["Jwt:Audience"]) },
                    ClockSkew = TimeSpan.Zero // Set clock skew to zero for stricter validation (optional)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var claims = principal.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);

                // Extract and validate claims securely
                var email = claims.ContainsKey(ClaimTypes.Email) ? claims[ClaimTypes.Email] : null;
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Missing or invalid 'email' claim in JWT token.");
                }

                var role = claims.ContainsKey(ClaimTypes.Role) ? claims[ClaimTypes.Role] : null;
                if (string.IsNullOrEmpty(role))
                {
                    throw new ArgumentException("Missing or invalid 'role' claim in JWT token.");
                }

                var aspId = claims.ContainsKey("aspId") ? claims["aspId"] : null;
                if (string.IsNullOrEmpty(aspId))
                {
                    throw new ArgumentException("Missing or invalid 'aspId' claim in JWT token.");
                }

                return new JwtClaimsModel
                {
                    Email = email,
                    Role = role,
                    AspId = aspId
                };
            }
            catch (SecurityTokenException ex)
            {
                // Handle potential security exceptions (e.g., invalid token, expired token)
                // Log the exception and return null
                Console.WriteLine("Error validating token: {0}", ex.Message);
                return null;
            }
            catch (ArgumentException ex)
            {
                // Handle missing or invalid claims
                Console.WriteLine("Error: {0}", ex.Message);
                return null;
            }
        }

        public string encry(string pass)
        {
            if (pass == null)
            {
                return null;
            }
            else
            {
                byte[] storePass = ASCIIEncoding.ASCII.GetBytes(pass);
                string encryptedPass = Convert.ToBase64String(storePass);
                return encryptedPass;
            }

        }

        public string decry(string pass)
        {
            if (pass == null)
            {
                return null;
            }
            else
            {
                byte[] encryptedPass = Convert.FromBase64String(pass);
                string decryptedPass = ASCIIEncoding.ASCII.GetString(encryptedPass);
                return decryptedPass;
            }
        }



    }
}
