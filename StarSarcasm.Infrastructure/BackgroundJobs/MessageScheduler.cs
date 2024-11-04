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
            _messageService = messageService;
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
                () => _messageService.SendMessagesToSubscribedUsers(),
                "0 8,12,16,20,23 * * *");
        }


        // Test Method
        //public void ScheduleMessagesForSubscribedUsersTest()
        //{
        //    RecurringJob.AddOrUpdate("SendMessagesToSubscribedUsersTest",
        //        () => _messageService.SendMessagesToSubscribedUsers(),
        //        "*/2 * * * *");
        //}




    }
}
