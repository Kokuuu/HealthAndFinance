using HealthAndFinance.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.BL
{
    public class UserRankService
    {
        private readonly DapperContext _context;

        public UserRankService(DapperContext context)
        {
            _context = context;
        }

        public async Task AddDailyResult(UserResultByDay result)
        {
            const string sql = @"
            INSERT INTO UserResultByDay 
                (UserID, ResultDate, SuccessRate)
            VALUES 
                (@UserID, @ResultDate, @SuccessRate)";

            await _context.ExecuteAsync(sql, result);
        }

        public async Task<UsersRanks> GetUserRank(int userId)
        {
            const string sql = @"
            SELECT 
                ur.UserID,
                ur.RankPercent,
                pr.StarsAppeal
            FROM UsersRanks ur
            LEFT JOIN ProgressRanks pr ON ur.RankPercent = pr.[Percent]
            WHERE ur.UserID = @UserId"
            ;

            return await _context.QuerySingleOrDefaultAsync<UsersRanks>(sql, new { UserId = userId });
        }

        public async Task UpdateUserRanks(IEnumerable<UserResultByDay> results)
        {
            const string sql = @"
            INSERT INTO UserResultByDay 
                (UserID, ResultDate, SuccessRate)
            VALUES 
                (@UserID, @ResultDate, @SuccessRate)";

            await _context.ExecuteAsync(sql, results);
        }
    }
}
