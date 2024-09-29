using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.IRepository;
using Models.Models;
using System.Security.Claims;

namespace MoviesAPI.Controllers
{
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	public class BookTicketController : ControllerBase
	{
		private readonly IUnitOfWork unitOfWork;

		public BookTicketController(IUnitOfWork unitOfWork)
        {
			this.unitOfWork = unitOfWork;
		}
		[HttpGet]
		[Authorize(Roles ="Admin")]
		public IActionResult GetAll()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var AllTicket=unitOfWork.BookTicket.GetAll(Includeword:"Cinema,Movie")
				.Select(m=>new BookTicketDTO
				{
					UserName = claim.Value,
					MovieName =m.Movie.Name,
					CinemaName=m.Cinema.Name, 
					MovieId=m.MovieId,
					CinemaId=m.CinemaId,
					NumberOfTickets=m.NumberOfTicket,
					 ShowDate=m.ShowDate
				});
			return Ok(AllTicket);
		}

		[HttpPost]
		public IActionResult SaveTicket(BookTicketDTO ticketDTO)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			
			var movie = unitOfWork.Movie.GetFirstorDefault(m => m.Id == ticketDTO.MovieId);
			var cinema=unitOfWork.Cinema.GetFirstorDefault(c => c.Id == ticketDTO.CinemaId);

			if (movie == null || cinema == null)
				return BadRequest("Invalid movie or cinema.");
			if (claim.Value == null)
				return NotFound();

			BookTicket bookTicket = new BookTicket
			{
				MovieId = ticketDTO.MovieId,
				CinemaId = ticketDTO.CinemaId,
				ApplicationUserId = claim.Value,
				NumberOfTicket = ticketDTO.NumberOfTickets,
				ShowDate = ticketDTO.ShowDate
			};
			unitOfWork.BookTicket.Add(bookTicket);
			unitOfWork.Complete();
			return Created();
		}

    }
}
