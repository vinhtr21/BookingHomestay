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


namespace Go2Hotel.Controllers
{
    //[Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly Booking_HomestayContext _context;
        private readonly IConfiguration _config;
        public HttpClient httpClient = new HttpClient();

        public LoginController(Booking_HomestayContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Login([FromBody] UserDTO user)
        {
            Guest tmp = await _context.Guests.FirstOrDefaultAsync(g => g.GuestPhone == user.GuestPhone);
            if (tmp == null)
            {
                return BadRequest("this phone is not registered. please registered");
            }
            Guest g = CheckGuestExist(user.GuestPhone, user.GuestPassword);
            if (g == null)
            {
                return BadRequest("double check your phone number and password then try again!");
            }
            if (g.GuestStatus == 0 || g.GuestStatus == null)
            {
                return BadRequest("your account has been disabled. contact to us via 0912181156 for help");
            }
            var token = JwtTokenGenerate(g.GuestName, g.GuestPhone);
            var refreshToken = GenerateRefreshToken();
            g.RefreshToken = refreshToken;
            g.RefreshTokenExpireTime = DateTime.Now.AddHours(5);
            _context.Entry(g).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new
            {
                userId = g.GuestId,
                token = token,
                refreshToken = refreshToken,
            });
        }

        [Route("refresh-token")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequest refreshTokenRequest)
        {
            var user = await _context.Guests.FirstOrDefaultAsync(g => g.RefreshToken == refreshTokenRequest.RefreshToken);
            if (user == null || user.RefreshTokenExpireTime <= DateTime.Now)
            {
                return BadRequest("Invalid token");
            }

            // Generate new access token
            var accessToken = JwtTokenGenerate(user.GuestName, user.GuestPhone);
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

        [Route("checkAccessToken")]
        [HttpPost]
        public async Task<IActionResult> CheckAccessToken([FromBody] string token)
        {
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
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return Ok();
            }
            catch (SecurityTokenValidationException)
            {
                return BadRequest();
            }
        }

        [Route("logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        private Guest CheckGuestExist(string phone, string password)
        {
            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                return null;
            }
            Guest g = (Guest)_context.Guests.FirstOrDefault(g => g.GuestPhone == phone);
            if (g == null || g.GuestPassword != PasswordHasher.HashPassword(password))
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
                new Claim(ClaimTypes.Role, "Guest"),
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
