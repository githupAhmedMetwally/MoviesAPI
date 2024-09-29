using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
	public class BookTicketDTO
	{
		public string MovieName { get; set; }
		public int MovieId { get; set; }
		public byte CinemaId { get; set; }
		public string CinemaName { get; set; }
        public string UserName { get; set; }
        public DateTime ShowDate { get; set; }
		public int NumberOfTickets { get; set; }
	}
}
