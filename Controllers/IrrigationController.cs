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
			var rega = await _context.Irrigations.FindAsync(id);

			if (rega == null)
			{
				return NotFound();
			}

			return rega;
		}

		//[HttpGet("my")]
		//public async Task<ActionResult<IEnumerable<IndividualWarning>>> GetRegaByToken()
		//{
		//	var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
		//	if (userIdClaim == null)
		//	{
		//		return Unauthorized();
		//	}

		//	var userId = int.Parse(userIdClaim.Value);

		//	var rega = await _context.Irrigations
		//								.Where(t => t.UserId == userId)
		//								.ToListAsync();

		//	return Ok(rega);
		//}

		[HttpPost]
		public async Task<ActionResult<Irrigation>> PostIrrigation(RegaCreateDto regaDto)
		{
			var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
			if (userIdClaim == null)
			{
				return Unauthorized();
			}

			var userId = int.Parse(userIdClaim.Value);
			var rega = new Irrigation
			{
				InitialDate = DateTime.UtcNow,
				EndDate = regaDto.EndDate.ToUniversalTime(),
				UserId = userId,
				Temperature = regaDto.Temperature,
			};

			_context.Irrigations.Add(rega);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetAllIrrigations", new { id = rega.Id }, rega);
		}

		public class RegaCreateDto
		{
			[StringLength(100)]
			public string? Temperature { get; set; }
			public DateTime EndDate { get; set; }
		}
	}
}
