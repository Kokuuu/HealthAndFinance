using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.BL.Dtos
{
    public class ToggleLikeRequest
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
