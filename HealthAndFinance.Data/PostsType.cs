using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.Data
{
    public class PostsType
    {
        public int PostTypesId { get; set;}
        public string PostType { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}
