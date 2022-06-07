using DocumentRefactoringServer.Requests;
using DocumentRefactoringServer.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DocumentRefactoringServer.Handlers
{
    public class UploadingCommand : IRequestHandler<UploadingRequest, bool>
    {
        private readonly DataContext _dataContext;

        public UploadingCommand(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> Handle(UploadingRequest request, CancellationToken cancellationToken)
        {
            IFormFileCollection files = request.Form.Files;
            var uploadPath = "C:\\Users\\mrcur\\Desktop\\DocumentRefactoringServer\\DocumentRefactoringServer\\wwwroot\\uploads\\";
            foreach (var file in files)
            {
                string fullPath = $"{uploadPath}/{file.FileName}";

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                var user = await _dataContext.Users.FirstAsync(x => x.Id == request.Id);
                _dataContext.UploadedDocuments.Add(new UploadedDocument
                {
                    Name = file.FileName,
                    Path = "/uploads/" + file.FileName,
                    User = user
                });
            }
            var res = await _dataContext.SaveChangesAsync();
            return res > 0;
        }
    }
}
