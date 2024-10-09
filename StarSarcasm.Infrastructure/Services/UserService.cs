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
    }
}
