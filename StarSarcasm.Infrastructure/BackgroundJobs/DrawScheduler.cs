using Hangfire;
using StarSarcasm.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.BackgroundJobs
{
	public class DrawScheduler
	{
		private readonly IAwardDrawService _drawService;

		public DrawScheduler(IAwardDrawService drawService)
		{
			_drawService = drawService;
		}

		public void ScheduleDrawEnd()
		{
			RecurringJob.AddOrUpdate("EndDrawIfNoWinner",
				() => _drawService.EndDrawIfNoWinner(),
				Cron.Hourly);
		}
	}
}
