using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using StarSarcasm.Application;
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
        private readonly FirebaseNotificationService _firebaseNotificationService;

        public AwardDrawService(Context context, IUserService userService,
            IUserDrawService userDrawService, IFileUploadService fileUpload, FirebaseNotificationService firebaseNotificationService)
        {
            _context = context;
            _userService = userService;
            _userDrawService = userDrawService;
            _fileUpload = fileUpload;
            _firebaseNotificationService = firebaseNotificationService;
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
            return new ResponseModel { IsSuccess = false, StatusCode = 404,Message="لا يوجد اي سحب حاليا" };
        }

        public async Task<ResponseModel> AddAsync(DrawDTO dto)
        {
            if(_context.Draws.Any(d=> DateTime.Now >= d.StartAt && DateTime.Now <= d.EndAt))
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "لا يمكنك اضافة سحب حتى ينتهي الموجود حاليا"
                };
            }

            if (dto == null)
            {
                return new ResponseModel { IsSuccess = false, StatusCode = 400, Message = "Invalid Data" };
            }

            string imagePath = null;

            if (dto.file != null)
            {
                try
                {
                    imagePath = await _fileUpload.SaveFileAsync(dto.file, "DrawImages");
                }
                catch (Exception)
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        StatusCode = 500,
                        Message = "Error occurred while saving the image. Please try again later."
                    };
                }
            }

            var draw = new Draw
            {
                Name = dto.Name,
                Description = dto.Description ?? string.Empty,
                StartAt = dto.StartAt.ToUniversalTime(),
                EndAt = dto.EndAt.ToUniversalTime(),
                ImagePath = imagePath,
                SubscribersNumber = 0
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
                    Message = "تم اضافة السحب بنجاح"
                }
            };
        }


        public async Task<ResponseModel> RandomDrawWinner(int drawId)
        {
            try
            {
                var draw = await _context.Draws
                    .Include(u=>u.UsersDraws)
                    .FirstOrDefaultAsync(d=>d.Id==drawId);
                if (draw == null)
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "هذا السحب غير موجود"
                    };
                }

                if (draw.UsersDraws.Any(ud => ud.IsWinner))
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "تم إعلان الفائز لهذا السحب من قبل"
                    };
                }

                var allSubscribers = await _context.UsersDraws
                    .Where(ud => ud.DrawId == drawId)
                    .ToListAsync();

                if (!allSubscribers.Any())
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "لا يوجد مشاركين في هذا السحب"
                    };
                }

                var oneMonthAgo = DateTime.Now.AddMonths(-1);
                var eligibleSubscribers = allSubscribers
                    .Where(u => !u.IsWinner || u.LastWinDate <= oneMonthAgo)
                    .ToList();

                if (!eligibleSubscribers.Any())
                {
                    eligibleSubscribers = allSubscribers;
                }

                Random random = new();
                var winnerIndex = random.Next(eligibleSubscribers.Count);
                var winner = eligibleSubscribers[winnerIndex];
                winner.IsWinner = true;
                winner.LastWinDate = DateTime.UtcNow;
                draw.EndAt = DateTime.UtcNow;
                var user = await _context.Users.Include(u=>u.RefreshTokens)
                    .FirstOrDefaultAsync(u=>u.Id==winner.UserId);

                _context.UsersDraws.Update(winner);
                _context.Draws.Update(draw);
                await _context.SaveChangesAsync();

                if (user.RefreshTokens!.Any(r => r.IsActive))
                {
                    await _firebaseNotificationService.SendNotificationAsync(
                        user.FcmToken,
                        "الجوائز",
                        "تهانينا! لقد فزت معنا في السحب 🎉"
                    );
                }

                return new ResponseModel
                {
                    Model = new UserDTO
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        IsSubscribed = user.IsSubscribed,
                        FcmToken = user.FcmToken,
                        Longitude = user.Longitude,
                        Latitude = user.Latitude,
                        BirthDate = user.BirthDate.ToString("yyyy-MM-dd")
                    },
                    Message = "مبارك للفائز!",
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "حدث خطأ غير متوقع، يرجى إعادة المحاولة لاحقاً"
                };
            }
        }

        public async Task<ResponseModel> UpdateAsync(int id, UpdateDrawDTO dto)
        {
            try
            {
                var draw = await _context.Draws.FindAsync(id);
                if (draw == null)
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "هذا السحب غير موجود",
                    };
                }

                if (!string.IsNullOrEmpty(dto.Name))
                {
                    draw.Name = dto.Name;
                }
                if (!string.IsNullOrEmpty(dto.Description))
                {
                    draw.Description = dto.Description;
                }
                if (dto.StartAt.HasValue)
                {
                    draw.StartAt = dto.StartAt.Value;
                }
                if (dto.EndAt.HasValue)
                {
                    draw.EndAt = dto.EndAt.Value;
                }

                if (dto.file != null)
                {
                    draw.ImagePath = await _fileUpload.SaveFileAsync(dto.file);
                }

                await _context.SaveChangesAsync();

                return new ResponseModel
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Model = draw,
                    Message = "تم التعديل بنجاح",
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
                            Name = user.Name,
                            Email = user.Email,
                            IsSubscribed = user.IsSubscribed,
                            FcmToken = user.FcmToken,
                            BirthDate = user.BirthDate.ToString("yyyy-MM-dd"),
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

        public async Task<ResponseModel> GetAll()
        {
            var draws = await _context.Draws
                .Select(d => new DrawWithWinnerDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    StartAt = d.StartAt,
                    EndAt = d.EndAt,
                    ImagePath = d.ImagePath,
                    SubscribersNumber = d.SubscribersNumber,
                    IsActive = d.EndAt > DateTime.UtcNow,
                    User = d.UsersDraws.Where(ud => ud.IsWinner)
                    .Select(ud => new WinnerDTO
                    {
                        UserId = ud.UserId,
                        Name = ud.User.Name,
                        Email = ud.User.Email,
                        BirthDate = ud.User.BirthDate,
                        LastWinDate = ud.LastWinDate,
                        IsSubscribed = ud.User.IsSubscribed
                    }).FirstOrDefault()
                }).OrderByDescending(d=>d.EndAt)
                .ToListAsync();

            return draws.Any() ?
                new ResponseModel { IsSuccess = true, Model = draws, StatusCode = 200 } :
                new ResponseModel 
                { 
                    IsSuccess = false,
                    Model = new List<Draw>(),
                    StatusCode = 404,
                    Message="لا يوجد أي سحب من قبل"
                };
        }

        //  Last 4 draws and it's winners

        public async Task<ResponseModel> GetLastFourDraws()
        {
             var draws = await _context.Draws
               .Where(d=>d.StartAt<=DateTime.Now)
                .Select(d => new DrawWithWinnerDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    StartAt = d.StartAt,
                    EndAt = d.EndAt,
                    ImagePath = d.ImagePath,
                    SubscribersNumber = d.SubscribersNumber,
                    IsActive = d.EndAt > DateTime.UtcNow,
                    User = d.UsersDraws.Where(ud => ud.IsWinner)
                    .Select(ud => new WinnerDTO
                    {
                        UserId = ud.UserId,
                        Name = ud.User.Name,
                        Email = ud.User.Email,
                        BirthDate = ud.User.BirthDate,
                        LastWinDate = ud.LastWinDate,
                        IsSubscribed = ud.User.IsSubscribed
                    }).FirstOrDefault()
                }).OrderByDescending(d=>d.EndAt).Take(4)
                .ToListAsync();

            return draws.Any() ?
                new ResponseModel { IsSuccess = true, Model = draws, StatusCode = 200 } :
                new ResponseModel { IsSuccess = false, Model = new List<Draw>(), StatusCode = 404 };
        }

        public async Task EndDrawIfNoWinner()
        {
            //get last recent active draw with no winner
            var lastActiveDraw = await _context.Draws.Include(d => d.UsersDraws)
                .Where(d => d.EndAt <= DateTime.Now)
                .Where(d => DateTime.Now <= d.EndAt.AddHours(1))
                .Where(d => !d.UsersDraws.Any(ud => ud.IsWinner))
                .OrderByDescending(d => d.EndAt).FirstOrDefaultAsync();

            if (lastActiveDraw != null && DateTime.Now >= lastActiveDraw.EndAt)
            {
                await RandomDrawWinner(lastActiveDraw.Id);
            }
        }
    }

}
