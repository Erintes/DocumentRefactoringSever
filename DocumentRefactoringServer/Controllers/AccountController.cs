using MediatR;
using Microsoft.AspNetCore.Mvc;
using DocumentRefactoringServer.Requests;
using DocumentRefactoringServer.Responses;
using DocumentRefactoringServer.Models;

namespace DocumentRefactoringServer.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator, DataContext dataContext)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("/registration")]
        public async Task<IActionResult> Registration([FromForm]RegistrationRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result ? Ok() : BadRequest();
        }

        [HttpPost]
        [Route("/authorization")]
        public async Task<AuthorizationResponse> Authorization([FromForm]AuthorizationRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result;
        }
    }
}