using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.IRepository;
using MoviesAPI.Models;

namespace MoviesAPI.Controllers
{
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	
	public class CategoryController : ControllerBase
	{
		private readonly IUnitOfWork unitOfWork;

		public CategoryController(IUnitOfWork unitOfWork)
        {
			this.unitOfWork = unitOfWork;
		}

		[HttpGet]
		//[AllowAnonymous]
		public IActionResult GetAll()
		{
			var result = unitOfWork.Category.GetAll().OrderBy(c=>c.Name);
			return Ok(result);
		}
		[HttpGet("{id}",Name ="GetById")]
		public IActionResult GetById(int id)
		{
			var result = unitOfWork.Category.GetFirstorDefault(x=>x.Id==id);
			if (result != null)
			{
				return Ok(result);
			}
			else
			{
				return NotFound();
			}
		}
		[HttpPost]
		public IActionResult Create(Category category)
		{
			if (ModelState.IsValid)
			{
			 unitOfWork.Category.Add(category);
				unitOfWork.Complete();
				var url = Url.Link("GetById",new {id=category.Id});
				return Created(url,category);
			}
			else
			{
				return BadRequest(ModelState);
			}
		}
		[HttpPut]
		public IActionResult Update(Category category)
		{
			if (ModelState.IsValid)
			{ 
					unitOfWork.Category.Update(category);
					unitOfWork.Complete();
					return StatusCode(204);
			}
            else
            {
				return BadRequest(ModelState);
            }
        }
		[HttpDelete]
		public IActionResult Delete(int id)
		{
			var result = unitOfWork.Category.GetFirstorDefault(x => x.Id == id);
			if (result == null)
			{
				return NotFound();
			}
			else
			{
				unitOfWork.Category.Remove(result);
				unitOfWork.Complete();
				return Ok(result);
			}
		}
    }
}
