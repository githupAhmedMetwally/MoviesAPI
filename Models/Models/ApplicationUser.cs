using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Models
{
	public class ApplicationUser:IdentityUser
	{
		public string EmailVerificationToken { get; set; }

        public List<RefreshToken>? RefreshTokens { get; set; }
	}
}
