using HealthAndFinance.BL;
using HealthAndFinance.BL.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthAndFinance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController : ControllerBase
    {
        private readonly LikeService _likeService;

        public LikesController(LikeService likeService)
        {
            _likeService = likeService;
        }

        [HttpPost("toggle-like")]
        [AllowAnonymous] 
        public async Task<IActionResult> ToggleLike([FromBody] ToggleLikeRequest request)
        {
            try
            {
                if (request.PostId <= 0 || request.UserId <= 0)
                    return BadRequest("Invalid PostId or UserId");

                var result = await _likeService.ToggleLike(request.PostId, request.UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error toggling like: " + ex.Message);
            }
        }

        [HttpGet("{postId}/status")]
        public async Task<IActionResult> GetLikeStatus(int postId)
        {
            try
            {
                var result = await _likeService.GetLikeStatus(postId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error getting like status");
            }
        }
    }
}
