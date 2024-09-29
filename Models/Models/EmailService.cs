using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
		public class EmailService
		{
			public async Task SendEmailAsync(string email, string subject, string message)
			{
				var mailMessage = new MailMessage("your-email@example.com", email, subject, message);
				mailMessage.IsBodyHtml = true;

				using (var client = new SmtpClient("smtp.your-email-provider.com"))
				{
					client.Credentials = new NetworkCredential("your-email@example.com", "your-email-password");
					client.EnableSsl = true;
					await client.SendMailAsync(mailMessage);
				}
			}
		}

	}

