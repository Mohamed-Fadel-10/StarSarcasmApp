using Hangfire;
using StarSarcasm.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.BackgroundJobs
{
    public class UserCleanUpScheduler
    {
        public void UserCleanUp()
        {
            RecurringJob.AddOrUpdate<UserCleanUpService>(
            "CleanUnconfirmedUsers",
            service => service.CleanUpUnconfirmedUsersAsync(),
            Cron.Daily);
        }
    }
}
