using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ticketing_api.Infrastructure;
using ticketing_api.Infrastructure.Identity;
using ticketing_api.Models;

namespace ticketing_api.Data
{
    public class DbContextHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationUserManager _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbContextHelper(ApplicationDbContext context, ApplicationUserManager userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public bool AllMigrationsApplied()
        {
            var applied = _context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = _context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        public async Task EnsureSeededAsync()
        {
            //if (_context.AppUser.Any()) return;

            var roles = typeof(AppConstants.AppRoles)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                //.Where(x => x.IsLiteral && !x.IsInitOnly)
                .Select(x => (string)x.GetValue(null)).ToList();

            foreach (var role in roles)
            {
                //try
                //{
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var create = await _roleManager.CreateAsync(new IdentityRole(role));

                    if (!create.Succeeded)
                    {
                        throw new Exception($"Failed to create role: {role}.");
                    }
                }
                //}
                //catch (Exception ex)
                //{
                //}
            }

            if (!_context.JobType.Any())
            {
                foreach (var jobType in InitialData.JobTypes)
                {
                    _context.JobType.Add(new JobType { Name = jobType, IsEnabled = true });
                }
            }

            if (!_context.Setting.Any())
            {
                _context.Setting.Add(new Setting { Key = AppConstants.Settings.AFEPO, Value = "1000" });
                _context.Setting.Add(new Setting { Key = "recordsPerPage", Value = "25" });
                _context.Setting.Add(new Setting { Key = "PaperTime", Value = "11PM" });
                _context.Setting.Add(new Setting { Key = "SpecialHandling", Value = "ND, ND-FB" });
            }

            _context.SaveChanges();

            var appUsers = new List<AppUser>
            {
                new AppUser
                {
                    Id = 1,
                    FirstName = "MICHAEL",
                    MiddleName ="",
                    LastName = "NEWLIN",
                    EffectiveDate = DateTime.Now,
                    IsEnabled = true,
                    Email = "mnewlin@gmail.com",
                    Username = "mnewlin@gmail.com",
                    Role = AppConstants.AppRoles.SysAdmin,
                    EmployeeCode = ""
                },
                //new AppUser
                //{
                //    Id = 2,
                //    FirstName = "TREVOR",
                //    MiddleName ="",
                //    LastName = "O'CONNOR",
                //    EffectiveDate = DateTime.Now,
                //    IsEnabled = true,
                //    Email = "toconnor@rolfsonoil.com",
                //    Username = "toconnor@rolfsonoil.com",
                //    Role = AppConstants.AppRoles.SysAdmin,
                //    EmployeeCode = "0000"
                //}
            };

            const string password = "password";

            foreach (var appUser in appUsers)
            {
                var aspnetUser = await _userManager.FindByEmailAsync(appUser.Email);
                if (aspnetUser != null)
                {

                    var userRoles = await _userManager.GetRolesAsync(aspnetUser);
                    await _userManager.RemoveFromRolesAsync(aspnetUser, userRoles);
                    await _userManager.AddToRoleAsync(aspnetUser, appUser.Role);

                    await _userManager.RemovePasswordAsync(aspnetUser);
                    await _userManager.AddPasswordAsync(aspnetUser, password);
                }
                else
                {
                    var result = await _userManager.CreateUserAsync(appUser.Username, appUser.Email, password, AppConstants.AppRoles.SysAdmin);
                    if (!result.Succeeded) continue;
                    aspnetUser = await _userManager.FindByEmailAsync(appUser.Email);
                }

                _context.SaveChanges();

                var token = await _userManager.GeneratePasswordResetTokenAsync(aspnetUser);

                var existingUser = _context.AppUser.FirstOrDefault(x => x.Id == appUser.Id);
                if (existingUser != null)
                {
                    existingUser.FirstName = appUser.FirstName;
                    existingUser.LastName = appUser.LastName;
                    existingUser.IsEnabled = appUser.IsEnabled;
                    existingUser.Email = appUser.Email;
                    existingUser.Role = appUser.Role;

                    existingUser.SetPasswordToken = token;
                    existingUser.AspNetUserId = aspnetUser?.Id;
                    _context.AppUser.Update(existingUser);
                }
                else
                {
                    appUser.SetPasswordToken = token;
                    appUser.AspNetUserId = aspnetUser?.Id;
                    _context.AppUser.Add(appUser);
                }

                _context.SaveChanges();
            }
    }
    }
}
