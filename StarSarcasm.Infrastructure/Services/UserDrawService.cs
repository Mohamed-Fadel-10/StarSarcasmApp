using Microsoft.EntityFrameworkCore;
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
    public class UserDrawService : IUserDrawService
    {
        private readonly Context _context;

        public UserDrawService(Context context)
        {
            _context = context;
        }

        public async Task<List<UserDrawDTO>> GetAllAsync()
        {
            List<UserDrawDTO> dto = new();
            var userDraws = await _context.UsersDraws.ToListAsync();

            if (userDraws.Any())
            {
                foreach (var userDraw in userDraws)
                {
                    var draw = new UserDrawDTO()
                    {
                        Id = userDraw.Id,
                        IsWinner = userDraw.IsWinner,
                    };
                    dto.Add(draw);
                }
            }
            return dto;
        }

        public async Task<ResponseModel> AddAsync(int drawId, string userId)
        {
            try
            {
                var isInDraw = _context.UsersDraws.Any(ud => ud.DrawId == drawId && ud.UserId == userId);
                if (isInDraw)
                {
                    return new ResponseModel
                    {
                        Message = "أنت بالفعل مشترك في هذا السحب",
                        IsSuccess = false,
                        StatusCode = 400,
                        Model = new
                        {
                            IsDrawSubscribed = true,
                        }
                    };
                }

                var user = await _context.Users.FindAsync(userId);
                if(user == null)
                {
                    return new ResponseModel
                    {
                        Message = "حدث خطأ، مستخدم غير موجود",
                        IsSuccess = false,
                        StatusCode = 404,
                    };
                }

                if (!user.IsSubscribed)
                {
                    return new ResponseModel
                    {
                        Message = "لا يمكنك الاشتراك في السحب لانك غير مشترك في الخدمة",
                        IsSuccess = false,
                        StatusCode = 400,
                        Model = new
                        {
                            IsSubscribed=false,
                        }
                    };
                }

                var draw = _context.Draws.Find(drawId);
                if (draw == null)
                {
                    return new ResponseModel
                    {
                        Message = "هذا السحب غير موجود ولا يمكن الاشتراك به",
                        IsSuccess = false,
                        StatusCode = 404
                    };
                }

                var userDraw = new UsersDraws()
                {
                    DrawId = drawId,
                    UserId = userId,
                };
                await _context.UsersDraws.AddAsync(userDraw);

                draw.SubscribersNumber++;
                _context.Draws.Update(draw);

                await _context.SaveChangesAsync();
                return new ResponseModel
                {
                    Message = "تم الاشتراك بنجاح",
                    IsSuccess = true,
                    StatusCode = 200,
                };
            }
            catch (Exception ex) {

                return new ResponseModel { IsSuccess = false, Message = "حدث خطأ غير متوقع يرجى اعادة المحاولة", StatusCode = 500 };
            }
        }
    }
}
