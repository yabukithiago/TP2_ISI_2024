using System.ComponentModel.DataAnnotations;

namespace TP2_ISI_2024.Models
{
	public class Room : ModelBase
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
		[Required]
		public decimal Cost { get; set; }

		[Required]
		public string Adress { get; set; }
		public bool Available { get; set; }
	}
}
