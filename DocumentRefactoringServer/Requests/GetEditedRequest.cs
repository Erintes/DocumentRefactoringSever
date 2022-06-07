using DocumentRefactoringServer.Responses;
using MediatR;

namespace DocumentRefactoringServer.Requests
{
    public class GetEditedRequest : IRequest<IReadOnlyList<DocumentResponse>>
    {
    }
}