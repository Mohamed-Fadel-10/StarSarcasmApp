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
using Twilio.TwiML.Fax;

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
                    Longitude = user.Longitude,
                    Latitude = user.Latitude,
                    Location=user.Location,
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
                        Longitude = subscriber.Longitude,
                        Latitude= subscriber.Latitude,
                        Location= subscriber.Location
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
                    Longitude=user.Longitude,
                    Latitude=user.Latitude,
                    Location=user.Location
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


        private async Task<List<UserDTO>> UsersWithZodiac(int zodiacNum)
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
                                Id=user.Id,
                                UserName=user.UserName,
                                Email=user.Email,
                                FcmToken=user.FcmToken,
                                IsSubscribed=user.IsSubscribed,
                                BirthDate=user.BirthDate.ToString("yyyy/MM/dd"),
                                Longitude=user.Longitude,
                                Latitude=user.Latitude,
                                Location=user.Location,
                            };
                            users.Add(dto);
                        }
                    }
                }
                return users;

            }
            catch (Exception ex)
            {
                return new List<UserDTO>();
            }

        }

		public async Task<ResponseModel> UpdateAsync(string id, UserDTO dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if(user != null)
                {
                    var newEmail=await _userManager.FindByEmailAsync(dto.Email);
                    var newUserName=await _userManager.FindByNameAsync(dto.UserName);

					if (newEmail!=user && newEmail != null)
                    {
						return new ResponseModel
						{
							IsSuccess = false,
							StatusCode = 400,
							Message = "البريد الالكتروني مستخدم من قبل"
						};
					}


					if (newUserName!=user && newUserName != null)
					{
						return new ResponseModel
						{
							IsSuccess = false,
							StatusCode = 400,
							Message = "هذا الاسم مستخدم من قبل"
						};
					}

					var newUser = new ApplicationUser
                    {
                        Id = id,
                        Name = dto.UserName,
                        UserName = dto.UserName,
                        FcmToken = user.FcmToken,
                        Longitude = dto.Longitude,
                        Latitude= dto.Latitude,
                        BirthDate = DateTime.Parse(dto.BirthDate),
                        Email = dto.Email,
                        IsSubscribed = user.IsSubscribed
                    };

                    _context.Entry(user).CurrentValues.SetValues(newUser);
                    var result = _context.Entry(user);
                    if (result.State != EntityState.Modified)
                    {
                        return new ResponseModel
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "تعذر تعديل البيانات"
                        };
                    }

                    await _context.SaveChangesAsync();
                    return new ResponseModel
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Model = dto,
						Message = "تم تعديل البيانات بنجاح"
					};
                }
				return new ResponseModel
				{
					IsSuccess = false,
					StatusCode = 404,
					Message = "هذا المستخدم غير موجود"
				};
			}
            catch
            {
				return new ResponseModel
				{
					IsSuccess = false,
					StatusCode = 500,
					Message = "حدث خطأ اثناء تعديل البيانات"
				};
			}
        }

        public async Task<ResponseModel> NearestUsersInZodiac(string userId, int zodiacNum)
        {
            var currentUser=await _userManager.FindByIdAsync(userId);
            if (currentUser != null)
            {
                var users = await UsersWithZodiac(zodiacNum);
                if (users.Count > 0)
                {
                    List<UserChatDTO> nearestUsers = new();
                    foreach(var user in users)
                    {
                        if (user.Id == userId)
                        {
                            // not return the current user in the list
                            continue;
                        }
                        var distance=await CalculateDistance(currentUser.Latitude,currentUser.Longitude
                            ,user.Latitude,user.Longitude);

                        var userChat = new UserChatDTO
                        {
                            ChatId = null,
                            ChatName = user.UserName,
                            ReceiverDate = DateTime.Parse(user.BirthDate),
                            ReceiverId = user.Id,
                            FcmToken = user.FcmToken,
                            Longitude = user.Longitude,
                            Latitude = user.Latitude,
                            Distance = distance,
                            Location = user.Location
                        };
                        nearestUsers.Add(userChat);
                        
                    }

                    return new ResponseModel
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Model = nearestUsers.OrderBy(u=>u.Distance)
                    };
                }

                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 204,
                    Message = "لا يوجد اشخاص يحملون نفس هذا البرج"
                };
            }

            return new ResponseModel
            {
                IsSuccess = false,
                StatusCode = 404,
                Message = "هذا المستخدم غير موجود"
            };
        }

        private async Task<double> CalculateDistance(double lat1,double long1,double lat2,double long2)
        {
            var earthRadius = 6371; // earth Radius with  km

            var dLatitude = ToRadians(lat2 - lat1); // difference between lat1 and lat2 and covert it to radians
            var dLongitude = ToRadians(long2 - long1);

            //haversine
            var dSin = Math.Sin(dLatitude / 2) * Math.Sin(dLatitude / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2))
                * Math.Sin(dLongitude / 2) * Math.Sin(dLongitude / 2);
            var rDistance = 2 * Math.Atan2(Math.Sqrt(dSin), Math.Sqrt(1 - dSin));

            var distanceWithKM = earthRadius * rDistance;
            return distanceWithKM;
        }

        private double ToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }
    }
}
