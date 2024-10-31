using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs
{
	public class DrawWithWinnerDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public DateTime StartAt { get; set; }
		public DateTime EndAt { get; set; }
		public string? ImagePath { get; set; }
		public int SubscribersNumber { get; set; }
		public bool IsActive { get; set; }

		public WinnerDTO? User { get; set; }
	}

	public class WinnerDTO
	{
		public string UserId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public DateTime BirthDate { get; set; }
		public DateTime? LastWinDate { get; set; }
		public bool IsSubscribed { get; set; }


	}
}
