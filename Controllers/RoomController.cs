using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TP2_ISI_2024.Data;
using TP2_ISI_2024.Models;

namespace TP2_ISI_2024.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RoomController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public RoomController(ApplicationDbContext _context)
		{
			this._context = _context;
		}

		// Lista todos os quartos com informações do usuario
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<Room>>> GetRoom()
		{
			return await _context.Rooms
				.Include(q => q.user)
				.Where(q => q.Available)
				.ToListAsync();
		}

		// Lista um quarto específico com informações do usuario
		[HttpGet("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<Room>> GetRoom(int id)
		{
			var quarto = await _context.Rooms
				.Include(q => q.user)
				.FirstOrDefaultAsync(q => q.Id == id);

			if (quarto == null)
			{
				return NotFound();
			}

			return quarto;
		}

		// Cria um quarto
		[HttpPost]
		[Authorize(Roles = "Admin,Rec")]
		public async Task<ActionResult<Room>> PostRoom([FromBody] Room room)
		{
			_context.Rooms.Add(room);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
		}

		// Atualiza um quarto
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin,Rec")]
		public async Task<IActionResult> PutRoom(int id, Room room)
		{
			if (id != room.Id)
			{
				return BadRequest();
			}

			_context.Entry(room).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.Rooms.Any(e => e.Id == id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// Deleta um quarto
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<Room>> DeleteRoom(int id)
		{
			var quarto = await _context.Rooms.FindAsync(id);
			if (quarto == null)
			{
				return NotFound();
			}

			_context.Rooms.Remove(quarto);
			await _context.SaveChangesAsync();

			return quarto;
		}

		// Endpoint para buscar quartos
		[HttpGet("search")]
		public async Task<IActionResult> Search(
			[FromQuery] string query = "",
			[FromQuery] string type = "",
			[FromQuery] decimal? maxPrice = null,
			[FromQuery] string location = ""
		)
		{
			try
			{
				var quartosQuery = _context.Rooms
					.Include(q => q.user)
					.AsQueryable();

				var lowerQuery = query.ToLower();
				var lowerType = type.ToLower();
				var lowerLocation = location.ToLower();

				if (!string.IsNullOrEmpty(lowerQuery))
				{
					quartosQuery = quartosQuery.Where(q =>
						q.Type.ToLower().Contains(lowerQuery)
						|| q.Description.ToLower().Contains(lowerQuery));
				}

				if (!string.IsNullOrEmpty(lowerType))
				{
					quartosQuery = quartosQuery.Where(q => q.Type.ToLower() == lowerType);
				}

				if (maxPrice.HasValue)
				{
					quartosQuery = quartosQuery.Where(q => q.Cost <= maxPrice.Value);
				}

				if (!string.IsNullOrEmpty(lowerLocation))
				{
					quartosQuery = quartosQuery.Where(q =>
						q.Adress.ToLower().Contains(lowerLocation));
				}

				var filteredQuartos = await quartosQuery.ToListAsync();

				return Ok(filteredQuartos);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}
