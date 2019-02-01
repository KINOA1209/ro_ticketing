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
using ticketing_api.Models.Views;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RigLocationsController : BaseController.BaseController
    {
        private readonly ILogger<RigLocationsController> _logger;
        private readonly RigLocationService _rigLocationService;
        private readonly LogService _rigLocationLogService;

        public RigLocationsController(ApplicationDbContext context, ILogger<RigLocationsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base (context, emailSender, sieveProcessor)
        {
            _logger = logger;
            _rigLocationService = new RigLocationService(_context, sieveProcessor);
            _rigLocationLogService = new LogService(_context, sieveProcessor);
        }

        // GET: api/RigLocations
        [HttpGet]
        public async Task<IActionResult> GetRigLocationAsync([FromQuery] SieveModel sieveModel)
        {
            PagingResults<RigLocationView> rigLocation = await _rigLocationService.GetRigLocationViewAsync(sieveModel);

            return Ok(rigLocation);
        }

        // GET: api/RigLocations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRigLocation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rigLocation = await _context.RigLocation.FindAsync(id);

            if (rigLocation == null)
            {
                return NotFound("RigLocation Id not found");
            }

            return Ok(rigLocation);
        }

        // POST: api/RigLocations
        [HttpPost]
        public async Task<IActionResult> PostRigLocation([FromBody] RigLocation rigLocation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var rigLocationNameExists = await _context.RigLocation.FirstOrDefaultAsync(r => r.Name.Equals(rigLocation.Name, StringComparison.OrdinalIgnoreCase));
            if (rigLocationNameExists != null)
            {
                return BadRequest("Riglocation name already exists");
            }
            else
            {
                _context.RigLocation.Add(rigLocation);
                 await _context.SaveChangesAsync();
            }

            RigLocationView rigLocationView = _rigLocationService.PostRigLocation(rigLocation);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _rigLocationLogService.StoreRigLocationLogInformationPost(rigLocationView, rigLocation, this.ControllerContext.RouteData.Values["action"].ToString(), userId);

            return CreatedAtAction("GetRigLocation", new { id = rigLocation.Id }, rigLocationView);
        }

        // PUT: api/RigLocations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRigLocation([FromRoute] int id, [FromBody] RigLocation rigLocation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rigLocation.Id)
            {
                return BadRequest("Requested RigLocation Id does not match with QueryString Id");
            }

            //Get the rigLocation data before update
            RigLocation rigLocationUpdateBefore = _context.RigLocation.AsNoTracking().FirstOrDefault(r => r.Id == id);
            RigLocationView rigLocationUpdateBeforeView = _rigLocationService.PostRigLocation(rigLocationUpdateBefore);
            try
            {
                var rigLocationName = _context.RigLocation.Where(r => r.Id == rigLocation.Id).Select(r => r.Name).Single();
                
                if (rigLocationName == rigLocation.Name)
                {
                    _context.Entry(rigLocation).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var rigLocationNameExists = _context.RigLocation.Count(r => r.Id != rigLocation.Id && r.Name.Equals(rigLocation.Name, StringComparison.OrdinalIgnoreCase));
                    if (rigLocationNameExists > 0)
                    {
                        return BadRequest("Riglocation name already exists");
                    }
                    else
                    {
                        _context.Entry(rigLocation).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RigLocationExists(id))
                {
                    return NotFound("RigLocation Id not found");
                }
                else
                {
                    throw;
                }
            }

            RigLocationView rigLocationView = _rigLocationService.PostRigLocation(rigLocation);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _rigLocationLogService.StoreRigLocationLogInformationPut(rigLocation, rigLocationView, this.ControllerContext.RouteData.Values["action"].ToString(), userId, rigLocationUpdateBefore, rigLocationUpdateBeforeView);

            return Ok(rigLocationView);
        }

        // DELETE: api/RigLocations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRigLocation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rigLocation = await _context.RigLocation.FindAsync(id);
            if (rigLocation == null)
            {
                return NotFound("RigLocation Id not found");
            }

            // check the RigLocation Id exist in Order
            var rigLocationExistOrder = _context.Order.Count(o => o.RigLocationId == id && !o.IsDeleted);

            if (rigLocationExistOrder > 0)
            {
                return BadRequest("Not able to delete RigLocation as it has reference in Order table");
            }

            // check the RigLocation Id exist in Well
            var rigLocationExistWell = _context.Well.Count(w => w.RigLocationId == id && !w.IsDeleted);


            if(rigLocationExistWell > 0)
            {
                return BadRequest("Not able to delete RigLocation as it has reference in Well table");
            }

            _context.RigLocation.Remove(rigLocation);
            await _context.SaveChangesAsync();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _rigLocationLogService.StoreLogInformationDelete(id, this.ControllerContext.RouteData.Values["action"].ToString(), userId);

            return Ok(rigLocation);
        }

        // Check Riglocation Exist or Not
        private bool RigLocationExists(int id)
        {
            return _context.RigLocation.Any(e => e.Id == id);
        }

        //GET: api/Riglocation/Filter?customerId=1
        [HttpGet("FilterRigByCustomer")]
        public IEnumerable<RigLocation> Filter([FromQuery] int customerId)
        { 
            var resultRig = _context.RigLocation.Where(x => x.CustomerId == customerId).AsQueryable().OrderBy(rig => rig.Name);
            return resultRig;
        }
    }
}