using MediatR;

namespace DocumentRefactoringServer.Requests
{
    public class RegistrationRequest : IRequest<bool>
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
