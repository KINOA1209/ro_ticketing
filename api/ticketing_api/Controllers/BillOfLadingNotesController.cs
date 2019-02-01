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
    public partial class BillOfLadingsController : BaseController.BaseController
    {
        public class NotesForm { public string notes; }

        /// <summary>
        /// Append to existing notes for purchase order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="notesForm">form with notes field</param>
        /// <returns></returns>
        [HttpPost("{id}/notes")]
        public async Task<IActionResult> PostNotes([FromRoute] int id, [FromBody]NotesForm notesForm)
        {
            if (notesForm == null) return BadRequest("Form body is missing");
            var notes = notesForm.notes;
            if(string.IsNullOrEmpty(notes)) return BadRequest("No note provided");

            var bol = await _context.BillOfLading.FindAsync(id);
            var errorMsg = await _billOfLadingService.CheckBillOfLadingAccessAsync(bol, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            bol.Notes += " " + notes;
            if (bol.Notes.Length > 1000) bol.Notes = bol.Notes.Substring(0, 1000);
            _context.Entry(bol).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //save ticket internal note change to the logs
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //OrderView orderView = _orderService.PostOrderView(ticket);
            //await _ticketLogService.StoreLogInformationPost(orderView, ticket, this.ControllerContext.RouteData.Values["action"].ToString(), userId);
            return Ok(notesForm);
        }

        /// <summary>
        /// Overwrite existing notes for purchase order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="notesForm">form with notes field</param>
        /// <returns></returns>
        [HttpPut("{id}/notes")]
        public async Task<IActionResult> PutInternalNotes([FromRoute] int id, [FromBody]NotesForm notesForm)
        {
            if (notesForm == null) return BadRequest("Form body is missing");
            var notes = notesForm.notes;
            if (string.IsNullOrEmpty(notes)) return BadRequest("No note provided");

            var bol = await _context.BillOfLading.FindAsync(id);
            var errorMsg = await _billOfLadingService.CheckBillOfLadingAccessAsync(bol, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            if (notes.Length > 1000) notes = notes.Substring(0, 1000);

            //Order ticketUpdateBefore = _context.Order.AsNoTracking().FirstOrDefault(o => o.Id == id);
            //OrderView ticketUpdateBeforeView = _orderService.PostOrderView(ticketUpdateBefore);
            bol.Notes = notes;
            _context.Entry(bol).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //var ticketUpdateAfter = _orderService.PostOrderView(ticket);

            //var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //await _ticketLogService.StoreLogInformationPut(ticketUpdateAfter, ticket, this.ControllerContext.RouteData.Values["action"].ToString(), userId, ticketUpdateBefore, ticketUpdateBeforeView);
            return Ok(notes);
        }
    }
}