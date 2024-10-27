using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Interfaces.IFileUploadService;
using StarSarcasm.Application.Response;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services
{
    public class PostService:IPostservice
    {
        private readonly Context _context;
        private readonly IFileUploadService _fileUploadService;

        public PostService(Context context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<ResponseModel> AddPost(PostDTO model)
        {
            if (model == null)
            {
                return new ResponseModel()
                {
                    StatusCode = 400,
                    Message = "يوجد بيانات غير صالحة يرجى اعادة المحاولة",
                    IsSuccess = false
                };
            }

            var post = new Post
            {
                Title = model.Title,
                Description = model.Description,
                CreatedAt = DateTime.UtcNow,
            };

            if (model.ImagePath != null && model.ImagePath.Length > 0)
            {
                try
                {
                    post.ImagePath = await _fileUploadService.SaveFileAsync(model.ImagePath, "PostImages");
                }
                catch (Exception)
                {
                    return new ResponseModel
                    {
                        StatusCode = 500,
                        Message = "حدث خطأ أثناء حفظ الصورة. يرجى المحاولة لاحقًا",
                        IsSuccess = false,
                    };
                }
            }

            if (model.VideoPath != null && model.VideoPath.Length > 0)
            {
                try
                {
                    post.VideoPath = await _fileUploadService.SaveFileAsync(model.VideoPath, "PostVideos");
                }
                catch (Exception)
                {
                    return new ResponseModel
                    {
                        StatusCode = 500,
                        Message = "حدث خطأ أثناء حفظ الفيديو. يرجى المحاولة لاحقًا",
                        IsSuccess = false,
                    };
                }
            }

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return new ResponseModel
            {
                StatusCode = 201,
                Message = "تم إضافة المنشور بنجاح",
                Model=post,
                IsSuccess = true
            };
        }

        public async Task<ResponseModel> DeletePost(int Id)
        {
                try
                {
                    var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == Id);
                    if (post == null) {
                        return new ResponseModel
                        {
                            IsSuccess = false,
                            StatusCode = 400,
                            Message = "لا يوجد منشور بهذا الرقم التعريفى"
                        };
                    }
                    _context.Posts.Remove(post);
                    await _context.SaveChangesAsync();

                    return new ResponseModel { 
                        IsSuccess = true, 
                        Message = $"تم حذف المنشور بنجاح",
                        StatusCode=200 
                    };

                } 
                catch (Exception ex) {
                    return new ResponseModel 
                    {
                        IsSuccess = false,
                        Message = "حدث خطأ اثناء حذف المنشور برجائ اعادة المحاولة ",
                        StatusCode=500 
                    };
                }
        }
        public async Task<ResponseModel> GetAllPosts()
        {
            var posts=await _context.Posts.ToListAsync();
            return
                posts.Any() ? 
                new ResponseModel { IsSuccess = true, Model = posts, StatusCode = 200 } :
                new ResponseModel { IsSuccess = false, Model = new List<Post>(), StatusCode = 404 };
        }

    }
}
