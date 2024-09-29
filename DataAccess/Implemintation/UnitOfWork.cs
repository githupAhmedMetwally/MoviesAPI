using Models.IRepository;
using MoviesAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Implemintation
{
	public class UnitOfWork:IUnitOfWork
	{
		private readonly ApplicationDbContext _context;
		public ICategory Category { get; private set; }
		public IMovie Movie { get; private set; }
		public ICinema Cinema { get; private set; }

		public IApplicationUser ApplicationUser { get; private set; }
		public IBookTicket BookTicket { get; private set; }

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
			Category = new CategoryRepository(context); 
			Movie = new MovieRepository(context); 
			Cinema = new CinemaRepository(context); 
			ApplicationUser = new ApplicationUserRepository(context); 
			BookTicket = new BookTicketRepository(context); 
		}


		public int Complete()
		{
			return _context.SaveChanges();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
