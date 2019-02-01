using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sieve.Models;
using Sieve.Services;
using ticketing_api.Controllers.BaseController;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Infrastructure.Identity;
using ticketing_api.Models;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : IdentityBaseController
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, ApplicationUserManager userManager, IEmailSender emailSender, ILogger<UserController> logger, ISieveProcessor sieveProcessor)
            : base(context, userManager, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel)
        {
            var users = _context.AppUser.Where(x => !x.IsDeleted && x.ClientId == AdminUser.ClientId).AsQueryable();

            //filter admin1 users
            if (!User.HasClaim(ClaimTypes.Role, AppConstants.AppRoles.SysAdmin))
            {
                users = users.Where(x => x.Role != AppConstants.AppRoles.SysAdmin);
            }
            var total = await users.CountAsync();

            users = _sieveProcessor.Apply(sieveModel, users);

            var usersNew = users;
            foreach (var usr in usersNew)
            {
                usr.Password = null;
                usr.SetPasswordToken = null;
            }

            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, users);
            return Ok(data);

        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var appUser = await _context.AppUser.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id && x.ClientId == AdminUser.ClientId);
            if (appUser == null)
                return NotFound();

            appUser.Clean();

            return Ok(appUser);
        }

        /// <summary>
        /// Create new user from user management UI
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        //[Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]AppUser appUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userService = new AppUserService(_context, UserManager);
            var result = await userService.CreateUserAsync(appUser, AdminUser);

            if (!string.IsNullOrEmpty(result.Message))
                return BadRequest(result.Message);

            if (result.IdentityResult != null && !result.IdentityResult.Succeeded)
            {
                AddErrors(result.IdentityResult);
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"User {result.AppUser.Id} created.");

            appUser.Password = null;
            appUser.SetPasswordToken = null;

            return Ok(result.AppUser);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromBody]AppUser appUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //check that id is correct and that the user is an admin for this Id
            var checkUser = await _context.AppUser.FirstOrDefaultAsync(x => !x.IsDeleted && x.ClientId == AdminUser.ClientId);
            if (checkUser == null)
            {
                ModelState.AddModelError("2", "Admin does not have access to this user.");
                return BadRequest(ModelState);
            }

            if (checkUser.Role.ToUpper() != "SYSADMIN") {
                ModelState.AddModelError("2", "Must be admin to update user.");
                return BadRequest(ModelState);
            }

            var user = _context.AppUser.AsNoTracking().FirstOrDefault(a => a.Id == appUser.Id);
            if (user == null)
                return BadRequest("User does not exist");

            appUser.AspNetUserId = user.AspNetUserId;
            appUser.SetPasswordToken = user.SetPasswordToken;
            appUser.Clean();

            _context.AppUser.Update(appUser);
            await _context.SaveChangesAsync();

            var netUser = await UserManager.FindByIdAsync(appUser.AspNetUserId);
            if (!string.Equals(netUser.UserName, appUser.Username, StringComparison.OrdinalIgnoreCase))
            {
                netUser.Email = appUser.Email;
                netUser.UserName = appUser.Username;
                netUser.NormalizedEmail = appUser.Email.ToUpper();
                netUser.NormalizedUserName = appUser.Username.ToUpper();
                await UserManager.UpdateAsync(netUser);
            }

            var existingRole = await _context.UserRole.FirstAsync(x => x.UserId == appUser.AspNetUserId);
            var existingRoleName = await _context.AspNetRoles.FirstAsync(x => x.Id == existingRole.RoleId);
            if (!String.Equals(existingRoleName.Name, appUser.Role, StringComparison.OrdinalIgnoreCase))
            {
                //if (existingRoleName.Name == "DRIVER" || existingRoleName.Name == "SALES")
                //{
                //    return BadRequest("Cannot change user from role of DRIVER or SALES.");
                //}

                await UserManager.RemoveFromRoleAsync(netUser, existingRoleName.Name);
                await UserManager.AddToRoleAsync(netUser, appUser.Role.ToUpper());
            }

            // Update user password only if this field is set
            if (!string.IsNullOrEmpty(appUser.Password))
            {
                try
                {
                    var result = await UserManager.UpdatePasswordAsync(netUser, appUser.Password);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            //send email to admin to let them know new user was created.
            //await EmailSender.SendAdminAlertUpdateUserAsync(AdminUser, appUser);

            _logger.LogInformation($"User {appUser.Id} updated.");

            appUser.Password = null;
            appUser.SetPasswordToken = null;
            return Ok(appUser);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //TODO: check that id is correct and that the user is an admin for this Id

            var appUser = _context.AppUser.FirstOrDefault(x => x.Id == id);
            if (appUser == null)
                return BadRequest();

            _context.AppUser.Remove(appUser);
            await _context.SaveChangesAsync();

            //send email to admin to let them know new user was created.
            //await EmailSender.SendAdminAlertDeleteUserAsync(AdminUser, appUser);
            _logger.LogInformation($"User {id} deleted.");

            return Ok();
        }

        [HttpGet("permissions")]
        public async Task<IActionResult> GetUserPermissions()
        {
            var roleName = User.FindFirst(ClaimTypes.Role).Value;
            var roleId = await _context.Roles.FirstAsync(x => x.Name == roleName);
            var permissions = await _context.Permission
                .Join(_context.Module, a => a.ModuleId, b => b.Id, (a, b) => new
                {
                    a.Id,
                    a.RoleId,
                    a.ModuleId,
                    b.ModuleName,
                    a.IsRead,
                    a.IsCreate,
                    a.IsUpdate,
                    a.IsDelete
                }).Where(x => x.RoleId == roleId.Id).ToListAsync();

            if (permissions == null)
            {
                return NotFound();
            }

            return Ok(permissions);
        }
    }
}