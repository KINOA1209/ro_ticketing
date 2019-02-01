using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sieve.Services;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using ticketing_api.Services;

namespace ticketing_api.Controllers.BaseController
{
    public class BaseController : ControllerBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IEmailSender _emailSender;
        protected readonly ApplicationSieveProcessor _sieveProcessor;

        public BaseController(ApplicationDbContext context, IEmailSender emailSender, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _emailSender = emailSender;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        private AppUser _appUser;
        protected AppUser AppUser
        {
            get
            {
                if (_appUser == null)
                {
                    var adminId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _appUser = _context.AppUser.FirstOrDefault(x => x.AspNetUserId == adminId);
                }

                return _appUser;
            }
        }
    }
}
