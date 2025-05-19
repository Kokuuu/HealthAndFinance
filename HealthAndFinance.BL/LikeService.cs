using Dapper;
using HealthAndFinance.BL.Dtos;
using HealthAndFinance.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.BL
{
    public class LikeService
    {
        private readonly DapperContext _context;
        private readonly PostService _postService;

        public LikeService(DapperContext context, PostService postService)
        {
            _context = context;
            _postService = postService;
        }

        public async Task<LikeActionResultDto> ToggleLike(int postId, int userId)
        {
            await using var connection = await _context.CreateAsyncConnection();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var existingLike = await connection.QueryFirstOrDefaultAsync<Like>(
                    "SELECT * FROM Likes WHERE PostId = @PostId AND UserId = @UserId",
                    new { PostId = postId, UserId = userId },
                    transaction);

                if (existingLike != null)
                {
                    await connection.ExecuteAsync(
                        "DELETE FROM Likes WHERE LikeId = @LikeId",
                        new { existingLike.LikeId },
                        transaction);
                }
                else
                {
                    await connection.ExecuteAsync(
                        "INSERT INTO Likes (UserId, PostId) VALUES (@UserId, @PostId)",
                        new { UserId = userId, PostId = postId },
                        transaction);
                }

                var likeCount = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM Likes WHERE PostId = @PostId",
                    new { PostId = postId },
                    transaction);

                var likedByUsers = await connection.QueryAsync<LikeUserDto>(
                    @"SELECT u.UserId, u.FirstName, u.LastName, u.ProfilePhoto 
                        FROM Likes l
                        JOIN Users u ON l.UserId = u.UserId
                        WHERE l.PostId = @PostId",
                    new { PostId = postId },
                    transaction);

                await transaction.CommitAsync();

                return new LikeActionResultDto
                {
                    PostId = postId,
                    LikeCount = likeCount,
                    LikedBy = likedByUsers.ToList()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<LikeActionResultDto> GetLikeStatus(int postId)
        {
            using var connection = _context.CreateConnection();

            var likeCount = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Likes WHERE PostId = @PostId",
                new { PostId = postId });

            var likedByUsers = await connection.QueryAsync<LikeUserDto>(
                @"SELECT u.UserId, u.FirstName, u.LastName, u.ProfilePhoto 
                    FROM Likes l
                    JOIN Users u ON l.UserId = u.UserId
                    WHERE l.PostId = @PostId",
                new { PostId = postId });

            return new LikeActionResultDto
            {
                PostId = postId,
                LikeCount = likeCount,
                LikedBy = likedByUsers.ToList()
            };
        }
    }
}
