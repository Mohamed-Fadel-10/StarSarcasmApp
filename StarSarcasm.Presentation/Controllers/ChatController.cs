using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.Interfaces;

namespace StarSarcasm.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IWebHostEnvironment _env;

        public ChatController(IChatService chatService, IWebHostEnvironment env) { 
        _chatService = chatService;
            _env = env;
        }


        [HttpGet("GetUserChats")]
        public async Task<IActionResult> GetUserChats(string userId)
        {
            var response=await _chatService.GetUserChats(userId);
            if (response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.Model);
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpGet("getFilePath")]
        public IActionResult FindFilePath(string folderName, string fileName)
        {
            // المسار الفعلي للمجلد في السيرفر
            var folderPath = Path.Combine(_env.WebRootPath, folderName);

            // التحقق من وجود المجلد
            if (Directory.Exists(folderPath))
            {
                var searchPattern = $"*{fileName}*";
                var matchedFiles = Directory.GetFiles(folderPath, searchPattern, SearchOption.AllDirectories).ToList();

                if (matchedFiles.Any())
                {
                    // توليد الرابط الكامل باستخدام الـ HttpContext
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";

                    // تحويل المسار الفعلي إلى رابط URL
                    var fileUrls = matchedFiles.Select(file =>
                    {
                        var relativePath = Path.GetRelativePath(_env.WebRootPath, file).Replace("\\", "/");
                        return $"{baseUrl}/{relativePath}";
                    }).ToList();

                    return Ok(fileUrls);
                }
                else
                {
                    return NotFound("No files matching the search pattern were found.");
                }
            }
            else
            {
                return BadRequest("Folder does not exist.");
            }
        }



    }
}
