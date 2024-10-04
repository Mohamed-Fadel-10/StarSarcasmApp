using Hangfire;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.BackgroundJobs
{
    public class MessageScheduler
    {
        private readonly IMessageService _messageService;
        public MessageScheduler(IMessageService messageService)
        {
            _messageService=messageService;
        }

        public void ScheduleMessagesForUnsubscribedUsers()
        {
            RecurringJob.AddOrUpdate("SendMessagesToUnsubscribed",
                () => _messageService.SendMessagesToUnSubscribedUsers(),
                "0 9,17 * * *"); 
        }

        public void ScheduleMessagesForSubscribedUsers()
        {
            RecurringJob.AddOrUpdate("SendMessagesToSubscribed",
                () => _messageService.SendMessagesToUnSubscribedUsers(),
                "0 8,12,16,20,23 * * *"); 
        }

        public void ScheduleMessagesForSubscribedUsersTest()
        {
            // The cron expression for every 2 minutes: */2 * * * *
            RecurringJob.AddOrUpdate("SendMessagesToSubscribedUsersTest",
                () => _messageService.SendMessagesToUnSubscribedUsers(),
                "*/2 * * * *");  // Cron expression as a string for every 2 minutes
        }


    }
}
