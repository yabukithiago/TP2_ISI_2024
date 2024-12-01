using System.ComponentModel.DataAnnotations;

namespace TP2_ISI_2024.Models
{
	public class User : ModelBase
	{
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		[Required]
		[EmailAddress]
		[StringLength(255)]
		public string Email { get; set; }

		[Required]
		[StringLength(255)]
		public string Password { get; set; }
		[Required]
		[StringLength(60)]
		public string Floor { get; set; }

		public DateTime Birth { get; set; }

		[Required]
		public Role Role { get; set; }
	}
	public enum Role
	{
		Admin,
		User,
		Rec
	}
}
