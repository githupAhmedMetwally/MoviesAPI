using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Models.Models
{
	public class GenerateEmail
	{
		public static string GenerateEmailVerificationToken()
		{
			return Guid.NewGuid().ToString();  // رمز تحقق فريد
		}
		public static async Task SendVerificationEmail(string email, string token)
		{
			var verificationLink = $"https://localhost:7178/api/Account/verify?email={email}&token={token}";
			var fromEmail = new MailAddress("ahmedmetwallyhassan@gmail.com", "MovieAPIWeb");
			var toEmail = new MailAddress(email);

			var message = new MailMessage
			{
				From = fromEmail,
				Subject = "Email Verification",
				Body = $"Please click the following link to verify your email: <a href='{verificationLink}'>Verify Email</a>",
				IsBodyHtml = true
			};
			message.To.Add(toEmail);

			using (var smtpClient = new SmtpClient("smtp.gmail.com"))
			{
				smtpClient.Port = 587;
				smtpClient.Credentials = new NetworkCredential("ahmedmetwallyhassan@gmail.com", "uxzf qpqy bzay uquc");
				smtpClient.EnableSsl = true;
				await smtpClient.SendMailAsync(message);
			}
		}
		public static async Task SendEmailAsync(string email, string subject, string message)
		{
			var mailMessage = new MailMessage("ahmedmetwallyhassan@gmail.com", email, subject, message);
			mailMessage.IsBodyHtml = true;

			using (var client = new SmtpClient("smtp.gmail.com"))
			{
			    client.Port = 587;
				client.Credentials = new NetworkCredential("ahmedmetwallyhassan@gmail.com", "uxzf qpqy bzay uquc");
				client.EnableSsl = true;
				await client.SendMailAsync(mailMessage);
			}
		}

	}
}
