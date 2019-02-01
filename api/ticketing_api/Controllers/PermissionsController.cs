using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : BaseController.BaseController
    {

        private readonly ILogger<ModulesController> _logger;

        public PermissionsController(ApplicationDbContext context, ILogger<ModulesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/Permission/{roleId}
        [HttpGet("{RoleId}")]
        public async Task<IActionResult> GetPermission([FromRoute]string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                return BadRequest("Role id not supplied");

            var permissions = await _context.Permission.Join(_context.Module, a => a.ModuleId, b => b.Id, (a, b) => new
            {
                a.Id,
                a.RoleId,
                a.ModuleId,
                b.ModuleName,
                a.IsRead,
                a.IsCreate,
                a.IsUpdate,
                a.IsDelete
            }).Where(x => x.RoleId == roleId).ToListAsync();

            if (permissions == null)
            {
                return NotFound();
            }

            string _data = JsonConvert.SerializeObject(permissions);
            return Ok(_data);
        }

        // POST : Insert Permission
        [HttpPost("{RoleId}")]
        public async Task<IActionResult> PostPermission([FromRoute] string roleId, [FromBody] List<Permission> _list)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var item in _list)
            {
                //Check Module and roleid in permission table
                if (PermissionExists(item))
                {
                    //Update permission here
                    _context.Entry(item).State = EntityState.Modified;
                }
                else
                {
                    //Add new permission here
                    _context.Permission.Add(item);
                }
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPermission", new { id = roleId }, _list);
        }

        private bool PermissionExists(Permission item)
        {
            return _context.Permission.Any(e => e.RoleId == item.RoleId && e.ModuleId == item.ModuleId);
        }
    }
}

