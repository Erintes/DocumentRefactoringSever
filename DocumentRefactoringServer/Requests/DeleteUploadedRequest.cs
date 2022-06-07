using MediatR;

namespace DocumentRefactoringServer.Requests
{
    public class DeleteUploadedRequest : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
