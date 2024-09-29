using MoviesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.IRepository
{
	public interface ICategory:IGenericRepository<Category>
	{
		void Update(Category category);
		bool handleCategoryId(int CategoryId);
	}
}
