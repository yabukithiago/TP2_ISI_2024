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
	public class MessageController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public MessageController(ApplicationDbContext context)
		{
			_context = context;
		}
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
		{
			return Ok(await _context.Messages.ToListAsync());
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Admin,User")]
		public async Task<ActionResult<Message>> GetMessages(int id)
		{
			var message = await _context.Messages.FindAsync(id);

			if (message == null)
			{
				return NotFound();
			}

			return message;
		}

		[HttpGet("my")]
		[Authorize(Roles = "User")]
		public async Task<ActionResult<IEnumerable<Message>>> GetUserMessages()
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
		public async Task<ActionResult<Message>> PostMessages(MessageCreateDto messageDto)
		{
			var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
			if (userIdClaim == null)
			{
				return Unauthorized();
			}

			var userId = int.Parse(userIdClaim.Value);

			var message = new Message
			{
				Name = messageDto.Name,
				Description = messageDto.Description,
				UserId = userId
			};

			_context.Messages.Add(message);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetMessages), new { id = message.Id }, message);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> PutTicket(int id, MessageUpdateDto MessageUpdateDto)
		{
			if (!MessageExists(id))
			{
				return BadRequest();
			}

			try
			{
				var message = await _context.Messages.FindAsync(id);

				message.Name = MessageUpdateDto.Name;
				message.Description = MessageUpdateDto.Description;
				message.Reply = MessageUpdateDto.Reply;
				message.UpdatedAt = DateTime.UtcNow;
				await _context.SaveChangesAsync();
				return Ok(MessageUpdateDto);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!MessageExists(id))
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
		public async Task<IActionResult> DeleteMessage(int id)
		{
			var ticket = await _context.Messages.FindAsync(id);
			if (ticket == null)
			{
				return NotFound();
			}

			_context.Messages.Remove(ticket);
			await _context.SaveChangesAsync();

			return Ok("Mensagem removida com sucesso.");
		}

		private bool MessageExists(int id)
		{
			return _context.Messages.Any(e => e.Id == id);
		}
	}
	public class MessageCreateDto
	{
		[Required]
		public string Name { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }
	}
	public class MessageUpdateDto
	{
		[Required]
		public string Name { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }

		public string Reply { get; set; }
	}
}
