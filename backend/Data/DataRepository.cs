using Dapper;
using static Dapper.SqlMapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QnA.Api.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace QnA.Api.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;
        public DataRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        public async Task<AnswerGetResponse> GetAnswer(int answerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<AnswerGetResponse>(@"EXEC dbo.Answer_Get_ByAnswerId @AnswerId = @AnswerId", new { AnswerId = answerId });
            }
        }

        public async Task<QuestionGetSingleResponse> GetQuestion(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (GridReader results = await connection.QueryMultipleAsync(
                    @"EXEC dbo.Question_GetSingle @QuestionId = @QuestionId; 
                      EXEC dbo.Answer_Get_ByQuestionId @QuestionId = @QuestionId",
                    new { QuestionId = questionId }))
                {
                    var question = (await results.ReadAsync<QuestionGetSingleResponse>()).FirstOrDefault();
                    if (question != null)
                    {
                        question.Answers = (await results.ReadAsync<AnswerGetResponse>()).ToList();
                    }
                    return question;
                }
            }
        }

        public async Task<IEnumerable<QuestionGetManyResponses>> GetQuestions()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<QuestionGetManyResponses>("EXEC dbo.Question_GetMany");
            }
        }

        public async Task<IEnumerable<QuestionGetManyResponses>> GetQuestionsWithAnswers()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var questionDictionary = new Dictionary<int, QuestionGetManyResponses>();
                return (await connection.QueryAsync<QuestionGetManyResponses, AnswerGetResponse, QuestionGetManyResponses>("EXEC dbo.Question_GetMany_WithAnswers",
                  map: (q, a) =>
                  {
                      QuestionGetManyResponses question;

                      if (!questionDictionary.TryGetValue(q.QuestionId, out question))
                      {
                          question = q;
                          question.Answers = new List<AnswerGetResponse>();
                          questionDictionary.Add(question.QuestionId, question);
                      }
                      question.Answers.Add(a);
                      return question;
                  },
                  splitOn: "QuestionId"))
                  .Distinct()
                  .ToList();
            }
        }

        public async Task<IEnumerable<QuestionGetManyResponses>> GetQuestionsBySearch(string search)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<QuestionGetManyResponses>(@"EXEC dbo.Question_GetMany_BySearch 
                    @Search = @Search",
                    new { Search = search });
            }
        }

        public async Task<IEnumerable<QuestionGetManyResponses>> GetQuestionsBySearchWithPaging(string search, int pageNumber, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var parameters = new { Search = search, PageNumber = pageNumber, PageSize = pageSize };
                return await connection.QueryAsync<QuestionGetManyResponses>(@"EXEC dbo.Question_GetMany_BySearch_WithPaging
                    @Search = @Search, @PageNumber = @PageNumber, @PageSize = @PageSize",
                    parameters);
            }
        }

        public async Task<IEnumerable<QuestionGetManyResponses>> GetUnansweredQuestions()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<QuestionGetManyResponses>("EXEC dbo.Question_GetUnanswered");
            }
        }

        public async Task<bool> QuestionExists(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstAsync<bool>(@"EXEC dbo.Question_Exists 
                    @QuestionId = @QuestionId",
                    new { QuestionId = questionId });
            }
        }

        public async Task<QuestionGetSingleResponse> PostQuestion(QuestionPostFullRequest question)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var questionId = await connection.QueryFirstAsync<int>(@"EXEC dbo.Question_Post 
                    @Title = @Title, @Content = @Content, 
                    @UserId = @UserId,  @UserName = @UserName, 
                    @Created = @Created",
                    question);
                return await GetQuestion(questionId);
            }
        }

        public async Task<QuestionGetSingleResponse> PutQuestion(int questionId, QuestionPutRequest question)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(@"EXEC dbo.Question_Put 
                    @QuestionId = @QuestionId, @Title = @Title, @Content = @Content",
                    new { QuestionId = questionId, question.Title, question.Content });
                return await GetQuestion(questionId);
            }
        }

        public async Task DeleteQuestion(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(@"EXEC dbo.Question_Delete 
                    @QuestionId = @QuestionId",
                    new { QuestionId = questionId });
            }
        }

        public async Task<AnswerGetResponse> PostAnswer(AnswerPostFullRequest answer)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstAsync<AnswerGetResponse>(@"EXEC dbo.Answer_Post 
                    @QuestionId = @QuestionId, @Content = @Content, @UserId = @UserId, @UserName = @UserName, @Created = @Created",
                    answer);
            }
        }

        public async Task<IEnumerable<QuestionGetManyResponses>> GetQuestionsWithPaging(int pageNumber, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var parameters = new {PageNumber = pageNumber, PageSize = pageSize };

                return await connection.QueryAsync<QuestionGetManyResponses>(@"EXEC dbo.Question_GetMany_WithPaging
                    @PageNumber = @PageNumber, @PageSize = @PageSize",
                    parameters);
            }
        }
    }
}
