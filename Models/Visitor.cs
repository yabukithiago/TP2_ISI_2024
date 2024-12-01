using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TP2_ISI_2024.Models
{
	public class Visitor : ModelBase
	{
		public int Id { get; set; }
		[Required]
		[StringLength(100)]
		public string Name { get; set; }
		public DateTime VisitDate { get; set; }
		[ForeignKey("UserId")]
		public int UserId { get; set; }

		public User? User { get; set; }
	}
}
