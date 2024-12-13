using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TP2_ISI_2024.Models
{
	public class Message
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Reply { get; set; }
		public int UserId { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
