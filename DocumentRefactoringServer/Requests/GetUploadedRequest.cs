using DocumentRefactoringServer.Responses;
using MediatR;

namespace DocumentRefactoringServer.Requests
{
    public class GetUploadedRequest : IRequest<IReadOnlyList<DocumentResponse>>
    {
    }
}
