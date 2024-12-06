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
	[Authorize(Roles = "Rec")]
	public class IrrigationController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;

		public IrrigationController(ApplicationDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Irrigation>>> GetAllIrrigations()
		{
			return await _context.Irrigations.ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Irrigation>> GetIrrigationById(int id)
		{
			var irrigation = await _context.Irrigations.FindAsync(id);

			if (irrigation == null)
			{
				return NotFound();
			}

			return irrigation;
		}

		[HttpPost]
		public async Task<ActionResult<Irrigation>> PostIrrigation(IrrigationCreateDto irrigationDto)
		{
			var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
			if (userIdClaim == null)
			{
				return Unauthorized();
			}

			var userId = int.Parse(userIdClaim.Value);
			var irrigation = new Irrigation
			{
				InitialDate = DateTime.UtcNow,
				EndDate = irrigationDto.EndDate.ToUniversalTime(),
				UserId = userId,
				Temperature = irrigationDto.Temperature,
			};

			_context.Irrigations.Add(irrigation);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetAllIrrigations", new { id = irrigation.Id }, irrigation);
		}

		public class IrrigationCreateDto
		{
			[StringLength(100)]
			public string? Temperature { get; set; }
			public DateTime EndDate { get; set; }
		}
	}
}
