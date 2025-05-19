using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.Data
{
    public class QuestionAnswer
    {
        public int QA_ID { get; set; }

        [Required]
        public string ElementsContents { get; set; }

        public int? ParentID { get; set; }

        [Required]
        public bool IsQuestion { get; set; }

        public int? LabelID { get; set; }

        public float? Score { get; set; }
        public virtual QuestionAnswer Parent { get; set; }
        public virtual ICollection<QuestionAnswer> Children { get; set; } = new List<QuestionAnswer>();
        public virtual Label Label { get; set; }
    }

}
