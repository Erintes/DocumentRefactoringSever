using MediatR;
using DocumentRefactoringServer.Responses;
using DocumentRefactoringServer.Requests;
using DocumentRefactoringServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentRefactoringServer.Handlers
{
    public class GetUploadedCommand : IRequestHandler<GetUploadedRequest, IReadOnlyList<DocumentResponse>>
    {
        private readonly DataContext _dataContext;

        public GetUploadedCommand(DataContext dataContext)
        {
            _dataContext = dataContext;
            var tom = new User() { Login = "Tom", Password = "password" };
            var anna = new User() { Login = "Anna", Password = "password" };
            if (!_dataContext.Users.Any())
            {
                _dataContext.Users.Add(tom);
                _dataContext.Users.Add(anna);
                _dataContext.SaveChanges();
            }
        }

        public async Task<IReadOnlyList<DocumentResponse>> Handle(GetUploadedRequest request, CancellationToken cancellationToken)
        {
            var documents = await _dataContext.UploadedDocuments.Include(x => x.User).ToListAsync();
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
