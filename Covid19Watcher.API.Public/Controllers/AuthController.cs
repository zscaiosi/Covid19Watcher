using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Covid19Watcher.Application.Contracts;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace Covid19Watcher.API.Public.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthController(IConfiguration conf)
        {
            _config = conf;
        }
        /// <summary>
        /// Dumb authentication. Has no service for simplicity's sake
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostAuthenticate([FromBody]PostAuth request)
        {
            if (request == null || string.IsNullOrEmpty(request.Login) || string.IsNullOrEmpty(request.Password))
                return StatusCode(401);
            // It is a dump authentication. Just to avoid abuses from clients
            var (_login, _psw) = (_config.GetSection("Credentials").GetSection("Login").Value, _config.GetSection("Credentials").GetSection("Password").Value);

            if (_psw != request.Password || _login != request.Login)
                return StatusCode(401);
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("Credentials").GetSection("key").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, _login)
                    }
                ),
                Expires = DateTime.UtcNow.AddHours(1),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "Covid19Watcher"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return StatusCode(200, new PostAuthResponse{ AccessToken = tokenHandler.WriteToken(token) });
        }
    }
}