using MediatR;
using DocumentRefactoringServer.Requests;
using DocumentRefactoringServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentRefactoringServer.Handlers
{
    public class DeleteEditedCommand : IRequestHandler<DeleteEditedRequest, bool>
    {
        private readonly DataContext _dataContext;

        public DeleteEditedCommand(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> Handle(DeleteEditedRequest request, CancellationToken cancellationToken)
        {
            var document = await _dataContext.EditedDocuments.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (document == null) return false;

            _dataContext.EditedDocuments.Remove(document);
            var res = await _dataContext.SaveChangesAsync();
            return res > 0;
        }
    }
}
