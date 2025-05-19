using HealthAndFinance.BL;
using HealthAndFinance.BL.Dtos;
using HealthAndFinance.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthAndFinance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendShipController : ControllerBase
    {
        private readonly FriendShipService _friendshipService;

        public FriendShipController(FriendShipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        [HttpGet("friendships/pending")]
        public async Task<IActionResult> GetPendingRequests(int userId)
        {
            var requests = await _friendshipService.GetPendingRequests(userId);
            return Ok(requests);
        }


        [HttpPost("send-request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDto request)
        {
            var result = await _friendshipService.SendFriendRequest(request.RequesterId, request.ReceiverId);

            if (result.IsSuccess)
                return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }

        [HttpPost("accept-request")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] AcceptRequestDto request)
        {
            var result = await _friendshipService.AcceptFriendRequest(request.UserId, request.FriendId);

            if (result.IsSuccess)
                return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }
    }
}