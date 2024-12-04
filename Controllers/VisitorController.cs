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
	public class VisitorController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;

		public VisitorController(ApplicationDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		[HttpGet]
		[Authorize(Roles = "Rec")]
		public async Task<ActionResult<Visitor>> GetAll()
		{
			return Ok(await _context.Visitors.ToListAsync());
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "User,Rec")]
		public async Task<ActionResult<Visitor>> GetById(int id)
		{
			var visitor = await _context.Visitors.FindAsync(id);

			if (visitor == null)
			{
				return NotFound();
			}

			return Ok(visitor);
		}

		[HttpGet("my")]
		[Authorize(Roles = "User")]
		public async Task<ActionResult<Visitor>> GetByUserId()
		{
			var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
			if (userIdClaim == null)
			{
				return Unauthorized();
			}

			var userId = int.Parse(userIdClaim.Value);
			var visitor = await _context.Visitors
							.Where(t => t.UserId == userId)
							.ToListAsync();

			return Ok(visitor);
		}

		[HttpPost]
		[Authorize(Roles = "Rec")]
		public async Task<ActionResult<Visitor>> PostVisitor(CreateVisitorDTO visitorDto)
		{
			var visitor = new Visitor
			{
				Name = visitorDto.Name,
				VisitDate = visitorDto.VisitDate.ToUniversalTime(),
				UserId = visitorDto.UserId,
			};

			_context.Visitors.Add(visitor);
			await _context.SaveChangesAsync();

			return Ok(visitor);
		}
		[HttpPut("{id}")]
		[Authorize(Roles = "Rec")]
		public async Task<IActionResult> PutVisitor(int id, UpdateVisitorDTO updateVisitorDTO)
		{
			if (!VisitorExists(id))
			{
				return BadRequest();
			}

			try
			{
				var visitor = await _context.Visitors.FindAsync(id);

				visitor.Name = updateVisitorDTO.Name;
				visitor.VisitDate = updateVisitorDTO.VisitDate.ToUniversalTime();
				visitor.UserId = updateVisitorDTO.UserId;
				visitor.UpdatedAt = DateTime.UtcNow;
				await _context.SaveChangesAsync();
				return Ok(visitor);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!VisitorExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Rec")]
		public async Task<IActionResult> DeleteVisitor(int id)
		{
			var visitor = await _context.Visitors.FindAsync(id);
			if (visitor == null)
			{
				return NotFound();
			}

			_context.Visitors.Remove(visitor);
			await _context.SaveChangesAsync();

			return Ok("Visitante removido com sucesso.");
		}

		private bool VisitorExists(int id)
		{
			return _context.Visitors.Any(e => e.Id == id);
		}

		public class VisitorDTO
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public DateTime VisitDate { get; set; }
			public int UserId { get; set; }
		}

		public class CreateVisitorDTO
		{
			[Required]
			[StringLength(100)]
			public string Name { get; set; }
			[Required]
			public DateTime VisitDate { get; set; }
			[Required]
			public int UserId { get; set; }
		}
		public class UpdateVisitorDTO
		{
			[StringLength(100)]
			public string Name { get; set; }
			public DateTime VisitDate { get; set; }
			public int UserId { get; set; }
		}
	}
}
