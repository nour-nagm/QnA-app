using System;
using System.ComponentModel.DataAnnotations;

namespace QnA.Api.Data.Models
{
    public class QuestionPostFullRequest : QuestionPostRequest
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
    }
}
