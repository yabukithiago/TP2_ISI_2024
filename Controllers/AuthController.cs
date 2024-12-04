using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TP2_ISI_2024.Data;
using TP2_ISI_2024.Models;

namespace TP2_ISI_2024.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;

		public AuthController(ApplicationDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		[HttpPost("register")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
		{
			var userExists = await _context.Users.AnyAsync(u => u.Email == userRegisterDto.Email || u.Name == userRegisterDto.Name);
			if (userExists)
				return BadRequest("Usuário já existe. Nome ou E-mail já cadastrado");

			// Crie um novo usuário
			var newUser = new User
			{
				Name = userRegisterDto.Name,
				Email = userRegisterDto.Email,
				Password = HashPassword(userRegisterDto.Password),
				Birth = userRegisterDto.Birth.ToUniversalTime(),
				Floor = userRegisterDto.Floor,
				Role = userRegisterDto.Role
			};

			await _context.Users.AddAsync(newUser);
			await _context.SaveChangesAsync();

			return Ok("Usuário registrado com sucesso.");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userLoginDto.Email);

			if (user == null || !VerifyPassword(userLoginDto.Password, user.Password))
			{
				return Unauthorized("Usuário ou senha inválidos.");
			}

			var token = GenerateJwtToken(user);

			return Ok(new { Token = token });
		}

		private string GenerateJwtToken(User user)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, user.Email),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim("UserName", user.Name),
			new Claim("UserId", user.Id.ToString()),
			new Claim(ClaimTypes.Role, user.Role.ToString())
		};

			var token = new JwtSecurityToken(
				_configuration["Jwt:Issuer"],
				_configuration["Jwt:Audience"],
				claims,
				expires: DateTime.UtcNow.AddDays(30),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}	

		private string HashPassword(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}

		private bool VerifyPassword(string password, string hashedPassword)
		{
			return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
		}
	}

	public class UserRegisterDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Password { get; set; }
		public DateTime Birth { get; set; }
		[Required]
		public string Floor { get; set; }

		[Required]
		public Role Role { get; set; }
	}

	public class UserLoginDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
	}
}
