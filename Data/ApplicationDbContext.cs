using Microsoft.EntityFrameworkCore;
using TP2_ISI_2024.Models;

namespace TP2_ISI_2024.Data
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Visitor> Visitors { get; set; }
		public DbSet<Schedule> Schedules { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<Room> Rooms { get; set; }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TP2_ISI_DB;Username=postgres;Password=123");
		}
	}
}
