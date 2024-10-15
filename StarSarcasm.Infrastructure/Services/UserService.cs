using Microsoft.AspNetCore.Identity;
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
    public class UserService : IUserService
    {
        private readonly Context _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(Context context, UserManager<ApplicationUser> _userManager)
        {
            _context = context;
          this._userManager = _userManager;
        }

        public async Task<List<UserDTO>> GetAll()
        {
            var dto = new List<UserDTO>();
            var users = await _context.Users.ToListAsync();

            if (!users.Any())
            {
                return new List<UserDTO>();
            }

            foreach (var user in users)
            {
                var userDto = new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FcmToken = user.FcmToken,
                    IsSubscribed = user.IsSubscribed,
                    BirthDate = user.BirthDate.ToString("yyyy-MM-dd"),
                    Location = user.Location,

                };
                dto.Add(userDto);
            }

            return dto;

        }

        public async Task<List<UserDTO>> GetAllSubscribers()
        {
            var subscribers = await _userManager.Users.Where(u => u.IsSubscribed)
                .ToListAsync();
            List<UserDTO> dto = new();

            if (subscribers.Any())
            {  
                foreach (var subscriber in subscribers)
                {
                    var userDto = new UserDTO
                    {
                        Id = subscriber.Id,
                        UserName = subscriber.UserName,
                        Email = subscriber.Email,
                        FcmToken = subscriber.FcmToken,
                        IsSubscribed = subscriber.IsSubscribed,
                        BirthDate = subscriber.BirthDate.ToString("yyyy-MM-dd"),
                        Location = subscriber.Location,

                    };
                    dto.Add(userDto);
                }
            }
            return dto;
        }

        public async Task<ResponseModel> Profile(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return new ResponseModel { IsSuccess = false, StatusCode = 404, Message = "User Not Found" };
            }
            return new ResponseModel
            {
                IsSuccess = true,
                StatusCode = 200,
                Model =
                new 
                {
                    UserName=user.UserName,
                    BirthDate=user.BirthDate.ToString("yyyy-MM-dd"),
                    Email=user.Email,
                    isSubscribed=user.IsSubscribed,
                    Location=user.Location,
                }
            };
        }
        public async Task<ResponseModel> RemoveUser(string id)
        {
            var user= await _userManager.FindByIdAsync(id);
            if (user == null) {

                return new ResponseModel { IsSuccess = false, Message = "User Not Exist !",StatusCode=404 };
            }
             var result=  await _userManager.DeleteAsync(user);

            return result.Succeeded ? new ResponseModel { IsSuccess = true,StatusCode=200,Message="User Deleted Successfully" } :
                new ResponseModel { 
                    IsSuccess = false, 
                    StatusCode = 400, 
                    Message = "Cannot Delete User" 
            };

        }


        public async Task<ResponseModel> UsersWithZodiac(int zodiacNum)
        {
            List<UserDTO> users = new();
            try
            {
                var zodiacUsers = await _context.Users.ToListAsync();
                if (zodiacUsers.Count > 0)
                {
                    foreach (var user in zodiacUsers)
                    {
                        if (user.BirthDate.Day == zodiacNum)
                        {
                            var dto = new UserDTO
                            {
                                Id = user.Id,
                                UserName = user.Name,
                                Email = user.Email,
                                IsSubscribed = user.IsSubscribed,
                                FcmToken = user.FcmToken,
                                Location = user.Location,
                                BirthDate = user.BirthDate.ToString("yyyy-MM-dd")
                            };
                            users.Add(dto);
                        }
                    }

                    if (users.Count > 0)
                    {
                        return new ResponseModel
                        {
                            IsSuccess = true,
                            StatusCode = 200,
                            Model = users
                        };
                    }
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "لا يوجد مستخدمين بهذا البرج"
                    };
                }

                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "لا يوجد مستخدمين!"
                };

            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = "حدث خطأ غير متوقع يرجى اعادة المحاولة", StatusCode = 500 };
            }

        }
    }
}
