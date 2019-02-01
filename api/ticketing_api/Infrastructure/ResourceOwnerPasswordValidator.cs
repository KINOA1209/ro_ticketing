using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using ticketing_api.Infrastructure.Identity;

namespace ticketing_api.Infrastructure
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ApplicationUserManager _userManager;

        public ResourceOwnerPasswordValidator(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            string username = context.UserName, password = context.Password;

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "User record does not exist.");
                return;
            }

            var success = await _userManager.CheckPasswordAsync(user, password);

            if (!success)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Username or Password was incorrect.");
                return;
            }
            context.Result = new GrantValidationResult(user.Id, "password");
        }
    }
}
