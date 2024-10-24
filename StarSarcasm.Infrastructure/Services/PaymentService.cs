using Microsoft.AspNetCore.Identity;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Response;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Domain.Entities.Payments;
using StarSarcasm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services
{
	public class PaymentService:IPaymentService
	{
		private readonly Context _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public PaymentService(Context context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<ResponseModel> SavePayment(string userId, PaymentDTO dto)
		{
			try
			{
				var user=await _userManager.FindByIdAsync(userId);
				if (user != null)
				{
					var payment = new Payment
					{
						Id = dto.Id,
						PayerId = dto.PayerId,
						FirstName = dto.FirstName,
						LastName = dto.LastName,
						Email = dto.Email,
						Amount = dto.Amount,
						Currency = dto.Currency,
						Status = dto.Status,
						MerchantId = dto.MerchantId,
						CreateTime = dto.CreateTime,
						UpdateTime = dto.UpdateTime,
						Links = dto.Links,
						UserId = userId
					};
					await _context.Payments.AddAsync(payment);
					await _context.SaveChangesAsync();

					return new ResponseModel
					{
						IsSuccess = true,
						StatusCode = 200,
						Model = dto,
						Message="تم حفظ الدفع بنجاح",
					};
				}

				return new ResponseModel
				{
					IsSuccess = false,
					StatusCode = 404,
					Message = "عذرا، هذا المستخدم غير موجود"
				};
			}
			catch
			{
				return new ResponseModel
				{
					IsSuccess = false,
					StatusCode = 500,
					Message = "حدث خطأ اثناء حفظ الدفع"
				};
			}
		}
	}
}
