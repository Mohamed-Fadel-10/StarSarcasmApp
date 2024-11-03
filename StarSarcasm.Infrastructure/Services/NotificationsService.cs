using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Response;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services
{
    public class NotificationsService: INotificationsService
    {
        private readonly Context _context;
        public NotificationsService(Context context)
        {
            _context = context;
        }
        public async Task<ResponseModel> SaveNotification(NotificationDTO model)
        {
            try
            {
                if (model == null)
                {
                    return new ResponseModel()
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Invalid Model"
                    };

                }
                var notification = new Notification()
                {
                    Title = model.Title,
                    Content = model.Content,
                    UserId = model.UserId,
                };
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
                return new ResponseModel()
                {
                    IsSuccess = true,
                    StatusCode = 201,
                    Message = "تم حفظ الاشعار بنجاح"
                };
            }
            catch (Exception ex) {

                return new ResponseModel()
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "internal Server Error"
                };

            }
       }

    }
}
