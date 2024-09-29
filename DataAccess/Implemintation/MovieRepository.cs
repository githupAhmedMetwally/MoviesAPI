using Microsoft.EntityFrameworkCore;
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
	public class MovieRepository : GenericRepository<Movie>, IMovie
	{
		private readonly ApplicationDbContext context;

		public MovieRepository(ApplicationDbContext context) : base(context)
		{
			this.context = context;
		}

		public IEnumerable<Movie> Search(string name)
		{
			var movies=context.movies.Where(x=>x.Name.Contains(name)).Include(x=>x.Cinema).Include(x=>x.Category).ToList();
			return movies;
		}

		public void Update(Movie movie)
		{
			context.movies.Update(movie);
		}
	}
}
