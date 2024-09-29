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
	public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUser
	{
		private readonly ApplicationDbContext context;

		public ApplicationUserRepository(ApplicationDbContext context) : base(context)
		{
			this.context = context;
		}
	}
}
