using QnA.Api.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QnA.Api.Data
{
    public interface IDataRepository
    {
        IEnumerable<QuestionGetManyResponses> GetQuestions(bool includeAnswers);
        IEnumerable<QuestionGetManyResponses> GetQuestionsWithPaging(int pageNumber, int pageSize, bool includeAnswers);
        IEnumerable<QuestionGetManyResponses> GetQuestionsBySearch(string search, bool includeAnswers);
        IEnumerable<QuestionGetManyResponses>GetQuestionsBySearchWithPaging(string search, int pageNumber, int pageSize, bool includeAnswers);
        IEnumerable<QuestionGetManyResponses> GetUnansweredQuestions();
        Task<IEnumerable<QuestionGetManyResponses>> GetUnansweredQuestionsAsync();
        QuestionGetSingleResponse GetQuestion(int questionId);
        bool QuestionExists(int questionId);
        AnswerGetResponse GetAnswer(int answerId);

        QuestionGetSingleResponse PostQuestion(QuestionPostFullRequest question);
        QuestionGetSingleResponse PutQuestion(int questionId, QuestionPutRequest question);
        void DeleteQuestion(int questionId);
        AnswerGetResponse PostAnswer(int questionId, AnswerPostFullRequest answer);
    }
}
