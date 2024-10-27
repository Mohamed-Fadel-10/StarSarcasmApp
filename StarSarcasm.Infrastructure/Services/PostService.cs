using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
                StatusCode = 200,
                Message = "تم إضافة المنشور بنجاح",
                IsSuccess = true
            };
        }

    }
}
