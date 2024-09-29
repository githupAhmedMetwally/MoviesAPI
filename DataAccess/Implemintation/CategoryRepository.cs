using Microsoft.EntityFrameworkCore;
using Models.IRepository;
using MoviesAPI.Data;
using MoviesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Implemintation
{
	public class CategoryRepository : GenericRepository<Category>, ICategory
	{
		private readonly ApplicationDbContext context;

		public CategoryRepository(ApplicationDbContext context) : base(context)
		{
			this.context = context;
		}

		public bool handleCategoryId(int categoryId)
		{
			var result = context.categories.Any(x => x.Id == categoryId);
			return result;
		}

		public void Update(Category category)
		{
			var CategoryInDb = context.categories.FirstOrDefault(x => x.Id == category.Id);
			if (CategoryInDb != null)
			{
				CategoryInDb.Name = category.Name; 
			}
		}
	}
}
