
using DataAccess.Implemintation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models.IRepository;
using Models.Models;
using MoviesAPI.Data;
using System.Text;


namespace MoviesAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();

			builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
	builder.Configuration.GetConnectionString("DefaultConnection")
	));
			builder.Services.AddIdentity<ApplicationUser, IdentityRole>().
			   AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
			builder.Services.AddAuthentication(options =>
			{       // token??????? ??? ??
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;   // ???? ??? ??token????? ??
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;    //login?? ?? ???? ?????? ??? ???? ??token????? ??
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(op =>
			{
				op.SaveToken = true;  //??? ????? ????
				op.RequireHttpsMetadata = false;
				op.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
					ValidateAudience = true,
					ValidAudience = builder.Configuration["JWT:ValidAudiance"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
					ClockSkew = TimeSpan.Zero     //??? ?????? ???? ????? ????? ???????? ?????
				};

			});
			builder.Services.AddCors();
			builder.Services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo
				{
					Version="v1",
					Title="MovieAPI",
					Description="My First API Project",
					Contact=new OpenApiContact
					{ 
					   Name="Ahmed",
					   Email="ahmedmetwallyhassan@gmail.com", 
					}
				});
				    //create button
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Enter 'Bearer' [space] and then your valid token in the next input"
				});
				      //put button in all controllers
				options.AddSecurityRequirement(new OpenApiSecurityRequirement
					{
						{
							new OpenApiSecurityScheme
						{
						 Reference=new OpenApiReference
						 {
							 Type=ReferenceType.SecurityScheme,
							 Id="Bearer"
							  },
						 Name="Bearer",
						 In=ParameterLocation.Header,
						   },
							new List<string>()
						}
					});
			});
			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
