using Models.IRepository;
using Models.Models;
using MoviesAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Implemintation
{
	public class BookTicketRepository : GenericRepository<BookTicket>, IBookTicket
	{
		private readonly ApplicationDbContext context;

		public BookTicketRepository(ApplicationDbContext context) : base(context)
		{
			this.context = context;
		}
	}
}
