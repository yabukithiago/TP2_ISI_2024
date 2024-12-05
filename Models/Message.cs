using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TP2_ISI_2024.Models
{
	public class Message : ModelBase
	{
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		[StringLength(500)]
		public string Description { get; set; }
		public string? Reply { get; set; }
		[ForeignKey("UserId")]
		public int UserId { get; set; }

		public User? User { get; set; }

	}
}
