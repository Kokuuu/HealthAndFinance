using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.Data
{
    public class Post
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public User Author { get; set; }
        public required string Title { get; set; }             
        public string Content { get; set; }
        public int PostTypeId { get; set; }
        public PostsType PostType { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public int? ParentPostID { get; set; }
        public Post ParentPost { get; set; }
        public List<Like> Likes { get; set; } = new List<Like>();
        public int LikeCount => Likes?.Count ?? 0;
    }
}
