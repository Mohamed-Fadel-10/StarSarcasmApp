using StarSarcasm.Application.DTOs.Message;
using StarSarcasm.Application.Response;
using StarSarcasm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.Interfaces
{
   public interface IMessageService
    {
        public Task<List<Message>> GetRandomMessages(int count);
        public  Task SendMessagesToSubscribedUsers();
        public  Task SendMessagesToUnSubscribedUsers();
        public Task<List<MessageDTO>> GetUserMessages(string id);


    }
}
