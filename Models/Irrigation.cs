using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TP2_ISI_2024.Models
{
	public class Irrigation : ModelBase
	{
		public int Id { get; set; }

		[Required]
		public DateTime InitialDate { get; set; }
		public DateTime EndDate { get; set; }
		[Required]
		[StringLength(100)]
		public string? Temperature { get; set; }
		[ForeignKey("UserId")]
		public int UserId { get; set; }

		public User? User { get; set; }
	}
}
