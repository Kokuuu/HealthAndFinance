using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.Data
{
    public class FriendShip
    {
        public int FriendShipID { get; set; }
        public int UserId_1 { get; set; }
        public User Requester { get; set; }
        public int UserId_2 { get; set; }
        public User Recever { get; set; }
        public DateTime CreatedFriendship { get; set; }
        public int StatusId { get; set; }
        public FriendShipStatus Status { get; set; }
    }
}
