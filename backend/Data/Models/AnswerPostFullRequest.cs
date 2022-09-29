using System;

namespace QnA.Api.Data.Models
{
    public class AnswerPostFullRequest : AnswerPostRequest
    {
        public new int QuestionId { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
    }
}
