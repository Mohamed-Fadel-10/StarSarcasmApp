using StarSarcasm.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs
{
	public class PaymentDTO
	{
		public string Id { get; set; }
		public string PayerId { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Email { get; set; }
		public string? Amount { get; set; }
		public string? Currency { get; set; }
		public string? Status { get; set; }
		public string? MerchantId { get; set; }
		public DateTime? CreateTime { get; set; }
		public DateTime? UpdateTime { get; set; }
		public List<LinkModel>? Links;
	}
}
