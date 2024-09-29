using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
	public class BookTicket
	{
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public byte CinemaId { get; set; }
        public Cinema Cinema { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int NumberOfTicket { get; set; }
        public DateTime ShowDate { get; set; }
    }
}
