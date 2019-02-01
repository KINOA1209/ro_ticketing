using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Infrastructure.Identity;
using ticketing_api.Models;

namespace ticketing_api.Services
{
    public class CreateUserResponse
    {
        public IdentityResult IdentityResult { get; set; }
        public string Message { get; set; }
        public AppUser AppUser { get; set; }
    }

    public class AppUserService
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _context;

        public AppUserService(ApplicationDbContext context, ApplicationUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<CreateUserResponse> CreateUserAsync(AppUser appUser, AppUser adminUser)
        {
            //check if the user already exists for that email
            if (!string.IsNullOrEmpty(appUser.Email))
            {
                var emailUser = await _userManager.FindByEmailAsync(appUser.Email);
                if (emailUser != null)
                {
                    return new CreateUserResponse {Message = "Email [" + appUser.Email + "] already exists"};
                }
            }

            //check if the user already exists for that email
            if (await _context.AppUser.AnyAsync(x => x.Username == appUser.Username))
            {
                return new CreateUserResponse { Message = "Username already exists" };
            }

            appUser.Id = 0;

            var username = !string.IsNullOrEmpty(appUser.Username) ? appUser.Username : appUser.Email;
            var email = string.IsNullOrEmpty(appUser.Email) ? "employee_" + appUser.Username + AppConfiguration.Instance.SendGrid["Domain"] : appUser.Email;

            //create new user in aspnet identity system 
            var result = await _userManager.CreateUserAsync(username, email, appUser.Password, appUser.Role);
            if (!result.Succeeded)
            {
                return new CreateUserResponse { IdentityResult = result};
            }

            //find user just created
            var aspnetUser = await _userManager.FindByNameAsync(username);

            ////generate a reset password token for this user
            //var token = await _userManager.GeneratePasswordResetTokenAsync(aspnetUser);
            //token = HttpUtility.UrlEncode(token);
            //appUser.SetPasswordToken = token;
            appUser.AspNetUserId = aspnetUser.Id;
            appUser.ClientId = adminUser.ClientId;
            appUser.IsEnabled = true;
            appUser.Clean();
            await _context.AppUser.AddAsync(appUser);
            await _context.SaveChangesAsync();

            //send email with password reset token, so they can set their password
            //await EmailSender.SendNewUserEmailAsync(AdminUser, appUser, token);
            //await EmailSender.SendAdminAlertNewUserAsync(AdminUser, appUser);

            appUser.Password = null;
            return new CreateUserResponse { AppUser = appUser };
        }

        //public async Task<CreateUserResponse> UpdateUserPasswordAsync(ApplicationUser appUser, AppUser adminUser)
        //{
        //    var result = await _userManager.UpdatePasswordAsync(appUser,);
        //    if (!result.Succeeded)
        //    {
        //        return new CreateUserResponse { IdentityResult = result };
        //    }           
        //    return new CreateUserResponse { AppUser = appUser };
        //}
    }
}
