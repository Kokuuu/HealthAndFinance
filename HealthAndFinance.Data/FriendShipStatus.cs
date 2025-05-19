using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.Data
{
    public class FriendShipStatus
    {
        public int StatusID { get; set; }

        public string StatusName { get; set; }

        public List<FriendShip> FriendShips { get; set; } = new List<FriendShip>();
    }

    public enum FriendshipStatus
    {
        Pending = 1,
        Accepted = 2,
        Declined = 3,
        Blocked = 4
    }
}
