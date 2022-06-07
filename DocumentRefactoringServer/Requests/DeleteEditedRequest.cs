using MediatR;

namespace DocumentRefactoringServer.Requests
{
    public class DeleteEditedRequest : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
