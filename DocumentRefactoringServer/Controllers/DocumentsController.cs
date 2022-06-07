using MediatR;
using Microsoft.AspNetCore.Mvc;
using DocumentRefactoringServer.Requests;
using DocumentRefactoringServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using DocumentRefactoringServer.Responses;

namespace DocumentRefactoringServer.Controllers
{
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DocumentsController(DataContext dataContext, IMediator mediator, IWebHostEnvironment environment)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("/uploading")]
        public async Task<RedirectResult> Uploading([FromForm] UploadingRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            return RedirectPermanent("/");
        }

        [HttpPost]
        [Route("/editing")]
        public async Task<IActionResult> Editing(EditingRequest request, CancellationToken cancellationToken)
        {
            var res = await _mediator.Send(request, cancellationToken);
            return res ? Ok() : BadRequest();
        }

        [HttpGet]
        [Route("/getUploaded")]
        public async Task<IReadOnlyCollection<DocumentResponse>> GetUploadedDocuments ([FromQuery] GetUploadedRequest request, CancellationToken cancellationToken)
        {
            var res = await _mediator.Send(request, cancellationToken);
            return res;
        }

        [HttpGet]
        [Route("/getEdited")]
        public async Task<IReadOnlyCollection<DocumentResponse>> GetEditedDocuments([FromQuery] GetEditedRequest request, CancellationToken cancellationToken)
        {
            var res = await _mediator.Send(request, cancellationToken);
            return res;
        }

        [HttpDelete]
        [Route("/deleteUploaded/{id}")]
        public async Task<IActionResult> DeleteUploaded([FromRoute] DeleteUploadedRequest request, CancellationToken cancellationToken)
        {
            var res = await _mediator.Send(request, cancellationToken);
            return res ? Ok() : BadRequest();
        }

        [HttpDelete]
        [Route("/deleteEdited/{id}")]
        public async Task<IActionResult> DeleteEdited([FromRoute] DeleteEditedRequest request, CancellationToken cancellationToken)
        {
            var res = await _mediator.Send(request, cancellationToken);
            return res ? Ok() : BadRequest();
        }
    }
}
