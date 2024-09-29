using MoviesAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
	public class Movie
	{
        public int Id { get; set; }
		[MaxLength(250)]
		public string Name { get; set; }
		[MaxLength(2500)]
		public string StoreLine { get; set; }
        public double Rate { get; set; }
        public byte[] Poster { get; set; }
		public string TrailerUrl { get; set; }
        public double Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
		public byte CinemaId { get; set; }
		public Cinema Cinema { get; set; }
		public byte CategoryId { get; set; }
        public Category Category { get; set; }
		
	}
}
