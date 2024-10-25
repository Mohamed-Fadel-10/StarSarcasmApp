using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Interfaces.IFileUploadService;
using StarSarcasm.Application.Response;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services
{
    public class AwardDrawService:IAwardDrawService
    {
        private readonly Context _context;
        private readonly IUserService _userService;
        private readonly IUserDrawService _userDrawService;
        private readonly IFileUploadService _fileUpload;

        public AwardDrawService(Context context, IUserService userService, IUserDrawService userDrawService, IFileUploadService fileUpload)
        {
            _context = context;
            _userService = userService;
            _userDrawService = userDrawService;
            _fileUpload = fileUpload;
        }

        public async Task<ResponseModel> GetActiveDrawAsync()
        {
            var draw = await _context.Draws
             .FirstOrDefaultAsync(d =>
              DateTime.Now >= d.StartAt && DateTime.Now <= d.EndAt);

            if (draw != null)
            {
                
                return new ResponseModel { IsSuccess=true ,Model= draw, StatusCode=200};
            }
            return new ResponseModel { IsSuccess = false, StatusCode = 404,Message="No Draws Available Now" };
        }

        public async Task<ResponseModel> AddAsync(DrawDTO dto)
        {
            if (dto != null)
            {
                string filename = string.Empty;

                if (dto.file != null)
                {
                    filename = await _fileUpload.SaveFileAsync(dto.file);
                }

                string imagePath = string.IsNullOrEmpty(filename) ? null : _fileUpload.GetFilePath(filename);

                var draw = new Draw()
                {
                    Name = dto.Name,
                    Description = dto.Description ?? string.Empty,
                    StartAt = dto.StartAt,
                    EndAt = dto.EndAt,
                    ImagePath = imagePath,
                    SubscribersNumber=0
                };

                await _context.Draws.AddAsync(draw);
                await _context.SaveChangesAsync();

                return new ResponseModel
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Model = new
                    {
                        Draw = draw,
                        Message = "Draw Added Successfully"
                    }
                };
            }

            return new ResponseModel { IsSuccess = false, StatusCode = 400, Message = "Invalid Data" };
        }


		public async Task<ResponseModel> RandomDrawWinner(int drawId)
        {
            try
            {

                var oneMonthAgo = DateTime.Now.AddMonths(-1);
                Random random = new();

                var allSubscribers = await _context.UsersDraws
                    .Where(ud => ud.DrawId == drawId)
                    .ToListAsync();

                if (!allSubscribers.Any())
                {
                    return new ResponseModel
                    {
                        Message = "لا يوجد مشاركين في هذا السحب",
                        IsSuccess = false,
                        StatusCode = 404,
                    };
                }

                var drawSubscribers = allSubscribers.Where(u => u.IsWinner == false
                            || u.LastWinDate <= oneMonthAgo).ToList();

                if (!drawSubscribers.Any())
                {
                    drawSubscribers = allSubscribers;
                }

                var winnerIndex = random.Next(drawSubscribers.Count);
                var winner = drawSubscribers[winnerIndex];
                winner.IsWinner = true;
                winner.LastWinDate = DateTime.Now;
                _context.UsersDraws.Update(winner);
                _context.SaveChanges();

                var user = await _context.Users.FindAsync(winner.UserId);
                return new ResponseModel
                {
                    Model = new UserDTO
                    {
                        Id = user.Id,
                        UserName = user.Name,
                        Email = user.Email,
                        IsSubscribed = user.IsSubscribed,
                        FcmToken = user.FcmToken,
                        Longitude = user.Longitude,
                        Latitude= user.Latitude,
                        BirthDate = user.BirthDate.ToString("yyyy/mm/dd")
                    },
                    Message = "مبارك للفائز ",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = "حدث خطأ غير متوقع يرجى اعادة المحاولة", StatusCode = 500 };

            }

        }

		public async Task<ResponseModel> UpdateAsync(int id,DrawDTO dto)
        {
            try
            {
                var draw = await _context.Draws.FindAsync(id);
                if( draw == null)
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "هذا السحب غير موجود",
                    };
                }

				string filename = string.Empty;
				if (dto.file != null)
				{
					filename = await _fileUpload.SaveFileAsync(dto.file);
				}
				string imagePath = string.IsNullOrEmpty(filename) ? null : _fileUpload.GetFilePath(filename);

				var newDraw = new Draw
                {
                    Id = draw.Id,
                    Name = dto.Name,
                    Description = dto.Description,
                    StartAt = dto.StartAt,
                    EndAt = dto.EndAt,
                    ImagePath = imagePath,
                };

                _context.Entry(draw).CurrentValues.SetValues(newDraw);
                var result = _context.Entry(draw);
                if (result.State == EntityState.Modified)
                {
                    await _context.SaveChangesAsync();
                    return new ResponseModel
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Model = draw,
						Message = "تم التعديل بنجاح",
					};
                }

                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "حدث خطأ اثناء تعديل السحب",
                };
            }
            catch
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "حدث خطأ اثناء تعديل السحب",
                };
            }
        }

		public async Task<ResponseModel> DeleteAsync(int id)
        {
            try
            {
                var draw = await _context.Draws.FindAsync(id);
                if (draw != null)
                {
                    _context.Draws.Remove(draw);
                    var result = _context.Entry(draw);
                    if(result.State== EntityState.Deleted)
                    {
                        await _context.SaveChangesAsync();
                        return new ResponseModel
                        {
                            IsSuccess = true,
                            StatusCode = 200,
                            Model = draw
                        };
                    }

					return new ResponseModel
					{
						IsSuccess = false,
						StatusCode = 400,
						Model = "تعذر حذف السحب",
					};
				}

				return new ResponseModel
				{
					IsSuccess = false,
					StatusCode = 400,
					Model = "السحب غير موجود بالفعل",
				};
			}
            catch
            {
				return new ResponseModel
				{
					IsSuccess = false,
					StatusCode = 500,
					Model = "حدث خطأ اثناء حذف السحب",
				};
			}
        }

        public async Task<ResponseModel> GetAllSubscribers(int id)
        {
            try
            {
                var all = await _context.UsersDraws.Include(d => d.Draw)
                    .Include(d=>d.User)
                    .Where(d => d.DrawId == id).Select(d=>d.User).ToListAsync();
                if (all.Any())
                {
                    List<UserDTO> users = new();
                    foreach (var user in all)
                    {
                        var dto = new UserDTO
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            IsSubscribed = user.IsSubscribed,
                            FcmToken = user.FcmToken,
                            BirthDate = user.BirthDate.ToString("yyyy/MM/dd"),
                            Location = user.Location,
                            Longitude = user.Longitude,
                            Latitude = user.Latitude,
                        };
                        users.Add(dto);
                    }

                    return new ResponseModel
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Model = users
                    };
                }
                return new ResponseModel
                {
                    IsSuccess = true,
                    StatusCode = 204,
                    Message="لا يوجد مشتركين في السحب"
                };
            }
            catch
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "حدث خطأ، يرجى المحاولة في وقت لاحق"
                };
            }
        }

    }

}
