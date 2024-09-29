using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
	public class MovieDetialDTO
	{
        public string CategoryName { get; set; }
		public string Name { get; set; }
		 
		public string StoreLine { get; set; }
		public double Rate { get; set; }
		public IFormFile Poster { get; set; }

		public byte CategoryId { get; set; }
	}
}
