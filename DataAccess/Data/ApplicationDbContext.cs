using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using MoviesAPI.Models;


namespace MoviesAPI.Data
{
	public class ApplicationDbContext:IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}
        public DbSet<Category> categories { get; set; }
        public DbSet<Movie> movies { get; set; }
        public DbSet<Cinema> cinemas { get; set; }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<BookTicket> bookTickets { get; set; }
    }
}
