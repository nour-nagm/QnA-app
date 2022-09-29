using QnA.Api.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QnA.Api.Data
{
    public interface IDataRepository
    {
        Task<IEnumerable<QuestionGetManyResponses>> GetQuestions();
        Task<IEnumerable<QuestionGetManyResponses>> GetQuestionsWithAnswers();
        Task<IEnumerable<QuestionGetManyResponses>> GetQuestionsBySearch(string search);
        Task<IEnumerable<QuestionGetManyResponses>> GetQuestionsBySearchWithPaging(string search, int pageNumber, int pageSize);
        Task<IEnumerable<QuestionGetManyResponses>> GetUnansweredQuestions();
        Task<QuestionGetSingleResponse> GetQuestion(int questionId);
        Task<bool> QuestionExists(int questionId);
        Task<AnswerGetResponse> GetAnswer(int answerId);
        Task<QuestionGetSingleResponse> PostQuestion(QuestionPostFullRequest question);
        Task<QuestionGetSingleResponse> PutQuestion(int questionId, QuestionPutRequest question);
      
        Task DeleteQuestion(int questionId);
        Task<AnswerGetResponse> PostAnswer(AnswerPostFullRequest answer);
        Task<IEnumerable<QuestionGetManyResponses>> GetQuestionsWithPaging(int pageNumber, int pageSize);
    }
}
