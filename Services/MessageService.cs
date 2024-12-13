using System.Collections.Generic;
using System.ServiceModel;
using System.Data;
using Npgsql;
using TP2_ISI_2024.Models;
using TP2_ISI_2024.Controllers;
using TP2_ISI_2024.Interface;

namespace TP2_ISI_2024.Services
{
	public class MessageService : IMessageService
	{
		private readonly string _connectionString;
		public MessageService()
		{
			var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			_connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		public List<Message> GetMessages()
		{
			using var connection = new NpgsqlConnection(_connectionString);
			connection.Open();

			var command = new NpgsqlCommand("SELECT * FROM \"Messages\"", connection);
			using var reader = command.ExecuteReader();

			var messages = new List<Message>();
			while (reader.Read())
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
			return messages;
		}

		public Message GetMessage(int id)
		{
			using var connection = new NpgsqlConnection(_connectionString);
			connection.Open();

			var command = new NpgsqlCommand("SELECT * FROM \"Messages\" WHERE \"Id\" = @Id", connection);
			command.Parameters.AddWithValue("@Id", id);

			using var reader = command.ExecuteReader();
			if (!reader.Read())
			{
				return null; // Or handle exception
			}

			return new Message
			{
				Id = reader.GetInt32(0),
				Name = reader.GetString(1),
				Description = reader.GetString(2),
				Reply = reader.IsDBNull(3) ? null : reader.GetString(3),
				UserId = reader.GetInt32(4),
				CreatedAt = reader.GetDateTime(5),
				UpdatedAt = reader.GetDateTime(6)
			};
		}

		public Message PostMessage(MessageCreateDto messageDto)
		{
			using var connection = new NpgsqlConnection(_connectionString);
			connection.Open();

			var command = new NpgsqlCommand(
				"INSERT INTO \"Messages\" (\"Name\", \"Description\", \"UserId\", \"CreatedAt\", \"UpdatedAt\") VALUES (@Name, @Description, @UserId, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) RETURNING \"Id\"", connection);
			command.Parameters.AddWithValue("@Name", messageDto.Name);
			command.Parameters.AddWithValue("@Description", messageDto.Description);
			command.Parameters.AddWithValue("@UserId", messageDto.UserId);

			var messageId = (int)command.ExecuteScalar();
			return new Message
			{
				Id = messageId,
				Name = messageDto.Name,
				Description = messageDto.Description
			};
		}

		public void PutMessage(int id, MessageUpdateDto messageDto)
		{
			using var connection = new NpgsqlConnection(_connectionString);
			connection.Open();

			var command = new NpgsqlCommand(
				"UPDATE \"Messages\" SET \"Name\" = @Name, \"Description\" = @Description, \"Reply\" = @Reply, \"UpdatedAt\" = CURRENT_TIMESTAMP WHERE \"Id\" = @Id", connection);
			command.Parameters.AddWithValue("@Id", id);
			command.Parameters.AddWithValue("@Name", messageDto.Name);
			command.Parameters.AddWithValue("@Description", messageDto.Description);
			command.Parameters.AddWithValue("@Reply", messageDto.Reply ?? (object)DBNull.Value);

			var rowsAffected = command.ExecuteNonQuery();
			if (rowsAffected == 0)
			{
				throw new FaultException("Message not found");
			}
		}

		public void DeleteMessage(int id)
		{
			using var connection = new NpgsqlConnection(_connectionString);
			connection.Open();

			var command = new NpgsqlCommand("DELETE FROM \"Messages\" WHERE \"Id\" = @Id", connection);
			command.Parameters.AddWithValue("@Id", id);

			var rowsAffected = command.ExecuteNonQuery();
			if (rowsAffected == 0)
			{
				throw new FaultException("Message not found");
			}
		}
	}
}
