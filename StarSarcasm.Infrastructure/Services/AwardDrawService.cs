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
                    ImagePath = imagePath
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


    }

}
