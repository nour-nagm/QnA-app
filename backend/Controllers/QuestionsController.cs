using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QnA.Api.Data;
using QnA.Api.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QnA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IDataRepository repository;

        public QuestionsController(IDataRepository dataRepository) => this.repository = dataRepository;

        #region Get Methods
        [HttpGet]
        public IEnumerable<QuestionGetManyResponses> GetQuestions
            (string search) => string.IsNullOrEmpty(search) ?
                repository.GetQuestions() :
                repository.GetQuestionsBySearch(search);

        [HttpGet("unanswered")]
        public IEnumerable<QuestionGetManyResponses> GetUnansweredQuestions()
            => repository.GetUnansweredQuestions();

        [HttpGet("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> GetQuestion(int questionId)
        {
            var question = repository.GetQuestion(questionId);

            return question != null ? question : NotFound();
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
