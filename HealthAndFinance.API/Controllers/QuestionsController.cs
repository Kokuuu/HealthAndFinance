using HealthAndFinance.BL.Dtos;
using HealthAndFinance.BL;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using HealthAndFinance.Data;
using System.Collections.Concurrent;

namespace HealthAndFinance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
            private readonly QuestionService _questionService;
            private static readonly ConcurrentDictionary<int, QuizSession> _sessions = new();

            public QuestionsController(QuestionService questionService)
            {
                _questionService = questionService;
            }

            [HttpPost("start")]
            public async Task<ActionResult<QuestionDto>> StartQuiz([FromBody] int userId)
            {
                var questions = (await _questionService.GetQuestionsBasedOnUserAnswer(userId, 0, 0)).ToList();

                if (!questions.Any())
                {
                    return NotFound("No questions available");
                }

                _sessions[userId] = new QuizSession
                {
                    Questions = questions,
                    CurrentQuestionIndex = 0
                };

                return await GetQuestion(userId);
            }

            [HttpGet("question")]
            public async Task<ActionResult<QuestionDto>> GetQuestion(int userId)
            {
                if (!_sessions.TryGetValue(userId, out var session))
                {
                    return BadRequest("No active quiz session. Please start a new quiz.");
                }

                var questionDto = await _questionService.GetNextQuestion(
                    userId,
                    session.CurrentQuestionIndex,
                    session.Questions);

                if (questionDto == null)
                {
                    return BadRequest("Quiz has already been completed.");
                }

                return Ok(questionDto);
            }

            [HttpPost("answer")]
            public async Task<ActionResult<QuestionDto>> SubmitAnswer([FromBody] AnswerSubmissionDto submission)
            {
                if (!_sessions.TryGetValue(submission.UserId, out var session))
                {
                    return BadRequest("No active quiz session. Please start a new quiz.");
                }

                // Save the answer
                var answer = await _questionService.GetAnswerById(submission.AnswerId);
                if (answer != null)
                {
                    await _questionService.SaveUserAnswer(
                        submission.UserId,
                        submission.QuestionId,
                        submission.AnswerId,
                        answer.Score ?? 0);
                }

                session.CurrentQuestionIndex++;

                if (session.CurrentQuestionIndex >= session.Questions.Count)
                {
                    var finalScore = await _questionService.CalculateFinalScore(submission.UserId);
                    _sessions.TryRemove(submission.UserId, out _); // Clean up session

                    return Ok(new QuizResultDto
                    {
                        FinalScore = finalScore,
                        TotalQuestions = session.Questions.Count
                    });
                }

                var nextQuestion = await _questionService.GetNextQuestion(
                    submission.UserId,
                    session.CurrentQuestionIndex,
                    session.Questions);

                return Ok(nextQuestion);
            }
        }

        public class QuizSession
        {
            public List<QuestionAnswer> Questions { get; set; }
            public int CurrentQuestionIndex { get; set; }


        }
    
}
