using DocumentRefactoringServer.Models;
using DocumentRefactoringServer.Requests;
using MediatR;

namespace Blogs.Startup.Features.Account
{
    public class RegisterNewPersonHandler : IRequestHandler<RegistrationRequest, bool>
    {
        private DataContext _dataContext;

        public RegisterNewPersonHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> Handle(RegistrationRequest request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Login = request.Login,
                Password = request.Password
            };
            await _dataContext.Users.AddAsync(user);

            var result = await _dataContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
