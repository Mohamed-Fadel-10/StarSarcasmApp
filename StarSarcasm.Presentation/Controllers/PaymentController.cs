using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Interfaces;

namespace StarSarcasm.Presentation.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentService _paymentService;

		public PaymentController(IPaymentService paymentService)
		{
			_paymentService = paymentService;
		}

		[HttpPost("savePayment")]
		public async Task<IActionResult> SavePayment(string userId,PaymentDTO dto)
		{
			var response=await _paymentService.SavePayment(userId, dto);
			if (response.IsSuccess)
			{
				return StatusCode(response.StatusCode, response.Model);
			}
			return StatusCode(response.StatusCode, response.Message);
		}
	}
}
