using MediatR;
using DocumentRefactoringServer.Responses;
using DocumentRefactoringServer.Requests;
using DocumentRefactoringServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentRefactoringServer.Handlers
{
    public class GetEditedCommand : IRequestHandler<GetEditedRequest, IReadOnlyList<DocumentResponse>>
    {
        private readonly DataContext _dataContext;

        public GetEditedCommand(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IReadOnlyList<DocumentResponse>> Handle(GetEditedRequest request, CancellationToken cancellationToken)
        {
            var documents = await _dataContext.EditedDocuments.Include(x => x.User).ToListAsync();
            return documents.Select(x => new DocumentResponse()
            {
                Id = x.Id,
                Name = x.Name,
                Path = x.Path,
                UserId = x.User.Id
            }).ToList();
        }
    }
}
