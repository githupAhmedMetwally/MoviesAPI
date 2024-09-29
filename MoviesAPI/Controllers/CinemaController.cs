using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.IRepository;
using Models.Models;

namespace MoviesAPI.Controllers
{
	[Route("api/[controller]")]
	[Authorize(Roles ="Admin")]
	[ApiController]
	public class CinemaController : ControllerBase
	{
		private readonly IUnitOfWork unitOfWork;

		public CinemaController(IUnitOfWork unitOfWork)
        {
			this.unitOfWork = unitOfWork;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult GetAll()
		{
			var result = unitOfWork.Cinema.GetAll();
			return Ok(result);
		}
		[HttpGet("{id}",Name ="GetBycinemaId")]
		public IActionResult GetById(int id)
		{
			var result = unitOfWork.Cinema.GetFirstorDefault(x=>x.Id==id);
			if (result != null)
				return Ok(result);
			else
				return StatusCode(404);
		}
		[HttpGet("{cinemaId}")]
		public IActionResult GetByCinemaId(int cinemaId)
		{
			var result = unitOfWork.Cinema.GetAll(x => x.Id == cinemaId);
			if (result != null)
				return Ok(result);
			else
				return StatusCode(404);
		}
		[HttpPost]
		public IActionResult Create(Cinema cinema)
		{
			if (ModelState.IsValid)
			{
				unitOfWork.Cinema.Add(cinema);
				unitOfWork.Complete();
				var url = Url.Link("GetBycinemaId", new { cinema.Id });
				return Created(url,cinema);
			}
			return BadRequest(ModelState);
		}
		[HttpPut]
		public IActionResult Update(Cinema cinema)
		{
			if (ModelState.IsValid)
			{
				unitOfWork.Cinema.Update(cinema);
				unitOfWork.Complete(); 
				return StatusCode(204);
			}
			return BadRequest(ModelState);
		}

		[HttpDelete]
		public IActionResult Delete(int id)
		{
			var result = unitOfWork.Cinema.GetFirstorDefault(x => x.Id == id);
			if (result != null)
			{
				unitOfWork.Cinema.Remove(result);
				unitOfWork.Complete();
				return Ok(result);
			}
			else
				return StatusCode(404);
		}



	}
}
