using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    public class WellsController : BaseController.BaseController
    {
        private readonly ILogger<WellsController> _logger;
        private readonly WellService _wellService;
        private readonly CheckPermissionService _checkPermissionService;

        public WellsController(ApplicationDbContext context, ILogger<WellsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
            _wellService = new WellService(_context, sieveProcessor);
            _checkPermissionService = new CheckPermissionService(context,sieveProcessor);

        }

        // GET: api/Wells
        [HttpGet]
        public async Task<IActionResult> GetWell ([FromQuery] SieveModel sieveModel)
        {
            //var roleName = User.FindFirst(ClaimTypes.Role).Value;
            //var isPermission = await _checkPermissionService.CheckPermissions("WELLS", "Read", roleName);
            //if (!isPermission) return BadRequest("Read Permission not allowed");

            PagingResults<WellView> wellView = await _wellService.GetWellViewAsync(sieveModel);
            return Ok(wellView);
        }

        // GET: api/Wells/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWell([FromRoute] int id)
        {
            //var roleName = User.FindFirst(ClaimTypes.Role).Value;
            //var isPermission = await _checkPermissionService.CheckPermissions("WELLS", "Read", roleName);
            //if (!isPermission) return BadRequest("Read permission not allowed");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var well = await _context.Well.FindAsync(id);

            if (well == null)
            {
                return NotFound("Well Id not found");
            }

            return Ok(well);
        }

        //GET: api/Wells/Filter?rigLocationId = 1
        [HttpGet("FilterWellByRigLocation")]
        public IEnumerable<Well> Filter([FromQuery] int rigLocationId)
        {
            var resultWell = _context.Well.Where(x => x.RigLocationId == rigLocationId).AsQueryable().OrderBy(well => well.Name);
            return resultWell;
        }

        // POST: api/Wells
        [HttpPost]
        public async Task<IActionResult> PostWell([FromBody] Well well)
        {
            //var roleName = User.FindFirst(ClaimTypes.Role).Value;
            //var isPermission = await _checkPermissionService.CheckPermissions("WELLS", "Create", roleName);
            //if (!isPermission) return BadRequest("Create permission not allowed");

            if (!ModelState.IsValid)
            {
                return NewMethod();
            }
            var wellNameExists = await _context.Well.FirstOrDefaultAsync(w => w.Name.Equals(well.Name, StringComparison.OrdinalIgnoreCase));
            if (wellNameExists != null)
            {
                return BadRequest("Well name already exists");
            }
            else
            {
                _context.Well.Add(well);
                 await _context.SaveChangesAsync();
            }

            WellView wellView = _wellService.PostWell(well);

            return CreatedAtAction("GetWell", new { id = well.Id }, wellView);
        }

        // PUT: api/Wells/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWell([FromRoute] int id, [FromBody] Well well)
        {
            //var roleName = User.FindFirst(ClaimTypes.Role).Value;
            //var isPermission = await _checkPermissionService.CheckPermissions("WELLS", "Update", roleName);
            //if (!isPermission) return BadRequest("Update permission not allowed");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != well.Id)
            {
                return BadRequest("Requested Well Id does not match with QueryString Id");
            }

            try
            {
                var wellName = _context.Well.Where(w => w.Id == well.Id).Select(w => w.Name).Single();

                if (wellName == well.Name)
                {
                    _context.Entry(well).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var wellNameExists = _context.Well.Count(w => w.Id != well.Id && w.Name.Equals(well.Name, StringComparison.OrdinalIgnoreCase));
                    if (wellNameExists > 0)
                    {
                        return BadRequest("Well name already exists");
                    }
                    else
                    {
                        _context.Entry(well).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WellExists(id))
                {
                    return NotFound("Well Id not found");
                }
                throw;
            }

            WellView wellView = _wellService.PostWell(well);

            return Ok(wellView);
        }

        private IActionResult NewMethod()
        {
            return BadRequest(ModelState);
        }

        // DELETE: api/Wells/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWell([FromRoute] int id)
        {
            //var roleName = User.FindFirst(ClaimTypes.Role).Value;
            //var isPermission = await _checkPermissionService.CheckPermissions("WELLS", "Delete", roleName);
            //if (!isPermission) return BadRequest("Delete permission not allowed");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var well = await _context.Well.FindAsync(id);
            if (well == null)
            {
                return NotFound("Well Id not found");
            }

            // check the Well Id exist in order
            var wellExistOrder = _context.Order.Count(o => o.WellId == id && !o.IsDeleted);

            if (wellExistOrder > 0)
            {
                return BadRequest("Not able to delete Well as it has reference in Order table");
            }

            _context.Well.Remove(well);
            await _context.SaveChangesAsync();

            return Ok(well);
        }

        // Check well Exist or Not
        private bool WellExists(int id)
        {
            return _context.Well.Any(e => e.Id == id);
        }
    }
}