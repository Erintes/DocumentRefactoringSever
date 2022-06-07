using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DocumentRefactoringServer.Requests;
using DocumentRefactoringServer.Responses;
using DocumentRefactoringServer.Models;
using DocumentRefactoringServer;

namespace Blogs.Startup.Features.Account
{
    public class AuthorizationCommandHandler : IRequestHandler<AuthorizationRequest, AuthorizationResponse>
    {
        private DataContext _dataContext;

        public AuthorizationCommandHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Task<AuthorizationResponse> Handle(AuthorizationRequest request, CancellationToken cancellationToken)
        {
            var identity = GetIdentity(request);
            if (identity == null) return Task.FromResult<AuthorizationResponse>(null);

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new AuthorizationResponse()
            {
                Token = encodedJwt,
                Id = int.Parse(identity.Name),
                Login = request.Login
            };

            return Task.FromResult(response);
        }

        private ClaimsIdentity? GetIdentity(AuthorizationRequest request)
        {
            User user = _dataContext.Users.FirstOrDefault(x => x.Login == request.Login && x.Password == request.Password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                    claims, 
                    "Token", 
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
    }
}
