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
        private readonly IPostservice _postService;

        public PostController(IPostservice postService)
        {
            _postService = postService;
        }
        [HttpPost("AddPost")]
        public async Task<IActionResult> AddPost(PostDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _postService.AddPost(model);
                return response.IsSuccess ? 
                    StatusCode(response.StatusCode, response.Model) :
                    StatusCode(response.StatusCode, response.Message);
            }
                return BadRequest(ModelState);
        }
        [HttpGet("GetAllPosts")]
        public async Task<IActionResult> GetAll()
        {
            var response= await _postService.GetAllPosts();
            return response.IsSuccess ?
                StatusCode(response.StatusCode, response.Model) : 
                StatusCode(response.StatusCode, response.Model);
        }
        [HttpDelete("DeletePost")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var response = await _postService.DeletePost(id);
            return response.IsSuccess ?
               StatusCode(response.StatusCode, response.Message) :
               StatusCode(response.StatusCode, response.Message);
        }
    }
}
