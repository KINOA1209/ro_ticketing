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
    public class JobTypesController : BaseController.BaseController
    {
        private readonly ILogger<JobTypesController> _logger;

        public JobTypesController(ApplicationDbContext context, ILogger<JobTypesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/JobTypes
        [HttpGet]
        public async Task<IActionResult> GetJobTypeAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.JobType.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return Ok(data);
        }

        // GET: api/JobTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jobType = await _context.JobType.FindAsync(id);

            if (jobType == null)
            {
                return NotFound("JobType Id not found");
            }

            return Ok(jobType);
        }

        // POST: api/JobTypes
        [HttpPost]
        public async Task<IActionResult> PostJobTypeAsync([FromBody] JobType jobType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jobTypeNameExists = await _context.JobType.FirstOrDefaultAsync(j => j.Name.Equals(jobType.Name, StringComparison.OrdinalIgnoreCase));
            if (jobTypeNameExists != null)
                return BadRequest("Job Type already exists");

            _context.JobType.Add(jobType);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetJobType", new { id = jobType.Id }, jobType);
        }

        // PUT: api/JobTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobTypeAsync([FromRoute] int id, [FromBody] JobType jobType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != jobType.Id)
                return BadRequest("Requested JobType Id does not match with QueryString Id");

            try
            {
                var jobTypeName = _context.JobType.Where(j => j.Id == jobType.Id).Select(j => j.Name).Single();

                if (jobTypeName == jobType.Name)
                {
                    _context.Entry(jobType).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var jobTypeNameExists = _context.JobType.Count(j => j.Id != jobType.Id && j.Name.Equals(jobType.Name, StringComparison.OrdinalIgnoreCase));
                    if (jobTypeNameExists > 0)
                    {
                        return BadRequest("Job Type already exists");
                    }

                    _context.Entry(jobType).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobTypeExists(id))
                {
                    return NotFound("JobType Id not found");
                }

                throw;
            }

            return Ok(jobType);
        }

        // DELETE: api/JobTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobTypeAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jobType = await _context.JobType.FindAsync(id);
            if (jobType == null)
            {
                return NotFound("JobType Id not found");
            }

            //check the JobType Id exist in order 
            var jobTypeExistOrder = _context.Order.Count(o => o.JobTypeId == id && !o.IsDeleted);

            if (jobTypeExistOrder > 0)
            {
                return BadRequest("Not able to delete JobType as it has reference in Order table");
            }
            _context.JobType.Remove(jobType);
            await _context.SaveChangesAsync();

            return Ok(jobType);
        }

        // Check JobType Exist or Not
        private bool JobTypeExists(int id)
        {
            return _context.JobType.Any(e => e.Id == id);
        }
    }
}