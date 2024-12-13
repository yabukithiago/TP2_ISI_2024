using System.ComponentModel.DataAnnotations;

namespace TP2_ISI_2024.Models
{
	public class CommonSpace : ModelBase
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public User user { get; set; }

		[Required]
		public string Type { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public int Capacitity { get; set; }
		public bool Available { get; set; }
	}
}
