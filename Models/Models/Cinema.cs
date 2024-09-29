using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
	public class Cinema
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte Id { get; set; }
		[MaxLength(100)]
        public string Name { get; set; }
		public string Address { get; set; }
    }
}
