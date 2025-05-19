using HealthAndFinance.BL;
using HealthAndFinance.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthAndFinance.BL.Dtos;
using System.Text;


namespace HealthAndFinance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            if (userDto == null) return BadRequest("Invalid input.");

            var userId = await _userService.CreateUserAsync(userDto);
            return Ok(new { UserId = userId });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return BadRequest("Invalid credentials.");

            var user = await _userService.LoginAsync(loginDto.Email, loginDto.Password);

            if (user == null)
                return Unauthorized("Incorrect email or password.");

            return Ok(new
            {
                user.UserID,
                user.FirstName,
                user.LastName,
                user.Email,
                user.ProfilePhoto
            });
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<PasswordChangeResultDto>> ChangePassword(
        [FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.ChangePassword(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /*[HttpPost("{userId}/calculate-success-rate")]
        public async Task<IActionResult> CalculateSuccessRate(int userId)
        {
            await _userService.CalculateSuccessUserRateAsync(userId);
            return Ok(new { message = "Success rate calculated and stored." });
        }

        [HttpGet("{userId}/questions")]
        public async Task<IActionResult> GetAdaptiveQuestions(int userId, int firstQid, int secondQid)
        {
            var (matching, nonMatching) = await _userService.GetQuestionsBasedOnUserAnswerAsync(userId, firstQid, secondQid);

            return Ok(new
            {
                MatchingLabelQuestions = matching,
                NonMatchingLabelQuestions = nonMatching
            });
        }*/

    }
}
