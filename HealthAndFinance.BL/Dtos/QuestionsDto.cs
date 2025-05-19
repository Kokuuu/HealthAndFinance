using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.BL.Dtos
{
    public class QuestionDto
    {
        public int QA_ID { get; set; }
        public string ElementsContents { get; set; }
        public List<AnswerDto> Answers { get; set; } = new();
        public int QuestionNumber { get; set; }
        public int TotalQuestions { get; set; }
    }

    public class AnswerDto
    {
        public int QA_ID { get; set; }
        public string ElementsContents { get; set; }
        public float? Score { get; set; }
    }

    public class QuizResultDto
    {
        public float FinalScore { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; } // Optional if you want to track this
    }

    public class AnswerSubmissionDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int AnswerId { get; set; }
    }
}
