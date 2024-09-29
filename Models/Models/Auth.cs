using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Models
{
	public class Auth
	{
        public string  messege { get; set; }
        public bool  IsAuthenticated { get; set; }
        public string Token { get; set; }
		[JsonIgnore]
		public string RefreshToken { get; set; }
		public DateTime RefreshTokenExpiration { get; set; }
	}
}
