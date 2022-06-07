using DocumentRefactoringServer.Requests;
using DocumentRefactoringServer.Models;
using DocumentRefactoring;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Packaging;

namespace DocumentRefactoringServer.Handlers
{
    public class EditingCommand : IRequestHandler<EditingRequest, bool>
    {
        private readonly DataContext _dataContext;

        public EditingCommand(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> Handle(EditingRequest request, CancellationToken cancellationToken)
        {
            var document = await _dataContext.UploadedDocuments.FirstOrDefaultAsync(x => x.Id == request.DocumentId);
            if (document == null) return false;

            var standard = GetStandard(request.Standard);
            if (standard == null) return false;

            var checkedDocument = "C:\\Users\\mrcur\\Desktop\\DocumentRefactoringServer\\DocumentRefactoringServer\\wwwroot\\edited\\" + document.Name;
            var a = "C:\\Users\\mrcur\\Desktop\\DocumentRefactoringServer\\DocumentRefactoringServer\\wwwroot\\uploads\\" + document.Name;
            File.Copy(a, checkedDocument, true);

            using(var editor = new Editor(WordprocessingDocument.Open(checkedDocument, true), standard))
            {
                editor.EditDocument();
                editor.AddComments();
            }
            var user = await _dataContext.Users.FirstAsync(x => x.Id == request.Id);
            await _dataContext.EditedDocuments.AddAsync(new EditedDocument() 
            { 
                Name = document.Name, 
                Path = "/edited/" + document.Name, 
                User = user 
            });
            var res = await _dataContext.SaveChangesAsync();

            return res > 0;
        }

        private IDocumentStandard GetStandard(string standardName)
        {
            switch (standardName)
            {
                case "ГОСТ 7.32-2017":
                    return new Standart7_32_2017();
                default: return null;
            }
        }
    }
}
