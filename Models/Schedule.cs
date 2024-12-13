using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace TP2_ISI_2024.Models
{
	public class Schedule : ModelBase
	{
		public int Id { get; set; }


		[Required]
		public DateTime Date { get; set; }
		[Required]
		public bool Available { get; set; }
		[Required]
		public bool Canceled { get; set; }

		[ForeignKey("UserId")]
		public int UserId { get; set; }
		[ForeignKey("RoomId")]
		public int RoomId { get; set; }
		public User? User { get; set; }
		public CommonSpace? Room { get; set; }
	}
}
