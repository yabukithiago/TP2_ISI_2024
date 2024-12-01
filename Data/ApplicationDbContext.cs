using Microsoft.EntityFrameworkCore;
using TP2_ISI_2024.Models;

namespace TP2_ISI_2024.Data
{
	public class ApplicationDbContext : DbContext
	{

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
	: base(options)
		{
		}
		public DbSet<User> Users { get; set; }
		public DbSet<Visitor> Visitors { get; set; }
		//public DbSet<IndividualWarning> IndividualWarnigns { get; set; }
		//public DbSet<GlobalWarning> GlobalWarnigns { get; set; }
		//public DbSet<Ticket> Tickets { get; set; }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TP2_ISI_DB;Username=postgres;Password=123");
		}
	}
}
