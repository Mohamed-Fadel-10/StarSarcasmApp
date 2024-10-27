using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Interfaces.IFileUploadService;

namespace StarSarcasm.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IPostservice _postService;

        public PostController(IPostservice postService, IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
            _postService = postService;
        }
        [HttpPost("AddPost")]
        public async Task<IActionResult> AddPost(PostDTO model)
        {
            if (ModelState.IsValid)
            {

                var response = await _postService.AddPost(model);
                if (response.IsSuccess)
                {
                    return StatusCode(response.StatusCode, response.Message);
                }
                return StatusCode(response.StatusCode, response.Message);
            }
            return BadRequest(ModelState);
        }
    }
}
