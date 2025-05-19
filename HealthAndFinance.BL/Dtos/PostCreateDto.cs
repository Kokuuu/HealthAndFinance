using System.ComponentModel.DataAnnotations;

namespace HealthAndFinance.BL.Dtos
{
    public class CreatePostDto
    {
        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        [Required]
        public int PostTypeId { get; set; }
    }
}
