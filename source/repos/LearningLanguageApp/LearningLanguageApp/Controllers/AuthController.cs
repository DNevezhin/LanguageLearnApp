using BCrypt.Net;
using LearningLanguageApp.Data;
using LearningLanguageApp.Entities;
using LearningLanguageApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearningLanguageApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register (RegisterDto dto)
        {
            if (_context.Users.Any(l =>l.Login == dto.Login))
            {
                return BadRequest("Пользователь уже существует");
            }
            User user = new User { Login = dto.Login, Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)};

            _context.Users.Add(user);

            _context.SaveChanges();

            return Ok("Пользователь успешно создан");

        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(l => l.Login == dto.Login);

            if (user == null) 
            {
                return Unauthorized("Неверный логин или пароль");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);

            if (!isPasswordValid)
            {
                return Unauthorized("Неверный логин или пароль");
            }
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login) };

            var secretKey = _configuration["JwtSettings:SecretKey"];

            var key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new {token= tokenString});

        }
           
    }

}