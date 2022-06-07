using MediatR;

namespace DocumentRefactoringServer.Requests
{
    public class UploadingRequest : IRequest<bool>
    {
        public IFormCollection Form { get; set; }
        public int Id { get; set; }
    }
}
