using Dapper;
using HealthAndFinance.Data;
using HealthAndFinance.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.BL
{
    public class FriendShipService
    {
        private DapperContext _context;
        public FriendShipService(DapperContext context)
        {
            _context = context;
        }
        
        private const string GET_PENDING_REQUESTS = @"
        SELECT 
            fs.FriendShipID, fs.UserId_1, fs.UserId_2, 
            fs.CreatedFriendship, fs.StatusId,
            req.UserID, req.FirstName, req.LastName, req.Email, req.ProfilePhoto,
            rec.UserID, rec.FirstName, rec.LastName, rec.Email, rec.ProfilePhoto,
            fsStatus.StatusID, fsStatus.StatusName
        FROM FriendShips fs
        INNER JOIN Users req ON fs.UserId_1 = req.UserID
        INNER JOIN Users rec ON fs.UserId_2 = rec.UserID
        INNER JOIN StatusFriendships fsStatus ON fs.StatusId = fsStatus.StatusID
        WHERE (fs.UserId_2 = @CurrentUserId AND fs.StatusId = 1)  -- StatusId 1 = Pending
        ORDER BY fs.CreatedFriendship DESC";

        public async Task<List<FriendShip>> GetPendingRequests(int userId)
        {
            return (await _context.QueryAsync<FriendShip, User, User, FriendShipStatus, FriendShip>(
                GET_PENDING_REQUESTS,
                (fs, requester, receiver, status) => new FriendShip
                {
                    FriendShipID = fs.FriendShipID,
                    UserId_1 = fs.UserId_1,
                    UserId_2 = fs.UserId_2,
                    CreatedFriendship = fs.CreatedFriendship,
                    StatusId = fs.StatusId,
                    Requester = requester,
                    Recever = receiver,
                    Status = status
                },
                new { CurrentUserId = userId},
                splitOn: "UserID,UserID,StatusID"))
                .ToList();
        }

        public async Task<OperationResult> AcceptFriendRequest(int userId, int friendId)
        {
            var (userId1, userId2) = userId < friendId ? (userId, friendId) : (friendId, userId);

            using var connection = _context.CreateConnection();

            var affectedRows = await connection.ExecuteAsync(
                @"UPDATE FriendShips 
          SET StatusId = @AcceptedStatus
          WHERE UserId_1 = @UserId1 
            AND UserId_2 = @UserId2
            AND StatusId = @PendingStatus",
                new
                {
                    UserId1 = userId1,
                    UserId2 = userId2,
                    AcceptedStatus = (int)FriendshipStatus.Accepted,
                    PendingStatus = (int)FriendshipStatus.Pending
                });

            return affectedRows > 0
                ? OperationResult.CreateSuccess("Friend request accepted")
                : OperationResult.CreateFailure("No pending friend request found");
        }

        public async Task<OperationResult> SendFriendRequest(int requesterId, int receiverId)
        {
            if (!await UserExists(requesterId) || !await UserExists(receiverId))
            {
                return OperationResult.CreateFailure("One or both users do not exist");
            }

            if (requesterId == receiverId)
            {
                return OperationResult.CreateFailure("Cannot send friend request to yourself");
            }

            var (userId1, userId2) = requesterId < receiverId
                ? (requesterId, receiverId)
                : (receiverId, requesterId);

            using var connection = _context.CreateConnection();

            var existingFriendship = await connection.QueryFirstOrDefaultAsync<FriendShip>(
                @"SELECT * FROM FriendShips 
          WHERE (UserId_1 = @UserId1 AND UserId_2 = @UserId2)",
                new { UserId1 = userId1, UserId2 = userId2 });

            if (existingFriendship != null)
            {
                return OperationResult.CreateFailure(
                    existingFriendship.StatusId == (int)FriendshipStatus.Pending
                        ? "Friend request already pending"
                        : "Users are already friends");
            }

            var friendshipId = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO FriendShips (UserId_1, UserId_2, CreatedFriendship, StatusId)
          VALUES (@UserId1, @UserId2, GETDATE(), @StatusId);
          SELECT CAST(SCOPE_IDENTITY() AS INT)",
                new
                {
                    UserId1 = userId1,
                    UserId2 = userId2,
                    StatusId = (int)FriendshipStatus.Pending
                });

            return OperationResult.CreateSuccess("Friend request sent successfully");
        }

        private async Task<bool> UserExists(int userId)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<bool>(
                "SELECT 1 FROM Users WHERE UserID = @UserId",
                new { UserId = userId });
        }
    }
}
