using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.Data
{
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] UserPassword { get; set; }
        public string ProfilePhoto { get; set; }
        public List<FriendShip> Requester { get; set; } = new List<FriendShip>();
        public List<FriendShip> Receiver { get; set; } = new List<FriendShip>(); 
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Like> Likes { get; set; } = new List<Like>();
        public List<UsersRanks> UsersRankss { get; set; } = new List<UsersRanks>();
    }
}
