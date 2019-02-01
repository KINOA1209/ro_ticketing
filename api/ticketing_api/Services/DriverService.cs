using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sieve.Services;
using ticketing_api.Data;
using ticketing_api.Models;
using ticketing_api.Models.Views;

namespace ticketing_api.Services
{
    public class DriverService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISieveProcessor _sieveProcessor;

        public DriverService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
        }

        public IQueryable<DriverView> GetDriverQuery()
        {
            return (from user in _context.AppUser
                join driver in _context.Driver on user.Id equals driver.AppUserId
                select new DriverView
                {
                    Id = driver.Id,
                    AppUserId = driver.AppUserId,
                    AspNetUserId = user.AspNetUserId,
                    Role = user.Role,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmployeeCode = user.EmployeeCode,
                    EffectiveDate = user.EffectiveDate,
                    IsEnabled = driver.IsEnabled && user.IsEnabled,
                    IsVisible = driver.IsVisible && user.IsVisible,
                    PhoneNumber = user.PhoneNumber,
                    Username = user.Username,
                    Position = user.Position
                }).AsQueryable();
        }

        public DriverView GetDriver(int id)
        {
            var driver = (from user in _context.AppUser
                join d in _context.Driver on user.Id equals d.AppUserId
                where d.Id == id
                select new DriverView
                {
                    Id = d.Id,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmployeeCode = user.EmployeeCode,
                    EffectiveDate = user.EffectiveDate,
                    IsEnabled = d.IsEnabled && user.IsEnabled,
                    IsVisible = d.IsVisible && user.IsVisible,
                    PhoneNumber = user.PhoneNumber,
                    Username = user.Username,
                    Position = user.Position,
                    AppUserId = d.AppUserId,
                    AspNetUserId = user.AspNetUserId,
                    Role = user.Role,
                }).FirstOrDefault();

            return driver;
        }

        public int GetDriverIdFromAppUserId(int id)
        {
            var driver = (from user in _context.AppUser
                          join d in _context.Driver on user.Id equals d.AppUserId
                          where d.AppUserId == id
                          select new
                          {
                              d.Id,
                          }).FirstOrDefault();
            return driver?.Id ?? 0;
        }

        public DriverView CombineDriverView(Driver driver, AppUser user)
        {
            if (driver == null || user == null) return new DriverView();

            return new DriverView
            {
                Id = driver.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                EmployeeCode = user.EmployeeCode,
                EffectiveDate = user.EffectiveDate,
                IsEnabled = driver.IsEnabled && user.IsEnabled,
                IsVisible = driver.IsVisible && user.IsVisible,
                PhoneNumber = user.PhoneNumber,
                Username = user.Username,
                Position = user.Position
            };
        }
    }
}
