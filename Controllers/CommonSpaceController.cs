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
	public class CommonSpaceController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public CommonSpaceController(ApplicationDbContext _context)
		{
			this._context = _context;
		}

		[HttpGet]
		[Authorize(Roles = "Admin, Rec")]
		public async Task<ActionResult<IEnumerable<CommonSpace>>> GetCommonSpace()
		{
			return await _context.CommonSpace
				.Include(q => q.user)
				.Where(q => q.Available)
				.ToListAsync();
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Admin, Rec")]
		public async Task<ActionResult<CommonSpace>> GetCommonSpace(int id)
		{
			var quarto = await _context.CommonSpace
				.Include(q => q.user)
				.FirstOrDefaultAsync(q => q.Id == id);

			if (quarto == null)
			{
				return NotFound();
			}

			return quarto;
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CommonSpace>> PostCommonSpace([FromBody] CommonSpace room)
		{
			_context.CommonSpace.Add(room);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetCommonSpace), new { id = room.Id }, room);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> PutCommonSpace(int id, CommonSpace room)
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
				if (!_context.CommonSpace.Any(e => e.Id == id))
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

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CommonSpace>> DeleteCommonSpace(int id)
		{
			var quarto = await _context.CommonSpace.FindAsync(id);
			if (quarto == null)
			{
				return NotFound();
			}

			_context.CommonSpace.Remove(quarto);
			await _context.SaveChangesAsync();

			return quarto;
		}

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
				var quartosQuery = _context.CommonSpace
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
