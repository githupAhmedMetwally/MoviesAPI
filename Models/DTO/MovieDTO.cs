using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
	public class MovieDTO
	{
         
        public string Name { get; set; }
		[MaxLength(2500)]
		public string StoreLine { get; set; }
		public double Rate { get; set; }
		public IFormFile? Poster { get; set; }
		public string TrailerUrl { get; set; }
		public double Price { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public byte CategoryId { get; set; }
		public byte cinemaId { get; set; }
	}
}
