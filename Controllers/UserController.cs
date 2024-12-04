using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TP2_ISI_2024.Data;
using TP2_ISI_2024.Models;

namespace TP2_ISI_2024.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public UserController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		[Authorize(Roles = "Admin,Rec")]
		public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
		{
			var users = await _context.Users
				.Select(u => new UserReadDto
				{
					Id = u.Id,
					Email = u.Email,
					Name = u.Name,
					Floor = u.Floor,
					Birth = u.Birth,
					Role = u.Role,
					CreatedAt = u.CreatedAt,
					UpdatedAt = u.UpdatedAt,
				})
				.ToListAsync();

			return Ok(users);
		}
		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<UserReadDto>> GetUser(int id)
		{
			var user = await _context.Users.FindAsync(id);

			if (user == null || (user.Id != id && !User.IsInRole("Admin")))
			{
				return NotFound();
			}

			return new UserReadDto
			{
				Id = user.Id,
				Email = user.Email,
				Name = user.Name,
				Floor = user.Floor,
				Birth = user.Birth,
				Role = user.Role
			};
		}

		[HttpGet("my")]
		[Authorize]
		public async Task<ActionResult<User>> GetUserByToken()
		{
			var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
			if (userIdClaim == null)
			{
				return Unauthorized();
			}

			var userId = int.Parse(userIdClaim.Value);

			var users = await _context.Users
										  .Where(u => u.Id == userId)
										  .Select(u => new
										  {
											  Id = u.Id,
											  Name = u.Name,
											  Email = u.Email,
											  Floor = u.Floor,
											  Birth = u.Birth,
											  Role = u.Role,
											  UpdatedAt = u.UpdatedAt,
											  CreatedAt = u.CreatedAt
										  })
										  .SingleOrDefaultAsync();

			return Ok(users);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin,User")]
		public async Task<IActionResult> PutUser(int id, UserUpdateDto userUpdateDto)
		{
			if (!UserExists(id))
			{
				return NotFound();
			}

			var user = await _context.Users.FindAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			user.Email = userUpdateDto.Email;
			user.Name = userUpdateDto.Name;
			user.Floor = userUpdateDto.Floor;
			user.Birth = userUpdateDto.Birth.ToUniversalTime();
			user.Role = userUpdateDto.Role;
			user.UpdatedAt = DateTime.UtcNow;

			try
			{
				await _context.SaveChangesAsync();
				return Ok(userUpdateDto);
			}
			catch (DbUpdateConcurrencyException)
			{
				return BadRequest();
			}
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin,User")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null)
			{
				return NotFound("Usuário não encontrado.");
			}

			_context.Users.Remove(user);
			await _context.SaveChangesAsync();

			return Ok("Usuário excluído com sucesso.");
		}

		private bool UserExists(int id)
		{
			return _context.Users.Any(e => e.Id == id);
		}
	}

	public class UserReadDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Floor { get; set; }
		public DateTime Birth { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public Role Role { get; set; }
	}

	public class UserCreateDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }
		[Required]
		public string Floor { get; set; }
		[Required]
		public string Name { get; set; }
		public DateTime Birth { get; set; }
		[Required]
		public Role Role { get; set; }

	}
	public class UserUpdateDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		public string Name { get; set; }
		public string Floor { get; set; }
		public DateTime Birth { get; set; }
		public Role Role { get; set; }
	}
}
