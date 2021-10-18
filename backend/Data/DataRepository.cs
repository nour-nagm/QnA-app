using Dapper;
using static Dapper.SqlMapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QnA.Api.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QnA.Api.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly string connectionString;
        public DataRepository(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        public QuestionGetSingleResponse GetQuestion(int questionId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // We use the QueryMultiple method in Dapper to execute our two
            // stored procedures in a single database round trip.The results
            // are added into a results variable and can be retrieved using
            // the Read method by passing the appropriate type in the
            // generic parameter.
            using GridReader results = connection.QueryMultiple(
                @"EXEC dbo.Question_GetSingle
                @QuestionId = @QuestionId;
                    
                EXEC dbo.Answer_Get_ByQuestionId
                @QuestionId = @QuestionId",
                new { QuestionId = questionId });

            var question = results
                .Read<QuestionGetSingleResponse>()
                .FirstOrDefault();

            if (question != null)
            {
                question.Answers = results
                    .Read<AnswerGetResponse>()
                    .ToList();
            }

            return question;
        }
        public IEnumerable<QuestionGetManyResponses> GetQuestions(bool includeAnswers)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // Getting answers in one opend connection using
            // Question_GetMany_WithAnswers stored procedure returns
            // tabular data and we require this to be mapped to the
            // questions - and - answers hierarchical structure we have
            // in our QuestionGetManyResponse model.
            if (includeAnswers)
            {
                var questionDictionary
                    = new Dictionary<int, QuestionGetManyResponses>();

                // So, Dapper's multi-mapping feature can be used to resolve
                // the N+1 problem and generally achieve better performance.
                // We do need to be careful with this approach, though, as
                // we are requesting a lot of data from the database because
                // of the duplicate parent records. Processing large amounts
                // of data in the web server can be inefficient and lead to
                // a slowdown in the garbage collection process.
                return connection.Query
                    <QuestionGetManyResponses,
                    AnswerGetResponse,
                    QuestionGetManyResponses>(
                    @"EXEC dbo.Question_GetMany_WithAnswers",
                    map: (q, a) =>
                    {
                        if (!questionDictionary.TryGetValue(q.QuestionId, out QuestionGetManyResponses question))
                        {
                            question = q;
                            question.Answers = new List<AnswerGetResponse>();
                            questionDictionary.Add(question.QuestionId, question);
                        }

                        question.Answers.Add(a);
                        return question;
                    }, splitOn: "QuestionId").Distinct().ToList();
            }

            return connection.Query<QuestionGetManyResponses>(@"EXEC dbo.Question_GetMany");

        }
        public IEnumerable<QuestionGetManyResponses> GetQuestionsWithPaging(int pageNumber, int pageSize, bool includeAnswers)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var questions = connection.Query<QuestionGetManyResponses>(
                @"EXEC dbo.Question_GetMany_WithPaging
                @PageNumber = @PageNumber,
                @PageSize = @PageSize",
                new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

            if (includeAnswers)
                FillAnswersList(connection, questions);
            
            return questions;
        }
        public IEnumerable<QuestionGetManyResponses> GetQuestionsBySearch(string search, bool includeAnswers)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var questions = connection.Query<QuestionGetManyResponses>(
                @"EXEC dbo.Question_GetMany_BySearch
                @Search = @Search",
                new { Search = search });

            // Getting answers in one opend connection using 
            // Answer_Get_ByQuestionId stored procedure for each question
            if (includeAnswers)
                FillAnswersList(connection, questions);

            return questions;
        }

        private static void FillAnswersList(SqlConnection connection, IEnumerable<QuestionGetManyResponses> questions)
        {
            foreach (var question in questions)
            {
                question.Answers = connection.Query<AnswerGetResponse>(
                    @"EXEC dbo.Answer_Get_ByQuestionId
                        @QuestionId = @QuestionId",
                    new { question.QuestionId })
                    .ToList();
            }
        }

        public IEnumerable<QuestionGetManyResponses> GetQuestionsBySearchWithPaging
            (string search, int pageNumber, int pageSize, bool includeAnswers)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var questions = connection.Query<QuestionGetManyResponses>(
                @"EXEC dbo.Question_GetMany_BySearch_WithPaging
                @Search = @Search,
                @PageNumber = @PageNumber,
                @PageSize = @PageSize",
                new
                {
                    Search = search,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

            if (includeAnswers)
                FillAnswersList(connection, questions);

            return questions;

        }

        public IEnumerable<QuestionGetManyResponses> GetUnansweredQuestions()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            return connection
                .Query<QuestionGetManyResponses>(
                    @"EXEC dbo.Question_GetUnanswered");
        }
        
        public QuestionGetSingleResponse PostQuestion(QuestionPostFullRequest question)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var questionId = connection
                .QueryFirst<int>(
                    @"EXEC dbo.Question_Post 
                    @Title = @Title, @Content = @Content, 
                    @UserId = @UserId, @UserName = @UserName, 
                    @Created = @Created",
                    question);

            return GetQuestion(questionId);
        }
        public QuestionGetSingleResponse PutQuestion(int questionId, QuestionPutRequest question)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            connection.Execute(
                    @"EXEC dbo.Question_Put
                    @QuestionId = @QuestionId,
                    @Title = @Title,
                    @Content = @Content",
                    new
                    {
                        QuestionId = questionId,
                        question.Title,
                        question.Content
                    });

            return GetQuestion(questionId);
        }
        public void DeleteQuestion(int questionId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            connection.Execute(
                @"EXEC dbo.Question_Delete 
                @QuestionId = @QuestionId",
                new { QuestionId = questionId });
        }
        public bool QuestionExists(int questionId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            return connection
                .QueryFirst<bool>(
                    @"EXEC dbo.Question_Exists 
                    @QuestionId = @QuestionId",
                    new { QuestionId = questionId });
        }

        public AnswerGetResponse GetAnswer(int answerId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            return connection
                .QueryFirstOrDefault<AnswerGetResponse>(
                    @"EXEC dbo.Answer_Get_ByAnswerId 
                    @AnswerId = @AnswerId",
                    new { AnswerId = answerId });
        }
        public AnswerGetResponse PostAnswer(int questionId, AnswerPostFullRequest answer)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            return connection.QueryFirst<AnswerGetResponse>(
                @"EXEC dbo.Answer_Post
                @QuestionId = @QuestionId,
                @Content = @Content,
                @UserId = @UserId,
                @UserName = @UserName,
                @Created = @Created",
                new
                {
                    QuestionId = questionId,
                    answer.UserId,
                    answer.UserName,
                    answer.Content,
                    answer.Created,
                });
        }

        public async Task<IEnumerable<QuestionGetManyResponses>> GetUnansweredQuestionsAsync()
        {
            using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync();

            return await connection.QueryAsync<QuestionGetManyResponses>(
                @"EXEC dbo.Question_GetUnanswered");
        }
    }
}
