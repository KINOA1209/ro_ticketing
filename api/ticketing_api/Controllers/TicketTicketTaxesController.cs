using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using System.Linq;
using System.Threading.Tasks;
using Sieve.Extensions;
using ticketing_api.Models;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    partial class TicketsController : BaseController.BaseController
    {
        [HttpGet("{id}/tickettaxes")]
        public async Task<IActionResult> GetTicketTaxAsync([FromRoute] int id, [FromQuery] SieveModel sieveModel)
        {
            var ticketTaxService = new TicketTaxService(_context, _ticketService);
            var taxes = ticketTaxService.UpdateTaxForTicket(id);

            //var query = _context.TicketTax.Where(x=>x.TicketId == id && x.IsEnabled).ToList();
            //var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            var data = new PagingResults<TicketTax>
            {
                Page = sieveModel.Page ?? 0,
                Limit = sieveModel.PageSize,
                Total = taxes.Count,
                Order = sieveModel.Sorts,
                Filters = sieveModel.Filters,
                Items = taxes
            };

            return Ok(data);
        }

        //// POST: api/TicketTaxes/4
        //[HttpPost("{id}/tickettax")]
        //public async Task<IActionResult> PostTicketTax([FromRoute] int id, [FromBody] List<TicketTax> ticketTax)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var ticket = await _context.Order.FindAsync(ticketId);

        //    if (ticket == null || ticket.OrderStatusId == AppConstants.OrderStatuses.Preticket)
        //    {
        //        return NotFound("Ticket Id does not exists");
        //    }
          
        //    _context.TicketTax.AddRange(ticketTax);
        //    await _context.SaveChangesAsync(); 
        //    return Ok(ticketTax);
        //}

        //// DELETE: api/TicketTax/4
        //[HttpDelete("{id}/tickettaxes/{ticketTaxId}")]
        //public async Task<IActionResult> DeleteTicketTax([FromRoute] int id, [FromRoute] int ticketTaxId)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var ticketTax = await _context.TicketTax.FindAsync(id);
        //    if (ticketTax == null)
        //    {
        //        return NotFound("TicketTax Id not found");
        //    }

        //    _context.TicketTax.Remove(ticketTax);
        //    await _context.SaveChangesAsync();

        //    return Ok(ticketTax);
        //}
    }
}

