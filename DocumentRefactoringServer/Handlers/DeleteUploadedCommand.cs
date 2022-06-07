using MediatR;
using DocumentRefactoringServer.Requests;
using DocumentRefactoringServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentRefactoringServer.Handlers
{
    public class DeleteUploadedCommand : IRequestHandler<DeleteUploadedRequest, bool>
    {
        private readonly DataContext _dataContext;

        public DeleteUploadedCommand(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> Handle(DeleteUploadedRequest request, CancellationToken cancellationToken)
        {
            var document = await _dataContext.UploadedDocuments.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (document == null) return false;

            _dataContext.UploadedDocuments.Remove(document);
            var res = await _dataContext.SaveChangesAsync();
            return res > 0;
        }
    }
}
