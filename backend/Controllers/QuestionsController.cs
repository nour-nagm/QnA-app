using Microsoft.AspNetCore.Mvc;
using QnA.Api.Data;
using QnA.Api.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QnA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IDataRepository repository;
        private readonly IQuestionCache cache;

        public QuestionsController(IDataRepository dataRepository, IQuestionCache cache)
        {
            repository = dataRepository;
            this.cache = cache;
        }

        #region Get Methods
        [HttpGet]
        public IEnumerable<QuestionGetManyResponses> GetQuestions
            (string search, int page = 1, int pageSize = 20, bool includeAnswers = false)
        {
            return string.IsNullOrEmpty(search) ? 
                repository.GetQuestionsWithPaging(page, pageSize, includeAnswers)
                : repository.GetQuestionsBySearchWithPaging
                (search, page, pageSize, includeAnswers);
        }

        [HttpGet("unanswered")]
        public async Task<IEnumerable<QuestionGetManyResponses>> GetUnansweredQuestions()
            => await repository.GetUnansweredQuestionsAsync();

        [HttpGet("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> GetQuestion(int questionId)
        {
            var question = repository.GetQuestion(questionId);
            
            if (question == null)
                return NotFound();

            cache.Set(question);
            return question;
        } 
        #endregion

        [HttpPost]
        public ActionResult<QuestionGetSingleResponse> PostQuestion
            (QuestionPostRequest questionPostRequest)
        {
            var savedQuestion = repository
                .PostQuestion(new QuestionPostFullRequest
                {
                    Title = questionPostRequest.Title,
                    Content = questionPostRequest.Content,
                    UserId = "1",
                    UserName = "bob.test@test.com",
                    Created = DateTime.UtcNow
                });

            return CreatedAtAction(
                actionName: nameof(GetQuestion),
                routeValues: new { questionId = savedQuestion.QuestionId },
                value: savedQuestion
                );
        }

        [HttpPut("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> PutQuestion
            (int questionId, QuestionPutRequest questionPutRequest)
        {
            var question = repository.GetQuestion(questionId);

            if (question == null)
                return NotFound();

            questionPutRequest.Title = string.IsNullOrEmpty(questionPutRequest.Title) ?
                question.Title : questionPutRequest.Title;

            questionPutRequest.Content = string.IsNullOrEmpty(questionPutRequest.Content) ?
                question.Content : questionPutRequest.Content;

            cache.Remove(questionId);
            
            return repository.PutQuestion(questionId, questionPutRequest);
        }

        [HttpDelete("{questionId}")]
        public ActionResult DeleteQuestion(int questionId)
        {
            var question = repository.GetQuestion(questionId);

            if (question == null)
                return NotFound();

            repository.DeleteQuestion(questionId);
            return NoContent();
        }

    }
}
