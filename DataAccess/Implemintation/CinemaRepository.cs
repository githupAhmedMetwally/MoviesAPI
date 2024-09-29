using Models.IRepository;
using Models.Models;
using MoviesAPI.Data;
using MoviesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Implemintation
{
	public class CinemaRepository : GenericRepository<Cinema>, ICinema
	{
		private readonly ApplicationDbContext context;

		public CinemaRepository(ApplicationDbContext context) : base(context)
		{
			this.context = context;
		}

		public void Update(Cinema cinema)
		{
			var cinemaInDb = context.cinemas.FirstOrDefault(x => x.Id == cinema.Id);
			if (cinemaInDb != null)
			{
				cinemaInDb.Name = cinema.Name;
				cinemaInDb.Address= cinema.Address;
			}
		}
	}
}
