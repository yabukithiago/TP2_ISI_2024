namespace TP2_ISI_2024.Models
{
	public abstract class ModelBase
	{
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		protected ModelBase()
		{
			CreatedAt = DateTime.UtcNow;
			UpdatedAt = DateTime.UtcNow;
		}
	}
}
