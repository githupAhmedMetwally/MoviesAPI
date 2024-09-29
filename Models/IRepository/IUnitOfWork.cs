using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.IRepository
{
	public interface IUnitOfWork:IDisposable
	{
		ICategory Category { get; }
		IMovie Movie { get; }
		ICinema Cinema { get; }
		IApplicationUser ApplicationUser { get; }
		IBookTicket BookTicket { get; }
 
		int Complete();
	}
}
