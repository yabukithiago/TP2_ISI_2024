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
	public class ScheduleController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;

		public ScheduleController(ApplicationDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		[HttpGet]
		[Authorize(Roles = "Admin,User")]
		public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedules()
		{
			return Ok(await _context.Schedules.ToListAsync());
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Admin,User")]
		public async Task<ActionResult<Schedule>> GetScheduleById(int id)
		{
			var area = await _context.Schedules.FindAsync(id);

			if (area == null)
			{
				return NotFound();
			}

			return area;
		}
		[HttpGet("my")]
		[Authorize(Roles = "Admin,User")]
		public async Task<ActionResult<Schedule>> GetScheduleByUser()
		{
			var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
			if (userIdClaim == null)
			{
				return Unauthorized();
			}

			var userId = int.Parse(userIdClaim.Value);

			var schedule = await _context.Schedules
										.Where(t => t.UserId == userId)
										.ToListAsync();

			return Ok(schedule);
		}

		[HttpPost]
		[Authorize(Roles = "Admin,User")]
		public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDTO createScheduleDTO)
		{
			var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
			if (userIdClaim == null)
			{
				return Unauthorized();
			}

			var userId = int.Parse(userIdClaim.Value);
			var existingReservation = _context.Schedules
				.Any(s => s.RoomId == createScheduleDTO.RoomId &&
						  s.Date >= DateTime.UtcNow.AddDays(-2) &&
						  s.Date <= DateTime.UtcNow);

			if (existingReservation)
			{
				return BadRequest("Você não pode fazer uma nova reserva nesse quarto com o espaço de menos de 2 dias.");
			}

			var conflictingReservation = _context.Schedules.Any(s =>
				s.RoomId == createScheduleDTO.RoomId &&
				s.Date == createScheduleDTO.Date);

			if (conflictingReservation)
			{
				return BadRequest("Já existe uma reserva nesse quarto e nessa data.");
			}

			var schedule = new Schedule
			{
				Date = createScheduleDTO.Date.ToUniversalTime(),
				UserId = userId,
				RoomId = createScheduleDTO.RoomId,
				Available = true,
				Canceled = false
			};

			_context.Schedules.Add(schedule);
			await _context.SaveChangesAsync();

			return Ok(schedule);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> PutSchedule(int id, UpdateScheduleDTO updateScheduleDTO)
		{
			if (!ScheduleExists(id))
			{
				return BadRequest();
			}

			try
			{
				var schedule = await _context.Schedules.FindAsync(id);

				schedule.Canceled = updateScheduleDTO.Canceled;
				schedule.UpdatedAt = DateTime.UtcNow;
				await _context.SaveChangesAsync();
				return Ok(schedule);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ScheduleExists(id))
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
		[Authorize(Roles = "Admin,User")]
		public async Task<IActionResult> DeleteSchedule(int id)
		{
			var schedule = await _context.Schedules.FindAsync(id);
			if (schedule == null)
			{
				return NotFound();
			}

			_context.Schedules.Remove(schedule);
			await _context.SaveChangesAsync();

			return Ok("Agendamento removido com sucesso.");
		}

		private bool ScheduleExists(int id)
		{
			return _context.Schedules.Any(e => e.Id == id);
		}

		public class ScheduleDTO
		{
			public int Id { get; set; }
			public DateTime Date { get; set; }
			public bool Available { get; set; }
			public bool Canceled { get; set; }
			public int UserId { get; set; }
			public int RoomId { get; set; }
		}
		public class CreateScheduleDTO
		{

			[Required]
			public DateTime Date { get; set; }

			[Required]
			public int RoomId { get; set; }
		}
		public class UpdateScheduleDTO
		{
			public bool Canceled { get; set; }
		}

	}
}
