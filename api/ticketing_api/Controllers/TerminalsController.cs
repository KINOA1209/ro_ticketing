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
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerminalsController : BaseController.BaseController
    {
        private readonly ILogger<TerminalsController> _logger;

        public TerminalsController(ApplicationDbContext context, ILogger<TerminalsController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) 
            : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/Terminals
        [HttpGet]
        public async Task<IActionResult> GetTerminalAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.Terminal.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/Terminals/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTerminal([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var terminal = await _context.Terminal.FindAsync(id);

            if (terminal == null)
            {
                return NotFound("Terminal Id not found");
            }

            return Ok(terminal);
        }

        // POST: api/Terminals
        [HttpPost]
        public async Task<IActionResult> PostTerminal([FromBody] Terminal terminal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var terminalNameExists = await _context.Terminal.FirstOrDefaultAsync(t => t.Name.Equals(terminal.Name, StringComparison.OrdinalIgnoreCase));
            if (terminalNameExists != null)
            {
                return BadRequest("Terminal name already exists");
            }
            else
            {
                _context.Terminal.Add(terminal);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetTerminal", new { id = terminal.Id }, terminal);
        }

        // PUT: api/Terminals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTerminal([FromRoute] int id, [FromBody] Terminal terminal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != terminal.Id)
            {
                return BadRequest("Requested Terminal Id does not match with QueryString Id");
            }

            try
            {
                var terminalName = _context.Terminal.Where(t => t.Id == terminal.Id).Select(t => t.Name).Single();

                if (terminalName == terminal.Name)
                {
                    _context.Entry(terminal).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var terminalNameExists = _context.Terminal.Count(t => t.Name.Equals(terminal.Name, StringComparison.OrdinalIgnoreCase) && t.Id != terminal.Id);
                    if (terminalNameExists > 0)
                    {
                        return BadRequest("Terminal name already exists");
                    }
                    else
                    {
                        _context.Entry(terminal).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TerminalExists(id))
                {
                    return NotFound("Terminal Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(terminal);
        }

        // DELETE: api/Terminals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTerminal([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var terminal = await _context.Terminal.FindAsync(id);
            if (terminal == null)
            {
                return NotFound("Terminal Id not found");
            }

            //var terminalExistOrder = _context.BillOfLading.Count(o => o == id && !o.IsDeleted);
            //if (terminalExistOrder > 0)
            //{
            //    return BadRequest("Not able to delete Terminal as it has reference in Order table");
            //}

            _context.Terminal.Remove(terminal);
            await _context.SaveChangesAsync();

            return Ok(terminal);
        }

        // Check Terminals Exist or Not
        private bool TerminalExists(int id)
        {
            return _context.Terminal.Any(e => e.Id == id);
        }
    }
}