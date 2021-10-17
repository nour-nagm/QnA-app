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
    [Route("api/Questions/{questionId}/[controller]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly IDataRepository repository;
        public AnswersController(IDataRepository dataRepository) => this.repository = dataRepository;

        [HttpPost]
        public ActionResult<AnswerGetResponse> PostAnswer
            (int questionId, AnswerPostRequest answerPostRequest)
        {
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
