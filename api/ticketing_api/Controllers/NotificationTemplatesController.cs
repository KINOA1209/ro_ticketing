using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationTemplatesController : BaseController.BaseController
    {
        private readonly ILogger<NotificationTemplatesController> _logger;

        public NotificationTemplatesController(ApplicationDbContext context, ILogger<NotificationTemplatesController> logger, IEmailSender emailSender, ISieveProcessor sieveProcessor) : base(context, emailSender, sieveProcessor)
        {
            _logger = logger;
        }

        // GET: api/NotificationTemplates
        [HttpGet]
        public async Task<IActionResult> GetNotificationTemplateAsync([FromQuery] SieveModel sieveModel)
        {
            var query = _context.NotificationTemplate.AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return Ok(data);
        }

        // GET: api/NotificationTemplates/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationTemplate([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var notificationTemplate = await _context.NotificationTemplate.FindAsync(id);

            if (notificationTemplate == null)
            {
                return NotFound("NotificationTemplate Id not found");
            }

            return Ok(notificationTemplate);
        }

        // POST: api/NotificationTemplates
        [HttpPost]
        public async Task<IActionResult> PostNotificationTemplate([FromBody] NotificationTemplate notificationTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var notificationTemplateNameExists = await _context.NotificationTemplate.FirstOrDefaultAsync(m => m.Name.Equals(notificationTemplate.Name, StringComparison.OrdinalIgnoreCase));
            if (notificationTemplateNameExists != null)
            {
                return BadRequest("NotificationTemplate code already exists");
            }
            else
            {
                _context.NotificationTemplate.Add(notificationTemplate);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetNotificationTemplate", new { id = notificationTemplate.Id }, notificationTemplate);
        }

        // PUT: api/NotificationTemplates/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotificationTemplate([FromRoute] int id, [FromBody] NotificationTemplate notificationTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != notificationTemplate.Id)
            {
                return BadRequest("Requested NotificationTemplate Id does not match with QueryString Id");
            }

            try
            {
                var notificationTemplateName = _context.NotificationTemplate.Where(m => m.Id == notificationTemplate.Id).Select(m => new { m.Name });

                if (notificationTemplateName.FirstOrDefault()?.Name == notificationTemplate.Name)
                {
                    _context.Entry(notificationTemplate).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var notificationTemplateNameExists = _context.NotificationTemplate.Count(m => m.Id != notificationTemplate.Id && m.Name.Equals(notificationTemplate.Name, StringComparison.OrdinalIgnoreCase));
                    if (notificationTemplateNameExists > 0)
                    {
                        return BadRequest("NotificationTemplate Name already exists");
                    }

                    _context.Entry(notificationTemplate).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationTemplateExists(id))
                {
                    return NotFound("NotificationTemplate Id not found");
                }
                else
                {
                    throw;
                }
            }

            return Ok(notificationTemplate);
        }

        // DELETE: api/NotificationTemplates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotificationTemplate([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var notificationTemplate = await _context.NotificationTemplate.FindAsync(id);
            if (notificationTemplate == null)
            {
                return NotFound("NotificationTemplate Id not found");
            }

            ////check the NotificationTemplate Id exist in Order 
            //var notificationTemplateExistOrder = _context.Order.Count(o => o.NotificationTemplateId == id && !o.IsDeleted);

            //if (notificationTemplateExistOrder > 0)
            //{
            //    return BadRequest("Not able to delete NotificationTemplate as it has reference in Order table");
            //}

            // check the NotificationTemplate Id exist in Tax
            //var notificationTemplateExistTax = _context.Tax.Count(t => t.NotificationTemplateId == id && !t.IsDeleted);

            //if (notificationTemplateExistTax > 0)
            //{
            //    return BadRequest("Not able to delete NotificationTemplate as it has reference in Tax table");
            //}

            // check the NotificationTemplate Id exist in ShippingPaper
            //var notificationTemplateExistShippingPaper = _context.ShippingPaper.Count(sp => sp.NotificationTemplateId == id && !sp.IsDeleted);

            //if(notificationTemplateExistShippingPaper > 0)
            //{
            //    return BadRequest("Not able to delete NotificationTemplate as it has reference in ShippingPaper table");
            //}

            // check the NotificationTemplate Id exist in TicketPaper
            //var notificationTemplateExistTicketPaper = _context.TicketPaper.Count(tp => tp.NotificationTemplateId == id && !tp.IsDeleted);

            //if (notificationTemplateExistTicketPaper > 0)
            //{
            //    return BadRequest("Not able to delete NotificationTemplate as it has reference in TicketPaper table");
            //}

            _context.NotificationTemplate.Remove(notificationTemplate);
            await _context.SaveChangesAsync();

            return Ok(notificationTemplate);
        }

        // Check NotificationTemplate Exist or Not
        private bool NotificationTemplateExists(int id)
        {
            return _context.NotificationTemplate.Any(e => e.Id == id);
        }
    }
}