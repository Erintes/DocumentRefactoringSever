using DocumentRefactoringServer.Responses;
using MediatR;

namespace DocumentRefactoringServer.Requests
{
    public class AuthorizationRequest : IRequest<AuthorizationResponse>
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
