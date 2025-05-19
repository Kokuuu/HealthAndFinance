using Dapper;
using HealthAndFinance.BL.Dtos;
using HealthAndFinance.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using BCrypt.Net;

namespace HealthAndFinance.BL
{
    public class UserService
    {
        private DapperContext _context;
        public UserService(DapperContext context)
        {
            _context = context;
        }
        private const string CREATE_USER_SQL = @"
            INSERT INTO Users (FirstName, LastName, Email, UserPassword, ProfilePhoto)
            VALUES (@FirstName, @LastName, @Email, @UserPassword, @ProfilePhoto);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        public async Task<int> CreateUserAsync(UserCreateDto userDto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.UserPassword);
            var passwordBytes = Convert.FromBase64String(
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(hashedPassword)));

            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                UserPassword = passwordBytes,
                ProfilePhoto = userDto.ProfilePhoto
            };

            using var connection = _context.CreateConnection();
            connection.Open();

            var userId = await connection.ExecuteScalarAsync<int>(CREATE_USER_SQL, user);
            return userId;
        }


        private const string GET_USER_BY_EMAIL_SQL = @"
            SELECT UserID, FirstName, LastName, Email, UserPassword, ProfilePhoto 
            FROM Users 
            WHERE Email = @Email";

        public async Task<User> LoginAsync(string email, string plainPassword)
        {
            using var connection = _context.CreateConnection();

            var user = await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Email = @Email", new { Email = email });

            if (user == null)
                return null;

            var storedHash = Encoding.UTF8.GetString(user.UserPassword);
            var isValid = BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);

            return isValid ? user : null;
        }

        public async Task CalculateSuccessUserRateAsync(int userId)
        {
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                "dbo.CalculateSuccessUserRate",
                new { UserID = userId },
                commandType: System.Data.CommandType.StoredProcedure
            );
        }

        public async Task<(List<QuestionAnswer> matching, List<QuestionAnswer> nonMatching)> GetQuestionsBasedOnUserAnswerAsync(int userId, int firstQid, int secondQid)
        {
            using var connection = _context.CreateConnection();

            var parameters = new
            {
                UserID = userId,
                FirstQuestionID = firstQid,
                SecondQuestionID = secondQid
            };

            using var multi = await connection.QueryMultipleAsync(
                "dbo.QuestionsBasedOnUserAnswer",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var matchingQuestions = (await multi.ReadAsync<QuestionAnswer>()).ToList();
            var nonMatchingQuestions = (await multi.ReadAsync<QuestionAnswer>()).ToList();

            return (matchingQuestions, nonMatchingQuestions);
        }

        public async Task<bool> VerifyCurrentPassword(int userId, string password)
        {
            using var connection = _context.CreateConnection();

            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT UserID, UserPassword FROM Users WHERE UserID = @UserId",
                new { UserId = userId });

            if (user == null || user.UserPassword == null || user.UserPassword.Length == 0)
                return false;

            try
            { 
                var storedHashBase64 = Convert.ToBase64String(user.UserPassword);
                var storedHash = Encoding.UTF8.GetString(Convert.FromBase64String(storedHashBase64));
                return BCrypt.Net.BCrypt.Verify(password, storedHash);
            }
            catch
            {
                return false;
            }
        }

        public async Task<PasswordChangeResultDto> ChangePassword(ChangePasswordDto dto)
        {
            try
            {
                if (!await VerifyCurrentPassword(dto.UserId, dto.CurrentPassword))
                {
                    return new PasswordChangeResultDto { Success = false, Message = "Current password is incorrect" };
                }

                var newHashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                var hashedPasswordBytes = Convert.FromBase64String(
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(newHashedPassword)));

                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(
                    "UPDATE Users SET UserPassword = @Password WHERE UserID = @UserId",
                    new { Password = hashedPasswordBytes, UserId = dto.UserId });

                return new PasswordChangeResultDto
                {
                    Success = affectedRows > 0,
                    Message = affectedRows > 0 ? "Password changed successfully" : "Failed to update password"
                };
            }
            catch (Exception ex)
            {
                return new PasswordChangeResultDto
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }
    }
}
