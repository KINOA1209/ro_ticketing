using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLocationsController : BaseController.BaseController
    {
        private readonly ILogger<UserLocationsController> _logger;

        public UserLocationsController(ApplicationDbContext context, ILogger<UserLocationsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/UserLocation
        [HttpGet]
        public async Task<IActionResult> UserLocationAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.AppUserLocation.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return Ok(data);
        }

        // GET: api/UserLocation/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserLocation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userLocation = await _context.AppUserLocation.FindAsync(id);

            if (userLocation == null)
            {
                return NotFound("UserLocation Id not found");
            }

            return Ok(userLocation);
        }

        // POST: api/UserLocation
        [HttpPost]
        public async Task<IActionResult> PostUserLocation([FromBody] AppUserLocation userLocation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            userLocation.AppUserId = userId;
            _context.AppUserLocation.Add(userLocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserLocation", new { id = userLocation.Id }, userLocation);
        }

        // PUT: api/UserLocation/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserLocation([FromRoute] int id, [FromBody] AppUserLocation userLocation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userLocation.Id)
            {
                return BadRequest("Requested UserLocation Id does not match with QueryString Id");
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                userLocation.AppUserId = userId;
                _context.Entry(userLocation).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserLocationExists(id))
                {
                    return NotFound("UserLocation Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(userLocation);
        }

        // DELETE: api/UserLocation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserLocation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var UserLocation = await _context.AppUserLocation.FindAsync(id);
            if (UserLocation == null)
            {
                return NotFound("UserLocation Id not found");
            }

            _context.AppUserLocation.Remove(UserLocation);
            await _context.SaveChangesAsync();

            return Ok(UserLocation);
        }

        // Check UserLocationExists  or Not
        private bool UserLocationExists(int id)
        {
            return _context.AppUserLocation.Any(e => e.Id == id);
        }
    }
}
