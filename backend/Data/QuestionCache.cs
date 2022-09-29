using Microsoft.Extensions.Caching.Memory;
using QnA.Api.Data.Models;

namespace QnA.Api.Data
{
    public class QuestionCache : IQuestionCache
    {
        private MemoryCache Cache { get; set; }
        public QuestionCache() => Cache = new MemoryCache(new MemoryCacheOptions { SizeLimit = 100 });

        private static string GetCacheKey(int questionId) 
            => $"Question-{questionId}";

        public QuestionGetSingleResponse Get(int questionId)
        {
            Cache.TryGetValue(GetCacheKey(questionId),
                out QuestionGetSingleResponse question);

            return question;
        }

        public void Remove(int questionId)
        {
            Cache.Remove(GetCacheKey(questionId));
        }

        public void Set(QuestionGetSingleResponse question)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1);

            Cache.Set(key: GetCacheKey(question.QuestionId),
                value: question,
                options: cacheEntryOptions);
        }
    }
}
