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
    public class SalesRepService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISieveProcessor _sieveProcessor;

        public SalesRepService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
        }

        public IQueryable<SalesRepView> GetSalesRepQuery()
        {
            return (from user in _context.AppUser
                join rep in _context.SalesRep on user.Id equals rep.AppUserId
                select new SalesRepView
                {
                    Id = rep.Id,
                    AppUserId = rep.AppUserId,
                    AspNetUserId = user.AspNetUserId,
                    Role = user.Role,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmployeeCode = user.EmployeeCode,
                    EffectiveDate = user.EffectiveDate,
                    IsEnabled = rep.IsEnabled && user.IsEnabled,
                    IsVisible = rep.IsVisible && user.IsVisible,
                    PhoneNumber = user.PhoneNumber,
                    Username = user.Username,
                    Position = user.Position,
                    Notes = rep.Notes
                }).AsQueryable();
        }

        public SalesRepView GetSalesRep(int id)
        {
            var view = (from user in _context.AppUser
                join d in _context.SalesRep on user.Id equals d.AppUserId
                where d.Id == id
                select new SalesRepView
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
                    Notes = d.Notes,
                    AspNetUserId = user.AspNetUserId,
                    Role = user.Role, 
                }).FirstOrDefault();

            return view;
        }

        public int GetSalesIdFromAppUserId(int id)
        {
            var sales = (from user in _context.AppUser
                          join s in _context.SalesRep on user.Id equals s.AppUserId
                          where s.AppUserId == id
                          select new
                          {
                              s.Id,
                          }).FirstOrDefault();
            return sales?.Id ?? -1;
        }

        public SalesRepView CombineSalesRepView(SalesRep sales, AppUser user)
        {
            if (sales == null || user == null) return new SalesRepView();

            return new SalesRepView
            {
                Id = sales.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                EmployeeCode = user.EmployeeCode,
                EffectiveDate = user.EffectiveDate,
                IsEnabled = sales.IsEnabled && user.IsEnabled,
                IsVisible = sales.IsVisible && user.IsVisible,
                PhoneNumber = user.PhoneNumber,
                Username = user.Username,
                Position = user.Position,
                Role = user.Role,
                AspNetUserId = user.AspNetUserId,
                AppUserId = sales.AppUserId,
                Notes = sales.Notes
            };
        }
    }
}
