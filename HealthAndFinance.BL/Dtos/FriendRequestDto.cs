using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.BL.Dtos
{
    public class FriendRequestDto
    {
        [Required]
        public int RequesterId { get; set; }

        [Required]
        public int ReceiverId { get; set; }
    }

    public class AcceptRequestDto
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
    }

}
