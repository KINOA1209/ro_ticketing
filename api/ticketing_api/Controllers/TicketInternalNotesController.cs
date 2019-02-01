using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ticketing_api.Infrastructure;
using System.Linq;
using System.Security.Claims;
using ticketing_api.Models.Views;
using ticketing_api.Models;

namespace ticketing_api.Controllers
{
    public partial class TicketsController : BaseController.BaseController
    {
        public class InternalNotesForm { public string internalNotes; }

        /// <summary>
        /// Append to existing InternalNotes for ticket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="internalNotesForm"></param>
        /// <returns></returns>
        [HttpPost("{id}/internalnotes")]
        public async Task<IActionResult> PostInternalNotes([FromRoute] int id, [FromBody]InternalNotesForm internalNotesForm)
        {
            if (internalNotesForm == null) return BadRequest("Form body is missing");
            var internalNotes = internalNotesForm.internalNotes;
            if(string.IsNullOrEmpty(internalNotes)) return BadRequest("No note provided");

            var ticket = await _context.Order.FindAsync(id);
            var errorMsg = await _ticketService.CheckTicketAccessAsync(ticket, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            ticket.InternalNotes += " " + internalNotes;
            if (ticket.InternalNotes.Length > 1000) ticket.InternalNotes = ticket.InternalNotes.Substring(0, 1000);
            _context.Entry(ticket).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //save ticket internal note change to the logs
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            OrderView orderView = _orderService.PostOrderView(ticket);
            await _ticketLogService.StoreLogInformationPost(orderView, ticket, this.ControllerContext.RouteData.Values["action"].ToString(), userId);

            internalNotesForm.internalNotes = ticket.InternalNotes;
            return Ok(internalNotesForm);
        }

        /// <summary>
        /// Overwrite existing InternalNotes for ticket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="internalNotesForm"></param>
        /// <returns></returns>
        [HttpPut("{id}/internalnotes")]
        public async Task<IActionResult> PutInternalNotes([FromRoute] int id, [FromBody]InternalNotesForm internalNotesForm)
        {
            if (internalNotesForm == null) return BadRequest("Form body is missing");
            var internalNotes = internalNotesForm.internalNotes;
            if (string.IsNullOrEmpty(internalNotes)) return BadRequest("No note provided");

            var ticket = await _context.Order.FindAsync(id);
            var errorMsg = await _ticketService.CheckTicketAccessAsync(ticket, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            if (internalNotes.Length > 1000) internalNotes = internalNotes.Substring(0, 1000);

            Order ticketUpdateBefore = _context.Order.AsNoTracking().FirstOrDefault(o => o.Id == id);
            OrderView ticketUpdateBeforeView = _orderService.PostOrderView(ticketUpdateBefore);
            ticket.InternalNotes = internalNotes;
            _context.Entry(ticket).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var ticketUpdateAfter = _orderService.PostOrderView(ticket);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _ticketLogService.StoreLogInformationPut(ticketUpdateAfter, ticket, this.ControllerContext.RouteData.Values["action"].ToString(), userId, ticketUpdateBefore, ticketUpdateBeforeView);
            return Ok(internalNotes);
        }
    }
}