using QnA.Api.Data.Models;
using System.Collections.Generic;

namespace QnA.Api.Data
{
    public interface IDataRepository
    {
        IEnumerable<QuestionGetManyResponses> GetQuestions();
        IEnumerable<QuestionGetManyResponses> GetQuestionsBySearch(string search);
        IEnumerable<QuestionGetManyResponses> GetUnansweredQuestions();
        QuestionGetSingleResponse GetQuestion(int questionId);
        bool QuestionExists(int questionId);
        AnswerGetResponse GetAnswer(int answerId);

        QuestionGetSingleResponse PostQuestion(QuestionPostRequest question);
        QuestionGetSingleResponse PutQuestion(int questionId, QuestionPutRequest question);
        void DeleteQuestion(int questionId);
        AnswerGetResponse PostAnswer(AnswerPostRequest answer);
    }
}
