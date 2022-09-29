using System.ComponentModel.DataAnnotations;

namespace QnA.Api.Data.Models
{
    public class AnswerPostRequest
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public int? QuestionId { get; set; }
    }
}
