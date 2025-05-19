using HealthAndFinance.BL.Dtos;
using HealthAndFinance.BL;
using HealthAndFinance.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace HealthAndFinance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly PostService _postService;

        public PostsController(PostService postService)
        {
            _postService = postService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPosts(int userId)
        {
            var posts = await _postService.GetUserPosts(userId);
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _postService.GetPostById(id);
            return post != null ? Ok(post) : NotFound();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto postDto)
        {
            if (postDto == null) return BadRequest("Invalid input.");
            var post = new Post
            {
                Title = postDto.Title,
                Content = postDto.Content,
                PostTypeId = postDto.PostTypeId,
                UserId = 12,
                CreationDate = DateTime.UtcNow
            };

            var postId = await _postService.CreatePostAsync(post);
            return Ok(new { PostId = postId });
        }
    }
}
