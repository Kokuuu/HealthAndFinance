using Dapper;
using HealthAndFinance.BL.Dtos;
using HealthAndFinance.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.BL
{
    public class QuestionService
    {
            private readonly DapperContext _context;

            public QuestionService(DapperContext context)
            {
                _context = context;
            }

        public async Task<QuestionDto> GetNextQuestion(int userId, int currentQuestionIndex, List<QuestionAnswer> currentQuizQuestions)
        {
            if (currentQuestionIndex >= currentQuizQuestions.Count)
                return null;

            var question = currentQuizQuestions[currentQuestionIndex];
            var answers = await GetAnswersForQuestion(question.QA_ID);

            return new QuestionDto
            {
                QA_ID = question.QA_ID,
                ElementsContents = question.ElementsContents,
                Answers = answers.Select(a => new AnswerDto
                {
                    QA_ID = a.QA_ID,
                    ElementsContents = a.ElementsContents,
                    Score = a.Score
                }).ToList(),
                QuestionNumber = currentQuestionIndex + 1,
                TotalQuestions = currentQuizQuestions.Count
            };
        }

        public async Task<IEnumerable<QuestionAnswer>> GetQuestionsBasedOnUserAnswer(int userId, int firstQuestionId, int secondQuestionId)
            {
                using var connection = _context.CreateConnection();

                return await connection.QueryAsync<QuestionAnswer>(
                    "dbo.QuestionsBasedOnUserAnswer",
                    new
                    {
                        UserID = userId,
                        FirstQuestionID = firstQuestionId,
                        SecondQuestionID = secondQuestionId
                    },
                    commandType: CommandType.StoredProcedure
                );
            }

            public async Task<IEnumerable<QuestionAnswer>> GetAnswersForQuestion(int questionId)
            {
                using var connection = _context.CreateConnection();

                return await connection.QueryAsync<QuestionAnswer>(
                    "SELECT * FROM QuestionAnswers WHERE ParentID = @QuestionId AND IsQuestion = 0",
                    new { QuestionId = questionId }
                );
            }

            public async Task<QuestionAnswer> GetAnswerById(int answerId)
            {
                using var connection = _context.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<QuestionAnswer>(
                    "SELECT * FROM QuestionAnswers WHERE QA_ID = @AnswerId",
                    new { AnswerId = answerId }
                );
            }

            public async Task SaveUserAnswer(int userId, int questionId, int answerId, float score)
            {
                using var connection = _context.CreateConnection();

                await connection.ExecuteAsync(
                    @"INSERT INTO UserChoices (UserID, QuestionID, AnswerID, Score, AnsweredDate)
              VALUES (@UserId, @QuestionId, @AnswerId, @Score, GETDATE())",
                    new
                    {
                        UserId = userId,
                        QuestionId = questionId,
                        AnswerId = answerId,
                        Score = score
                    }
                );
            }

            public async Task<float> CalculateFinalScore(int userId)
            {
                using var connection = _context.CreateConnection();

                var totalScore = await connection.QueryFirstOrDefaultAsync<float>(
                    "SELECT ISNULL(SUM(Score), 0) FROM UserChoices WHERE UserID = @UserId",
                    new { UserId = userId }
                );

                return totalScore / 100;
            }

        }
}
