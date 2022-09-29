using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using QnA.Api.Data;

namespace QnA.Api.Authorization
{
    public class MustBeQuestionAuthorHandler : AuthorizationHandler<MustBeQuestionAuthorRequirement>
    {
        private readonly IDataRepository _dataRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MustBeQuestionAuthorHandler(IDataRepository dataRepository, IHttpContextAccessor httpContextAccessor)
        {
            _dataRepository = dataRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeQuestionAuthorRequirement requirement)
        {
            // check that the user is authenticated
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            // get the question id from the request
            var questionId = _httpContextAccessor.HttpContext.Request.RouteValues["questionId"];

            int questionIdAsInteger = System.Convert.ToInt32(questionId);

            // get the user id from the name identifier claim
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // get the question from the data repository
            var question = await _dataRepository.GetQuestion(questionIdAsInteger);

            if(question == null)
            {
                // let it through so the controller can return a 404
                context.Succeed(requirement);
                return;
            }

            // return failure if the user id in the question from the data repository is different to the user id in the request
            if (question.UserId != userId)
            {
                context.Fail();
                return;
            }

            // return success if we manage to get here
            context.Succeed(requirement);
        }
    }
}