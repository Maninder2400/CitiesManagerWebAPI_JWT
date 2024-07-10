using CitiesManager.Core.Identity;
using CitiesManager.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CitiesManager.Infrastructure.DatabaseContext
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
	{
		public ApplicationDbContext(DbContextOptions options) : base(options) { }
		public ApplicationDbContext() { }

		public virtual  DbSet<City> Cities { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<City>().HasData(new City() { CityID = Guid.Parse("F012F6F3-5267-430A-B89B-54AAA83BE11E"), CityName = "New York" });
			modelBuilder.Entity<City>().HasData(new City() { CityID = Guid.Parse("56AA9698-C9B8-45DB-87AB-24904F030DF3"), CityName = "London" });


		}
	}
}
