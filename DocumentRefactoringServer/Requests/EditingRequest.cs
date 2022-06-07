using MediatR;

namespace DocumentRefactoringServer.Requests
{
    public class EditingRequest : IRequest<bool>
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Standard { get; set; }
    }
}
