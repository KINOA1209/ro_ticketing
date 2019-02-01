using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sieve.Services;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Infrastructure.Identity;
using ticketing_api.Models;

namespace ticketing_api.Controllers.BaseController
{
    public class IdentityBaseController : BaseController
    {
        protected readonly ApplicationUserManager UserManager;
        protected readonly IEmailSender EmailSender;

        public IdentityBaseController(
            ApplicationDbContext context,
            ApplicationUserManager userManager,
            IEmailSender emailSender,
            ISieveProcessor sieveProcessor

        ) : base(context, emailSender, sieveProcessor)
        {
            UserManager = userManager;
            EmailSender = emailSender;
        }

        private AppUser _adminUser;
        protected AppUser AdminUser
        {
            get
            {
                if (_adminUser == null)
                {
                    var adminId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _adminUser = _context.AppUser.FirstOrDefault(x => x.AspNetUserId == adminId);
                }
                return _adminUser;
            }
        }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
        }
    }
}