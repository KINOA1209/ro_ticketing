using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class DriversController : IdentityBaseController
    {
        private readonly ILogger<DriversController> _logger;
        private readonly DriverService _driverService;

        public DriversController(ApplicationDbContext context, ApplicationUserManager userManager, ILogger<DriversController> logger,
            IEmailSender emailSender, ISieveProcessor sieveProcessor)
            : base(context, userManager, emailSender, sieveProcessor)
        {
            _logger = logger;
            _driverService = new DriverService(context, sieveProcessor);
        }

        // GET: api/Drivers
        //[Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetDriverAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _driverService.GetDriverQuery();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/Drivers/5
        [HttpGet("{id}")]
        public IActionResult GetDriver([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var driver = _driverService.GetDriver(id);

            if (driver == null)
            {
                return NotFound("Driver Id not found");
            }

            return Ok(driver);
        }

        // POST: api/Drivers
        //[Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> PostDriver([FromBody] DriverView driver)
        {
            driver.Role = "DRIVER";
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(string.IsNullOrEmpty(driver.Password))
            {
                return BadRequest("Driver password not provided");
            }

            var fullName = driver.FullName.ToLower();
            var driverNameExists = await _context.AppUser.FirstOrDefaultAsync(d => d.FullName.ToLower() == fullName); // && d.Role=="Driver");
            if (driverNameExists != null)
            {
                return BadRequest("Driver name already exists");
            }

            var username = driver.Username.ToLower();
            var userNameExists = await _context.AppUser.FirstOrDefaultAsync(d => d.Username.ToLower() == username); // && d.Role=="Driver");
            if (userNameExists != null)
            {
                return BadRequest("Username already exists");
            }

            driver.Email = driver.Email ?? "";

            var appUser = new AppUser
            {
                FirstName = driver.FirstName,
                MiddleName = driver.MiddleName,
                LastName = driver.LastName,
                Email = driver.Email,
                PhoneNumber = driver.PhoneNumber,
                Position = driver.Position,
                EmployeeCode = driver.EmployeeCode,
                Username = driver.Username,
                Password = driver.Password,
                Role = driver.Role,
                IsVisible = driver.IsVisible
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

            //insert driver records
            Driver driverRecord = new Driver() { };
            driverRecord.AppUserId = appUser.Id;
            driverRecord.IsVisible = appUser.IsVisible;
            _context.Driver.Add(driverRecord);
            await _context.SaveChangesAsync();

            //get Driver Id from AppUserId
            int myNewDriverId = _driverService.GetDriverIdFromAppUserId(appUser.Id);
            driver = _driverService.GetDriver(myNewDriverId);
            return CreatedAtAction("GetDriver", new { id = myNewDriverId }, driver);
        }

        // PUT: api/Drivers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDriver([FromRoute] int id, [FromBody] DriverView driver)
        {
            driver.Role = "DRIVER";
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != driver.Id)
            {
                return BadRequest("Requested Driver Id does not match with query id");
            }

            driver.Email = driver.Email ?? "";
        
            try
            {
                var dbDriver = _driverService.GetDriver(id);

                var appUser = new AppUser
                {
                    Id = dbDriver.AppUserId,
                    AspNetUserId = dbDriver.AspNetUserId,
                    FirstName = driver.FirstName,
                    MiddleName = driver.MiddleName,
                    LastName = driver.LastName,
                    Email = driver.Email,
                    PhoneNumber = driver.PhoneNumber,
                    Position = driver.Position,
                    EmployeeCode = driver.EmployeeCode,
                    Username = driver.Username,
                    Role = "DRIVER",
                    IsVisible = driver.IsVisible
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


                // Update Driver password only if this field is set
                if (!string.IsNullOrEmpty(driver.Password))
                {
                    try
                    {
                        var result = await UserManager.UpdatePasswordAsync(netUser, driver.Password);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                if (driver.Id > 0)
                {
                    _context.Entry(appUser).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var driverNameExists = await _context.AppUser.SingleOrDefaultAsync(d => d.Id != dbDriver.Id && d.FullName.Equals(dbDriver.FullName,StringComparison.OrdinalIgnoreCase));
                    if (driverNameExists != null)
                    {
                        return BadRequest("Driver name already exists");
                    }
                }
                //update driver
                if (dbDriver.Id > 0)
                {
                    Driver driverUpdate = new Driver
                    {
                        Id = dbDriver.Id, AppUserId = dbDriver.AppUserId, IsVisible = driver.IsVisible
                    };
                    _context.Entry(driverUpdate).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

                driver = _driverService.GetDriver(id);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DriverExists(id))
                {
                    return NotFound("Driver id not found");
                }

                throw;
            }

            return Ok(driver);
        }

        // DELETE: api/Drivers/5
        //[Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver([FromRoute] int id)
        { 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var driver = await _context.Driver.FindAsync(id);
            if (driver == null)
            {
                return NotFound("Driver Id not found");
            }

            // check the Driver Id exist in order
            var driverExistOrder = _context.Order.Count(o => o.DriverId == id && !o.IsDeleted);
            if (driverExistOrder > 0)
            {
                return BadRequest("Not able to delete Driver as it has reference in Order table");
            }

            //to delete appuser
            var dbDriver = _driverService.GetDriver(id);
            var appUser = new AppUser
            {
                Id = dbDriver.AppUserId,
                AspNetUserId = dbDriver.AspNetUserId,
                FirstName = dbDriver.FirstName,
                MiddleName = dbDriver.MiddleName,
                LastName = dbDriver.LastName,
                Email = dbDriver.Email,
                PhoneNumber = dbDriver.PhoneNumber,
                Position = dbDriver.Position,
                EmployeeCode = dbDriver.EmployeeCode,
                Username = dbDriver.Username,
                Role = "DRIVER",
                IsVisible = dbDriver.IsVisible,
                IsDeleted = true
            };

            _context.Entry(appUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //to delete driver
            _context.Driver.Remove(driver);
            await _context.SaveChangesAsync();

            return Ok(driver);
        }

        // Check Drivers Exist or Not
        private bool DriverExists(int id)
        {
            return _context.Driver.Any(e => e.Id == id);
        }
    }
}