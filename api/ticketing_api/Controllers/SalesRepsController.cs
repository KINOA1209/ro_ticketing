using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;
using ticketing_api.Controllers.BaseController;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Infrastructure.Identity;
using ticketing_api.Models;
using ticketing_api.Models.Views;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesRepsController : IdentityBaseController
    {
        private readonly ILogger<SalesRepsController> _logger;
        private readonly SalesRepService _salesRepService;

        public SalesRepsController(ApplicationDbContext context, ApplicationUserManager userManager, ILogger<SalesRepsController> logger,
            IEmailSender emailSender, ISieveProcessor sieveProcessor)
            : base(context, userManager, emailSender, sieveProcessor)
        {
            _logger = logger;
            _salesRepService = new SalesRepService(context, sieveProcessor);
        }

        // GET: api/SalesReps
        [HttpGet]
        public async Task<IActionResult> GetSalesRepAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _salesRepService.GetSalesRepQuery();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return Ok(data);
        }

        // GET: api/SalesReps/5
        [HttpGet("{id}")]
        public IActionResult GetSalesRep([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var salesRep = _salesRepService.GetSalesRep(id);
            //var salesRep = await _context.SalesRep.FindAsync(id);

            if (salesRep == null)
            {
                return NotFound("SalesReps Id not found");
            }

            return Ok(salesRep);
        }

        // POST: api/SalesRep
        [HttpPost]
        public async Task<IActionResult> PostSalesRep([FromBody] SalesRepView salesRep)
        {
            salesRep.Role = "SALES";
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if (string.IsNullOrEmpty(salesRep.Password))
            //{
            //    return BadRequest("SalesRep password not provided");
            //}

            var fullName = salesRep.FullName.ToLower();
            var nameExists = await _context.AppUser.FirstOrDefaultAsync(d => d.Id != salesRep.Id && d.FullName.ToLower() == fullName); // && d.Role=="Driver");
            if (nameExists != null)
            {
                return BadRequest("SalesRep name already exists");
            }

            var userName = salesRep.Username.ToLower();
            var userNameExists = await _context.AppUser.FirstOrDefaultAsync(d => d.Id != salesRep.Id && d.Username.ToLower() == userName); // && d.Role=="Driver");
            if (userNameExists != null)
            {
                return BadRequest("Username already exists");
            }

            salesRep.Email = salesRep.Email ?? "";
            var appUser = new AppUser
            {
                FirstName = salesRep.FirstName,
                MiddleName = salesRep.MiddleName,
                LastName = salesRep.LastName,
                Email = salesRep.Email,
                Position = salesRep.Position,
                PhoneNumber = salesRep.PhoneNumber,
                EmployeeCode = salesRep.EmployeeCode,
                Username = salesRep.Username,
                Password = salesRep.Password,
                Role = "SALES",
                IsVisible = salesRep.IsVisible
            };

            var userService = new AppUserService(_context, UserManager);
            var result = await userService.CreateUserAsync(appUser, AdminUser);

            if (!string.IsNullOrEmpty(result.Message))
                return BadRequest(result.Message);

            if (result.IdentityResult != null && !result.IdentityResult.Succeeded)
            {
                AddErrors(result.IdentityResult);
                return BadRequest(ModelState);
            }

            //insert sales records
            SalesRep salesRepRecord = new SalesRep
            {
                AppUserId = appUser.Id,
                IsVisible = salesRep.IsVisible,
                Notes = salesRep.Notes
            };
            _context.SalesRep.Add(salesRepRecord);
            await _context.SaveChangesAsync();

            int myNewSalesRepId = _salesRepService.GetSalesIdFromAppUserId(appUser.Id);
            salesRep = _salesRepService.GetSalesRep(myNewSalesRepId);

            return CreatedAtAction("GetSalesRep", new { id = myNewSalesRepId }, salesRep);
        }

        // PUT: api/SalesReps/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesRep([FromRoute] int id, [FromBody] SalesRepView salesRep)
        {
            salesRep.Role = "SALES";
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salesRep.Id)
            {
                return BadRequest("Requested SalesRep Id does not match with QueryString Id");
            }

            salesRep.Email = salesRep.Email ?? "";
            try
            {
                var dbSales = _salesRepService.GetSalesRep(id);

                var appUser = new AppUser
                {
                    Id = dbSales.AppUserId,
                    AspNetUserId = salesRep.AspNetUserId,
                    FirstName = salesRep.FirstName,
                    MiddleName = salesRep.MiddleName,
                    LastName = salesRep.LastName,
                    Email = salesRep.Email,
                    PhoneNumber = salesRep.PhoneNumber,
                    Position = salesRep.Position,
                    EmployeeCode = salesRep.EmployeeCode,
                    Username = salesRep.Username,
                    Role = "SALES",
                    IsVisible = salesRep.IsVisible
                };

                var netUser = await UserManager.FindByIdAsync(appUser.AspNetUserId);
                if (!string.Equals(netUser.UserName, appUser.Username, StringComparison.OrdinalIgnoreCase))
                {
                    netUser.Email = appUser.Email;
                    netUser.UserName = appUser.Username;
                    netUser.NormalizedEmail = appUser.Email.ToUpper();
                    netUser.NormalizedUserName = appUser.Username.ToUpper();
                    await UserManager.UpdateAsync(netUser);
                }


                // Update Sales Rep password only if this field is set
                if (!string.IsNullOrEmpty(salesRep.Password))
                {
                    try
                    {
                        var result = await UserManager.UpdatePasswordAsync(netUser, salesRep.Password);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                if (dbSales.Id > 0)
                {
                    _context.Entry(appUser).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var salesNameExists = await _context.AppUser.SingleOrDefaultAsync(d => d.FullName.ToLower() == dbSales.FullName.ToLower());
                    if (salesNameExists != null)
                    {
                        return BadRequest("Sales name already exists");
                    }

                    //update appuser
                    _context.Entry(appUser).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                // update SalesRep
                var salesUpdate = new SalesRep
                {
                    Id = dbSales.Id,
                    AppUserId = dbSales.AppUserId,
                    IsVisible = salesRep.IsVisible,
                    Notes = salesRep.Notes
                };
                _context.Entry(salesUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                salesRep = _salesRepService.GetSalesRep(id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesRepExists(id))
                {
                    return NotFound("SalesReps Id not found");
                }

                throw;
            }
            return Ok(salesRep);
        }

        // DELETE: api/SalesReps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesRep([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var salesRep = await _context.SalesRep.FindAsync(id);
            if (salesRep == null)
            {
                return NotFound("SalesReps Id not found");
            }

            // check the SalesRep Id exist in order
            var salesRepExistOrder = _context.Order.Count(o => o.SalesRepId == id && !o.IsDeleted);

            if (salesRepExistOrder > 0)
            {
                return BadRequest("Not able to delete SalesRep as it has reference in Order table");
            }

            //to delete appuser
            var dbSales = _salesRepService.GetSalesRep(id);
            var appUser = new AppUser
            {
                Id = dbSales.AppUserId,
                AspNetUserId = dbSales.AspNetUserId,
                FirstName = dbSales.FirstName,
                MiddleName = dbSales.MiddleName,
                LastName = dbSales.LastName,
                Email = dbSales.Email,
                PhoneNumber = dbSales.PhoneNumber,
                Position = dbSales.Position,
                EmployeeCode = dbSales.EmployeeCode,
                Username = dbSales.Username,
                Password = dbSales.Password,
                Role = "SALES",
                IsVisible = salesRep.IsVisible,
                SetPasswordToken = dbSales.SetPasswordToken,
                IsDeleted = true
            };

            _context.Entry(appUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //to delete salesRep
            _context.SalesRep.Remove(salesRep);
            await _context.SaveChangesAsync();

            return Ok(salesRep);
        }

        // Check SaleRep Exist or Not
        private bool SalesRepExists(int id)
        {
            return _context.SalesRep.Any(e => e.Id == id);
        }
    }
}
