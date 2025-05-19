using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.Data
{
    public class UserResultByDay
    {
        public int UserID { get; set; }
        public User User { get; set; }
        public DateTime ResultDate { get; set; } = DateTime.Now;
        public float SuccessRate { get; set; }
    }
}
