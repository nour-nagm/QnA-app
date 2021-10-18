using Microsoft.AspNetCore.Mvc;
using QnA.Api.Data;
using QnA.Api.Data.Models;
using System;

namespace QnA.Api.Controllers
{
    [Route("api/Questions/{questionId}/[controller]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly IDataRepository repository;
        private readonly IQuestionCache cache;
        public AnswersController(IDataRepository dataRepository, IQuestionCache cache)
        {
            repository = dataRepository;
            this.cache = cache;
        }

        [HttpPost]
        public ActionResult<AnswerGetResponse> PostAnswer
            (int questionId, AnswerPostRequest answerPostRequest)
        {
            cache.Remove(questionId);
            return !repository.QuestionExists(questionId) ? NotFound()
                : repository.PostAnswer(questionId, new AnswerPostFullRequest
                {
                    Content = answerPostRequest.Content,
                    UserId = "1",
                    UserName = "bob.test@test.com",
                    Created = DateTime.UtcNow
                });
        }
    }
}
