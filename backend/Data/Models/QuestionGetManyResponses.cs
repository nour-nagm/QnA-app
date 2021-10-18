using System;
using System.Collections.Generic;

namespace QnA.Api.Data.Models
{
    public class QuestionGetManyResponses
    {
        public int QuestionId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
        public List<AnswerGetResponse> Answers { get; set; }
    }
}
