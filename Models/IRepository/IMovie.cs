using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.IRepository
{
	public interface IMovie:IGenericRepository<Movie>
	{
		void Update(Movie movie);
		IEnumerable<Movie> Search(string name);
	}
}
