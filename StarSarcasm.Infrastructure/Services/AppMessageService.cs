using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
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
    public class AppMessageService:IAppMessageService
    {
        private readonly Context _context;

        public AppMessageService(Context context)
        {
            _context = context;
        }

        public async Task<ResponseModel> UploadMessagesExcelFile(IFormFile file)
        {
            if(file==null || file.Length == 0)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "الملف فارغ او غير موجود"
                };
            }

            var messages = new List<Message>();

            using(var stream=new  MemoryStream())
            {
                await file.CopyToAsync(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowsCount = worksheet.Dimension.Rows;

                    for(int row=1; row<rowsCount; row++)
                    {
                        var message = new Message
                        {
                            Title = "",
                            Content = worksheet.Cells[row, 1].Text
                        };
                        messages.Add(message);
                    }
                }
            }
            _context.Messages.AddRange(messages);
            await _context.SaveChangesAsync();

            return new ResponseModel
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "تم تحميل الملف وحفظ الرسائل بنجاح"
            };

        }

        public async Task<ResponseModel> AddMessage(string msg)
        {
            if (msg.IsNullOrEmpty())
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 204,
                    Message = "الرسالة فارغة"
                };
            }

            var message = new Message
            {
                Title = "",
                Content = msg
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return new ResponseModel
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "تم اضافة الرسالة بنجاح",
                Model = msg
            };
        }
    }
}
