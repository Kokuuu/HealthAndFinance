using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.BL.Dtos
{
    public class LikeActionResultDto
    {
        public int PostId { get; set; }
        public int LikeCount { get; set; }
        public List<LikeUserDto> LikedBy { get; set; } = new();
    }
}
