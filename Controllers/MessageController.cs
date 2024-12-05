using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using TP2_ISI_2024.Data;
using TP2_ISI_2024.Models;

namespace TP2_ISI_2024.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TicketController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public TicketController(ApplicationDbContext context)
		{
			_context = context;
		}
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<Message>>> GetTickets()
		{
			return Ok(await _context.Messages.ToListAsync());
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Admin,User")]
		public async Task<ActionResult<Message>> GetTicket(int id)
		{
			var ticket = await _context.Messages.FindAsync(id);

			if (ticket == null)
			{
				return NotFound();
			}

			return ticket;
		}

		[HttpGet("my")]
		[Authorize(Roles = "User")]
		public async Task<ActionResult<IEnumerable<Message>>> GetUserTickets()
		{
			var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
			if (userIdClaim == null)
			{
				return Unauthorized();
			}

			var userId = int.Parse(userIdClaim.Value);

			var tickets = await _context.Messages
			.Where(t => t.UserId == userId)
										.ToListAsync();

			return Ok(tickets);
		}


		[HttpPost]
		[Authorize(Roles = "User")]
		public async Task<ActionResult<Message>> PostTicket(TicketCreateDto ticketDto)
		{
			var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
			if (userIdClaim == null)
			{
				return Unauthorized();
			}

			var userId = int.Parse(userIdClaim.Value);

			var ticket = new Message
			{
				Name = ticketDto.Name,
				Description = ticketDto.Description,
				UserId = userId
			};

			_context.Messages.Add(ticket);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> PutTicket(int id, TicketUpdateDto TicketUpdateDto)
		{
			if (!TicketExists(id))
			{
				return BadRequest();
			}

			try
			{
				var ticket = await _context.Messages.FindAsync(id);

				ticket.Name = TicketUpdateDto.Name;
				ticket.Description = TicketUpdateDto.Description;
				ticket.Reply = TicketUpdateDto.Reply;
				ticket.UpdatedAt = DateTime.UtcNow;
				await _context.SaveChangesAsync();
				return Ok(TicketUpdateDto);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!TicketExists(id))
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
		[Authorize]
		public async Task<IActionResult> DeleteTicket(int id)
		{
			var ticket = await _context.Messages.FindAsync(id);
			if (ticket == null)
			{
				return NotFound();
			}

			_context.Messages.Remove(ticket);
			await _context.SaveChangesAsync();

			return Ok("Ticket removido com sucesso.");
		}

		private bool TicketExists(int id)
		{
			return _context.Messages.Any(e => e.Id == id);
		}
	}
	public class TicketCreateDto
	{
		[Required]
		public string Name { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }
	}
	public class TicketUpdateDto
	{
		[Required]
		public string Name { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }

		public string Reply { get; set; }
	}
}
