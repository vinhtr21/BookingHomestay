using Go2Hotel.DTO;
using Go2Hotel.Models;
using Go2Hotel.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace Go2Hotel.Controllers
{
    //[Route("[controller]")]
    [ApiController]
    public class OwnerLoginController : ControllerBase
    {
        private readonly Booking_HomestayContext _context;
        private readonly IConfiguration _config;
        public HttpClient httpClient = new HttpClient();
        private EmailSender sender = new EmailSender();

        public OwnerLoginController(Booking_HomestayContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Route("owner-login")]
        [HttpPost]
        public async Task<ActionResult<OwnerDTO>> Login([FromBody] OwnerDTO owner)
        {
            Owner tmp = await _context.Owners.FirstOrDefaultAsync(g => g.OwnerPhone == owner.OwnerPhone);
            if (tmp == null)
            {
                return BadRequest("this phone is not registered. please registered");
            }
            Owner g = CheckGuestExist(owner.OwnerPhone, owner.OwnerPassword);
            if (g == null)
            {
                return BadRequest("double check your phone number and password then try again!");
            }
            if (g.OwnerStatus == 0 || g.OwnerStatus == null)
            {
                return BadRequest("your account has been disabled. contact to us via 0912181156 for help");
            }
            var token = JwtTokenGenerate(g.OwnerName, g.OwnerPhone);
            var refreshToken = GenerateRefreshToken();
            g.RefreshToken = refreshToken;
            g.RefreshTokenExpireTime = DateTime.Now.AddHours(5);
            _context.Entry(g).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new
            {
                userId = g.OwnerId,
                token = token,
                refreshToken = refreshToken,
            });
        }

        [Route("owner-refresh-token")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequest refreshTokenRequest)
        {
            var user = await _context.Owners.FirstOrDefaultAsync(g => g.RefreshToken == refreshTokenRequest.RefreshToken);
            if (user == null || user.RefreshTokenExpireTime <= DateTime.Now)
            {
                return BadRequest("Invalid token");
            }

            // Generate new access token
            var accessToken = JwtTokenGenerate(user.OwnerName, user.OwnerPhone);
            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Append("AccessToken", accessToken);
            // Update refresh token expiry time
            user.RefreshTokenExpireTime = DateTime.Now.AddDays(5); // Refresh token expiry time
            await _context.SaveChangesAsync();

            // Return new access token
            return Ok(new
            {
                token = accessToken
            });
        }


        [AllowAnonymous]
        [HttpPost("owner-forgot-password")]
        public async Task<IActionResult> ForgotPassword([Required] ForgotPasswordModel model)
        {
            var user = await _context.Owners.FirstOrDefaultAsync(u => u.OwnerEmail == model.Email);
            if (user == null)
            {
                return BadRequest("email not found");
            }

            var token = Guid.NewGuid().ToString("N");
            var expireTime = DateTime.UtcNow.AddMinutes(5);
            var forgotToken = new ForgotPasswordModel
            {
                Email = model.Email,
                ExpireDate = expireTime,
            };
            sender.SendMail(model.Email, "RESET PASSWORD!", $"The code is: {token} \n Code is available in 5 minutes since this email is sent \n DO NOT SHARE TO ANYONE FOR PRIVACY!!");
            try
            {
                user.ResetPasswordToken = token.ToString();
                await _context.SaveChangesAsync();
                return Ok(new { token = token });

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model, CancellationToken cancellation = default)
        {
            var user = await _context.Owners.FirstOrDefaultAsync(u => u.ResetPasswordToken == model.Token, cancellation);
            if (user == null)
            {
                return BadRequest("invalid token");
            }
            try
            {
                user.OwnerPassword = PasswordHasher.HashPassword(model.ConfirmPassword);
                user.ResetPasswordToken = null;
                await _context.SaveChangesAsync(cancellation);
                return Ok("reset successfull");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("getOwners")]
        public async Task<ActionResult<List<Owner>>> GetAllOwners()
        {
            if (_context.Owners == null)
            {
                return NotFound();
            }
            var owners = await _context.Owners.ToListAsync();

            if (owners == null)
            {
                return NotFound();
            }

            return owners;
        }
        private Owner CheckGuestExist(string phone, string password)
        {
            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                return null;
            }
            Owner g = (Owner)_context.Owners.FirstOrDefault(g => g.OwnerPhone == phone);
            if (g == null || g.OwnerPassword != PasswordHasher.HashPassword(password))
            {
                return null;
            }
            return g;
        }

        private string JwtTokenGenerate(string email, string phone)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.MobilePhone, phone),
                new Claim(ClaimTypes.Role, "Owner"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                expires: DateTime.Now.AddMinutes(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
                );

            var result = new JwtSecurityTokenHandler().WriteToken(token);
            return result;
        }

        private bool CheckCookies()
        {
            if (Request.Cookies.TryGetValue("AccessToken", out string jwtToken))
            {
                // Validate the JWT signature
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _config["JwtSettings:Issuer"],
                    ValidAudience = _config["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"])),
                    ClockSkew = TimeSpan.Zero
                };

                try
                {
                    ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(jwtToken, validationParameters, out SecurityToken validatedToken);
                    return true;
                }
                catch (SecurityTokenValidationException)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetClaimsPrincipalFromExpiredAccessToken(string token)
        {
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"])),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}
