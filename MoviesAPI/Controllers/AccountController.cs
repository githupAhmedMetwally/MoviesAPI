using DataAccess.Implemintation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.DTO;
using Models.Models;
using MoviesAPI.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Security.Cryptography;

namespace MoviesAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IConfiguration config;
		private readonly ApplicationDbContext context;

		public AccountController(UserManager<ApplicationUser> userManager,IConfiguration config,ApplicationDbContext context)
        {
			this.userManager = userManager;
			this.config = config;
			this.context = context;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register(RegisterDTO registerDTO)
		{
			if (ModelState.IsValid)
			{
				var verificationToken = GenerateEmail.GenerateEmailVerificationToken();
				ApplicationUser user = new ApplicationUser();
				user.Email = registerDTO.Email;
				user.UserName=registerDTO.UserName;
				user.EmailVerificationToken = verificationToken;
				var result=await userManager.CreateAsync(user, registerDTO.Password);
				await GenerateEmail.SendVerificationEmail(registerDTO.Email, verificationToken);
				if (result.Succeeded) {
					return Ok("Account Add succeeded .... Please check your email address to verfiy your Account");
				}
				else
				{
                  return BadRequest(result.Errors.FirstOrDefault());
				}
			}
			return BadRequest(ModelState);
		}


		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginDTO loginDTO)
		{
			if (ModelState.IsValid)
			{
				var user =await userManager.FindByNameAsync(loginDTO.UserName);
				if (user != null)
				{
					var result=await userManager.CheckPasswordAsync(user, loginDTO.Password);
					if (result)
					{
						if (!user.EmailConfirmed)
						{
							return BadRequest("Please verify your email before logging in.");
						}

						//create token
						var claims = new List<Claim>();
						claims.Add(new Claim(ClaimTypes.Name, user.UserName));
						claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
						claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // id token

						// role
						var roles = await userManager.GetRolesAsync(user);
						foreach (var item in roles)
						{
							claims.Add(new Claim(ClaimTypes.Role, item));
						}
						SecurityKey security = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"]));  //from string to byte

						SigningCredentials signingCredentials = new SigningCredentials(security, SecurityAlgorithms.HmacSha256);

						JwtSecurityToken myToken = new JwtSecurityToken(
							issuer: config["JWT:ValidIssuer"],
							audience: config["JWT:ValidAudiance"],
							claims: claims,
							expires: DateTime.Now.AddMinutes(1),
							signingCredentials: signingCredentials
							);
						var auth = new Auth();
						auth.Token = new JwtSecurityTokenHandler().WriteToken(myToken);
						auth.RefreshTokenExpiration = myToken.ValidTo;

						if (user.RefreshTokens.Any(x => x.IsActive))
						{
							var activeRefreshToken = user.RefreshTokens.FirstOrDefault(x => x.IsActive);
							auth.RefreshToken = activeRefreshToken.Token;
							auth.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
						}
						else
						{
							var RefreshToken = GetRefreshToken();
							auth.RefreshToken = RefreshToken.Token;
							auth.RefreshTokenExpiration = RefreshToken.ExpiresOn;
							user.RefreshTokens.Add(RefreshToken);
							await userManager.UpdateAsync(user);
						}
						if (!string.IsNullOrEmpty(auth.RefreshToken))
							SetRefreshtokenInCookie(auth.RefreshToken, auth.RefreshTokenExpiration);
						return Ok(auth);
					}
					return Unauthorized();
				}
				else
				{
					return Unauthorized();
				}
			}
			return BadRequest(ModelState);
		}

		[HttpGet]
		[Route("verify")]
		public  IActionResult VerifyEmail(string email, string token)
		{
			// البحث عن المستخدم بواسطة البريد الإلكتروني والرمز
			var user =  context.applicationUsers.Where(u => u.Email == email && u.EmailVerificationToken == token).FirstOrDefault();

			if (user == null)
			{
				return BadRequest("Invalid verification link.");
			}

			// تفعيل الحساب
			user.EmailConfirmed = true;
			user.EmailVerificationToken = null; // إزالة الرمز بعد التحقق
			context.applicationUsers.Update(user);
			context.SaveChanges();

			return Ok("Email verified successfully.");
		}

		[Route("ForgotPassword")]
		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByEmailAsync(model.Email);
				if (user == null)
					return BadRequest("Invalid PayLoad");

				var token = await userManager.GeneratePasswordResetTokenAsync(user);
				if (string.IsNullOrEmpty(token))
					return BadRequest("SomeThing is wrong");

				var url = $"https://localhost:7178/api/Account/ResetPassword?token={token}&email={user.Email}";
 				await GenerateEmail.SendEmailAsync(model.Email, "ResetPassword",
				$"Please reset your password by clicking here: <a href='{url}'>link</a>");

				return Ok("Please check your email to reset your password.");
				 
			}
			return BadRequest("Invalid PayLoad");

		}

		[Route("ResetPassword")]
		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
		{
			if (!ModelState.IsValid)
				return BadRequest("Invalid payload");

			var user = await userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				return BadRequest("Invalid request.");
			}

			var result = await userManager.ResetPasswordAsync(user,model.Token, model.NewPassword);
			if (result.Succeeded)
			{
				return Ok("Password has been reset successfully.");
			}

			return BadRequest("Somthing went wrong");
		}

		[HttpPost("ChangePassword")]
		public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// الحصول على المستخدم الحالي
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // استخراج معرف المستخدم الحالي
			var user = await userManager.FindByIdAsync(userId);

			if (user == null)
				return BadRequest("User not found.");

			// التحقق من أن كلمة المرور الجديدة مطابقة لتأكيد كلمة المرور
			if (model.NewPassword != model.ConfirmNewPassword)
				return BadRequest("The new password and confirmation password do not match.");

			// محاولة تغيير كلمة المرور
			var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

			if (result.Succeeded)
			{
				return Ok("Password has been changed successfully.");
			}

			// التعامل مع الأخطاء المحتملة
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}

			return BadRequest(ModelState);
		}

		[HttpGet("RefreshTokrn")]
		public async Task<IActionResult> RefreshToken()
		{
			var refreshToken = Request.Cookies["refreshToken"];
			var result = await RefreshTokenAsync(refreshToken);

			if (!result.IsAuthenticated)
				return BadRequest(result);
			SetRefreshtokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
			return Ok(result);
		}

		[HttpPost("revoke-token")]
		public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDTO tokenDTO)
		{
			var token = tokenDTO.Token ?? Request.Cookies["refreshToken"];
			if (string.IsNullOrEmpty(token))
				return BadRequest("Token is Required !!!!!!");

			var result = await RevokeTokenAsync(token);
			if (!result)
				return BadRequest("Token is Invalid !!!!!!");

			return Ok("Token has revoked successfully");
		}

		private async Task<Auth> RefreshTokenAsync(string token)
		{
			var auth = new Auth();
			var user = await userManager.Users.SingleOrDefaultAsync(x => x.RefreshTokens.Any(x => x.Token == token));
			if (user == null)
			{
				auth.IsAuthenticated = false;
				auth.messege = "Invalid token";
				return auth;
			}
			var refresh = user.RefreshTokens.Single(x => x.Token == token);
			if (!refresh.IsActive)
			{
				auth.IsAuthenticated = false;
				auth.messege = "Inactive token";
				return auth;
			}

			refresh.RevokedOn = DateTime.UtcNow;
			var newrefreshToken = GetRefreshToken();
			user.RefreshTokens.Add(newrefreshToken);
			await userManager.UpdateAsync(user);

			var claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.Name, user.UserName));
			claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
			claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // id token

			// role
			var roles = await userManager.GetRolesAsync(user);
			foreach (var item in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, item));
			}
			SecurityKey security = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"]));  //from string to byte

			SigningCredentials signingCredentials = new SigningCredentials(security, SecurityAlgorithms.HmacSha256);

			JwtSecurityToken myToken = new JwtSecurityToken(
				issuer: config["JWT:ValidIssuer"],
				audience: config["JWT:ValidAudiance"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(1),
				signingCredentials: signingCredentials
				);
			auth.IsAuthenticated = true;
			auth.Token = new JwtSecurityTokenHandler().WriteToken(myToken);
			auth.RefreshToken = newrefreshToken.Token;
			auth.RefreshTokenExpiration = newrefreshToken.ExpiresOn;
			return auth;
		}
		private async Task<bool> RevokeTokenAsync(string token)
		{
			var auth = new Auth();
			var user = await userManager.Users.SingleOrDefaultAsync(x => x.RefreshTokens.Any(x => x.Token == token));
			if (user == null)
				return false;

			var refresh = user.RefreshTokens.Single(x => x.Token == token);
			if (!refresh.IsActive)
				return false;

			refresh.RevokedOn = DateTime.UtcNow;
			await userManager.UpdateAsync(user);
			return true;
		}
		private RefreshToken GetRefreshToken()
		{
			var randomNumber = new byte[32];
			using var generator = new RNGCryptoServiceProvider();
			generator.GetBytes(randomNumber);
			return new RefreshToken
			{
				Token = Convert.ToBase64String(randomNumber),
				ExpiresOn = DateTime.UtcNow.AddMinutes(1),
				CreatedOn = DateTime.UtcNow
			};
		}
		private void SetRefreshtokenInCookie(string refreshToken,DateTime Expires)
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = Expires.ToLocalTime(),
			};
			Response.Cookies.Append("refreshToken",refreshToken,cookieOptions);
		}

		

		
		
	}
}
