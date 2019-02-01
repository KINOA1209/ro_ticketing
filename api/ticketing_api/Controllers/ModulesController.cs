using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ModulesController : BaseController.BaseController
    {
        private readonly ILogger<ModulesController> _logger;

        public ModulesController(ApplicationDbContext context, ILogger<ModulesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/Modules
        [HttpGet]
        public async Task<IActionResult> GetModules([FromQuery]SieveModel sieveModel)
        {
            var query = _context.Module.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
            
        }

        // GET: api/Modules/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetModule([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var modules = await _context.Module.FindAsync(id);

            if (modules == null)
            {
                return NotFound("Module Id not found");
            }

            return Ok(modules);
        }

        // POST : api/Modules
        [HttpPost]
        public async Task<IActionResult> PostModule([FromBody] Module module)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //set normalizedName in module
            module.NormalizedName = module.ModuleName.ToUpper();

            var moduleNameExists = await _context.Module.FirstOrDefaultAsync(m => m.ModuleName.Equals(module.ModuleName, StringComparison.OrdinalIgnoreCase));
            if (moduleNameExists != null)
            {
                return BadRequest("Module name already exists");
            }
            else
            { 
                 _context.Module.Add(module);
                  await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetModule", new { id = module.Id }, module);
        }

        // PUT: api/Modules/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModule([FromRoute] int id, [FromBody] Module module)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != module.Id)
            {
                return BadRequest("Requested Module Id does not match with QueryString Id");
            }

            try
            {
                var moduleName = _context.Module.Where(mo => mo.Id == module.Id).Select(mo => mo.ModuleName).Single();

                //set normalizedName in module
                module.NormalizedName = module.ModuleName.ToUpper();
                if (moduleName == module.ModuleName)
                {
                    _context.Entry(module).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var moduleNameExists = _context.Module.Count(m => m.Id != module.Id && m.ModuleName.Equals(module.ModuleName, StringComparison.OrdinalIgnoreCase));
                    if (moduleNameExists > 0)
                    {
                        return BadRequest("Module name already exists");
                    }
                    else
                    {
                        _context.Entry(module).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleExists(id))
                {
                    return NotFound("Module Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(module);
        }

        // DELETE: api/Modules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var module = await _context.Module.FindAsync(id);
            if (module == null)
            {
                return NotFound("Module Id not found");
            }

            // check the Module Id exist in Permission
            var moduleExistPermission = _context.Permission.Count(p => p.ModuleId == id);

            if (moduleExistPermission > 0)
            {
                return BadRequest("Not able to delete Module as it has reference in Permission table");
            }

            _context.Module.Remove(module);
            await _context.SaveChangesAsync();

            return Ok(module);
        }

        // Check module Exist or Not
        private bool ModuleExists(int id)
        {
            return _context.Module.Any(e => e.Id == id);
        }

    }
}
