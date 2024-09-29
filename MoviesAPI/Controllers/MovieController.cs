using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.IRepository;
using Models.Models;

namespace MoviesAPI.Controllers
{
	[Route("api/[controller]")]
	[Authorize(Roles ="Admin")]
	[ApiController]
	public class MovieController : ControllerBase
	{
		private readonly IUnitOfWork unitOfWork;
		private new List<string> allwedExtention = new List<string> { ".jpg", ".png" };
		private long maxSize = 1048576;
		public MovieController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}
		[HttpGet]
		[AllowAnonymous]
		public IActionResult GetAll()
		{
			var result = unitOfWork.Movie.GetAll(Includeword: "Category,Cinema");
			return Ok(result);
		}
		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{ 
			var result = unitOfWork.Movie.GetFirstorDefault(x=>x.Id==id,Includeword:"Category,Cinema"); 
			if (result != null)
				return Ok(result);
			else
				return NotFound();
		}
		[HttpGet("GetAllMovieInThisCategory/{categoryId}")]
		public IActionResult GetByCategoryId(byte categoryId)
		{
			var result = unitOfWork.Movie.GetAll(x => x.CategoryId == categoryId,Includeword:"Category,Cinema");
			if (result.Count()==0)
				return StatusCode(404);
			return Ok(result);
		}
		[HttpGet("GetAllMovieInThisCinema/{cinemaId}")]
		public IActionResult GetByCinemaId(int cinemaId)
		{
			var result = unitOfWork.Movie.GetAll(x => x.CinemaId == cinemaId,Includeword:"Cinema,Category");
			if (result.Count() == 0)
				return StatusCode(404);
			return Ok(result);
		}
		[HttpGet("search/{name}")]
		public IActionResult Search(string name)
		{
			var result = unitOfWork.Movie.Search(name);
			if (result.Count() == 0)
				return StatusCode(404);
			return Ok(result);
		}
		[HttpPost]
		public IActionResult Create([FromForm]MovieDTO movieDTO)
		{
			if (movieDTO.Poster == null)
				return BadRequest("Poster is Required!!!!!!");
			if (!allwedExtention.Contains(Path.GetExtension(movieDTO.Poster.FileName).ToLower()))
				return BadRequest("Only .png and .jpg imges are aalwed");
			if (movieDTO.Poster.Length > maxSize)
				return BadRequest("max allwed size for poster is 1MB");

			var isValidCategory = unitOfWork.Category.handleCategoryId(movieDTO.CategoryId);
			if (!isValidCategory)
				return BadRequest("invalid category id");

			using var dataStream=new MemoryStream();
			movieDTO.Poster.CopyTo(dataStream);
			if (ModelState.IsValid)
			{
				Movie movie = new Movie()
				{
                   Name=movieDTO.Name,
				   Rate=movieDTO.Rate,
				   CategoryId=movieDTO.CategoryId,
				   Poster=dataStream.ToArray(),
				    StoreLine=movieDTO.StoreLine,
					Price=movieDTO.Price,
					StartDate=movieDTO.StartDate,
					EndDate=movieDTO.EndDate,
					TrailerUrl=movieDTO.TrailerUrl,
					CinemaId=movieDTO.cinemaId
				};
				unitOfWork.Movie.Add(movie);
				unitOfWork.Complete();
				return Created();
			}
			return BadRequest(ModelState);
		}
		[HttpPut("{id}")]
		public IActionResult Update(int id,[FromForm]MovieDTO movieDTO) 
		{
			if (ModelState.IsValid)
			{
				var movie = unitOfWork.Movie.GetFirstorDefault(x=>x.Id==id);
				if (movie == null)
					return NotFound($"no movie was found with id {id} ");


				var isValidCategory = unitOfWork.Category.handleCategoryId(movieDTO.CategoryId);
				if (!isValidCategory)
					return BadRequest("invalid category id");

				if (movieDTO.Poster != null)
				{
					if (!allwedExtention.Contains(Path.GetExtension(movieDTO.Poster.FileName).ToLower()))
						return BadRequest("Only .png and .jpg imges are aalwed");
					if (movieDTO.Poster.Length > maxSize)
						return BadRequest("max allwed size for poster is 1MB");
					using var dataStream = new MemoryStream();
					movieDTO.Poster.CopyTo(dataStream);

					movie.Poster = dataStream.ToArray();
				}

				movie.Name = movieDTO.Name;
				movie.CategoryId = movieDTO.CategoryId;
				movie.CinemaId = movieDTO.cinemaId;
				movie.StoreLine = movieDTO.StoreLine;
				movie.Rate = movieDTO.Rate;
				movie.Price = movieDTO.Price;
				movie.StartDate = movieDTO.StartDate;
				movie.EndDate = movieDTO.EndDate;
				movie.TrailerUrl = movieDTO.TrailerUrl;
				unitOfWork.Complete();
				return Ok();
			}
			else
			{
				return BadRequest(ModelState);
			}
 		}

		[HttpDelete]
		public IActionResult Delete(int id)
		{
			var result = unitOfWork.Movie.GetFirstorDefault(x => x.Id == id);
			if (result == null)
				return NotFound();

			unitOfWork.Movie.Remove(result);
			unitOfWork.Complete();
			return Ok(result);

		}
	}
}
