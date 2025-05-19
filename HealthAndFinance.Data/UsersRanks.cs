using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.Data
{
    public class UsersRanks
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int RankPercent { get; set; }
    }
}
