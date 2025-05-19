using HealthAndFinance.BL;
using HealthAndFinance.Data;
using Microsoft.AspNetCore.Mvc;

namespace HealthAndFinance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRankController : ControllerBase
    {
        private readonly UserRankService _userRankService;

        public UserRankController(UserRankService userRankService)
        {
            _userRankService = userRankService;
        }

        [HttpPost("daily-result")]
        public async Task<IActionResult> AddDailyResult([FromBody] UserResultByDay result)
        {
            await _userRankService.AddDailyResult(result);
            var updatedRank = await _userRankService.GetUserRank(result.UserID);
            return Ok(updatedRank);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetRank(int userId)
        {
            var rank = await _userRankService.GetUserRank(userId);
            return Ok(rank);
        }
    }
}
