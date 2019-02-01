using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Newtonsoft.Json;
using ticketing_api.Data;
using ticketing_api.Infrastructure.Identity;

namespace ticketing_api.Infrastructure
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationDbContext dbContext;


        public ProfileService(ApplicationUserManager userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            this.dbContext = dbContext;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var claims = context.Subject.Claims.ToList();

            string userid = context.Subject.GetSubjectId();
            if (string.IsNullOrEmpty(userid))
                throw new ArgumentException("Sub claim not found for user.");

            //find users by id
            var user = await _userManager.FindByIdAsync(userid);
            if (user == null)
                throw new ArgumentException("User not found by user id.");

            if (!claims.Exists(x => x.Type == JwtClaimTypes.Email)) claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
            if (!claims.Exists(x => x.Type == JwtClaimTypes.Subject)) claims.Add(new Claim(JwtClaimTypes.Subject, userid));

            var appUser = dbContext.AppUser.FirstOrDefault(x => x.AspNetUserId == userid);
            if (!claims.Exists(x => x.Type == JwtClaimTypes.Name))
            {
                claims.Add(appUser != null
                    ? new Claim(JwtClaimTypes.Name, appUser.FullName)
                    : new Claim(JwtClaimTypes.Name, user.UserName));
            }
            var roleId = dbContext.UserRole.Where(x => x.UserId.Equals(userid)).Select(x => x.RoleId).First();
            //check for duplicate role entry
            //if (!claims.Exists(x => x.Type == "roleid" && x.Value != roleId))
            //{
                claims.Add(new Claim("roleid", roleId));
            //}

            //find roles for user
            var roles = await _userManager.GetRolesAsync(user);
            //add roles in uppercase to claims
            foreach (var role in roles)
            {
                var claim = new Claim(JwtClaimTypes.Role, role.ToUpper());
                if (claims.Where(x => x.Type == JwtClaimTypes.Role).All(x => string.Compare(x.Value, role, StringComparison.OrdinalIgnoreCase) != 0))
                {
                    if (role.ToUpper() == "DRIVER")
                    {
                        var driver = dbContext.Driver.FirstOrDefault(x => x.AppUserId == appUser.Id);
                        if(driver!=null)
                        claims.Add(new Claim("driverId", driver.Id.ToString()));
                    }
                    else if (role.ToUpper() == "SALES")
                    {
                        var sales = dbContext.SalesRep.FirstOrDefault(x => x.AppUserId == appUser.Id);
                        if (sales != null)
                            claims.Add(new Claim("salesRepId", sales.Id.ToString()));
                    }

                    claims.Add(claim);
                }

                break;
            }

            //var userClaims = await _userManager.GetClaimsAsync(user);
            //foreach (var userClaim in userClaims)
            //{
            //    claims.Add(new Claim(userClaim.Type, userClaim.Value));
            //}

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }

    }
}
