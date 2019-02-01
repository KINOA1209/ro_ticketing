using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ticketing_api.Infrastructure.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private readonly ILogger<UserManager<ApplicationUser>> logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, 
            IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, RoleManager<IdentityRole> roleManager,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            this.logger = logger;
            this._roleManager = roleManager;
        }

        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<IdentityResult> CreateUserAsync(string username, string email, string password, string role)
        {
            //var user = new ApplicationUser { UserName = username, Email = email };
            var user = new ApplicationUser { UserName = username.Trim(), Email = email.Trim() };
            var result = await CreateAsync(user, password);

            if (result != null && result.Succeeded)
            {
                logger.LogInformation($"User {username} created.");

                if (!string.IsNullOrWhiteSpace(role))
                {
                    role = role.ToUpper();
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        var create = await _roleManager.CreateAsync(new IdentityRole(role));
                        if (!create.Succeeded)
                        {
                            throw new Exception($"Failed to create role: {role}.");
                        }
                    }

                    result = await AddToRoleAsync(user, role);
                }
            }

            return result;
        }

        /// <summary>
        /// Update the password for the user.
        /// </summary>
        public async Task<IdentityResult> ChangePasswordAsync(string username, string currentPassword, string newPassword)
        {
            var user = await FindByEmailAsync(username);
            if (user == null) return null;

            var result = await base.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    Logger.LogInformation(error.Description);
                }
            }

            return result;
        }

        //public Task RemoveFromRolesAsync(ApplicationUser aspnetUser)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<IdentityResult> UpdatePasswordAsync(ApplicationUser netUser, string newPassword)
        {
            if (netUser == null) return null;

            string resetToken = await GeneratePasswordResetTokenAsync(netUser);
            var result = await ResetPasswordAsync(netUser, resetToken, newPassword);

            //var result = await base.UpdatePasswordHash(user, newPassword, false);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    Logger.LogInformation(error.Description);
                }
            }

            return result;
        }
    }
}
