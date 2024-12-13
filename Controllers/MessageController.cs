using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using TP2_ISI_2024.Models;

namespace TP2_ISI_2024.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MessageController : ControllerBase
	{
		private readonly string _connectionString;
		private readonly IConfiguration _configuration;

		public MessageController(IConfiguration configuration)
		{
			_configuration = configuration;
			_connectionString = _configuration.GetConnectionString("DefaultConnection");
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
		{
			using var connection = new NpgsqlConnection(_connectionString);
			await connection.OpenAsync();

			var command = new NpgsqlCommand("SELECT * FROM \"Messages\"", connection);
			using var reader = await command.ExecuteReaderAsync();

			var messages = new List<Message>();
			while (await reader.ReadAsync())
			{
				messages.Add(new Message
				{
					Id = reader.GetInt32(0),
					Name = reader.GetString(1),
					Description = reader.GetString(2),
					Reply = reader.IsDBNull(3) ? null : reader.GetString(3),
					UserId = reader.GetInt32(4),
					CreatedAt = reader.GetDateTime(5),
					UpdatedAt = reader.GetDateTime(6)
				});
			}

			return Ok(messages);
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Admin,User")]
		public async Task<ActionResult<Message>> GetMessage(int id)
		{
			using var connection = new NpgsqlConnection(_connectionString);
			await connection.OpenAsync();

			var command = new NpgsqlCommand("SELECT * FROM \"Messages\" WHERE \"Id\" = @Id", connection);
			command.Parameters.AddWithValue("@Id", id);

			using var reader = await command.ExecuteReaderAsync();
			if (!await reader.ReadAsync())
			{
				return NotFound();
			}

			var message = new Message
			{
				Id = reader.GetInt32(0),
				Name = reader.GetString(1),
				Description = reader.GetString(2),
				Reply = reader.IsDBNull(3) ? null : reader.GetString(3),
				UserId = reader.GetInt32(4),
				CreatedAt = reader.GetDateTime(5),
				UpdatedAt = reader.GetDateTime(6)
			};

			return Ok(message);
		}

		[HttpPost]
		[Authorize(Roles = "Admin, User")]
		public async Task<ActionResult<Message>> PostMessage(MessageCreateDto messageDto)
		{
			var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
			if (userIdClaim == null)
			{
				return Unauthorized();
			}

			var userId = int.Parse(userIdClaim.Value);

			using var connection = new NpgsqlConnection(_connectionString);
			await connection.OpenAsync();

			var command = new NpgsqlCommand(
				"INSERT INTO \"Messages\" (\"Name\", \"Description\", \"UserId\", \"CreatedAt\", \"UpdatedAt\") VALUES (@Name, @Description, @UserId, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) RETURNING \"Id\"", connection);

			command.Parameters.AddWithValue("@Name", messageDto.Name);
			command.Parameters.AddWithValue("@Description", messageDto.Description);
			command.Parameters.AddWithValue("@UserId", userId);

			var messageId = (int)await command.ExecuteScalarAsync();

			return CreatedAtAction(nameof(GetMessage), new { id = messageId }, new { Id = messageId, messageDto.Name, messageDto.Description });
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> PutMessage(int id, MessageUpdateDto messageDto)
		{
			using var connection = new NpgsqlConnection(_connectionString);
			await connection.OpenAsync();

			var command = new NpgsqlCommand(
				"UPDATE \"Messages\" SET \"Name\" = @Name, \"Description\" = @Description, \"Reply\" = @Reply, \"UpdatedAt\" = CURRENT_TIMESTAMP WHERE \"Id\" = @Id", connection);

			command.Parameters.AddWithValue("@Id", id);
			command.Parameters.AddWithValue("@Name", messageDto.Name);
			command.Parameters.AddWithValue("@Description", messageDto.Description);
			command.Parameters.AddWithValue("@Reply", messageDto.Reply ?? (object)DBNull.Value);

			var rowsAffected = await command.ExecuteNonQueryAsync();
			if (rowsAffected == 0)
			{
				return NotFound();
			}

			return Ok();
		}

		[HttpDelete("{id}")]
		[Authorize]
		public async Task<IActionResult> DeleteMessage(int id)
		{
			using var connection = new NpgsqlConnection(_connectionString);
			await connection.OpenAsync();

			var command = new NpgsqlCommand("DELETE FROM \"Messages\" WHERE \"Id\" = @Id", connection);
			command.Parameters.AddWithValue("@Id", id);

			var rowsAffected = await command.ExecuteNonQueryAsync();
			if (rowsAffected == 0)
			{
				return NotFound();
			}

			return Ok("Mensagem removida com sucesso.");
		}
	}

	public class MessageCreateDto
	{
		public int UserId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
		
	public class MessageUpdateDto
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Reply { get; set; }
	}
}