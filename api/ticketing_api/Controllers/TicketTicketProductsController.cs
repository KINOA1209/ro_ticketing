using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ticketing_api.Models;
using ticketing_api.Models.Views;

namespace ticketing_api.Controllers
{
    partial class TicketsController : BaseController.BaseController
    {
        /// <summary>
        /// Get list of products for ticket
        /// </summary>
        /// <param name="id">ticket Id</param> 
        /// <returns></returns>
        [HttpGet("{id}/ticketproducts")]
        public async Task<IActionResult> GetTicketProductsAsync(int id)
        {
            var ticket = await _context.Order.FindAsync(id);
            var errorMsg = await _ticketService.CheckTicketAccessAsync(ticket, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            var products = _ticketService.GetTicketProducts(id);

            var ticketTicketProductView = new TicketTicketProductView() { Ticket = ticket, TicketProducts = products };
            return Ok(ticketTicketProductView);
        }

        /// <summary>
        /// Add products to ticket
        /// </summary>
        /// <param name="id">ticketId</param>
        /// <param name="ticketProducts"></param>
        /// <returns></returns>
        [HttpPost("{id}/ticketproducts")]
        public async Task<IActionResult> PostTicketProductAsync([FromRoute] int id, [FromBody] List<TicketProduct> ticketProducts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticket = await _context.Order.FindAsync(id);
            var errorMsg = await _ticketService.CheckTicketAccessAsync(ticket, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            foreach (var ticketProduct in ticketProducts)
            {
                if (ticketProduct.TicketId != id)
                    return BadRequest("TicketProduct is not for this ticket id");
            }

            var resultViews = new List<TicketProductView>();
            foreach (var ticketProduct in ticketProducts)
            {
                _context.TicketProduct.Add(ticketProduct);
                await _context.SaveChangesAsync();
                var resultView = _ticketService.PostTicketProduct(ticketProduct);
                resultViews.Add(resultView);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _ticketLogService.StoreTicketProductLogInformation(id, resultView, this.ControllerContext.RouteData.Values["action"].ToString(), userId);
            }
            return Ok(resultViews);
        }

        /// <summary>
        /// Update an array of products for a ticket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ticketProducts"> list of the products to update</param>
        /// <returns></returns>
        //[Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}/ticketproducts")]
        public async Task<IActionResult> PutTicketProductsAsync([FromRoute] int id, [FromBody] List<TicketProduct> ticketProducts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var ticketProduct in ticketProducts)
            {
                if (ticketProduct.TicketId != id)
                    return BadRequest("TicketProduct is not for this ticket id");
            }

            var ticket = await _context.Order.FindAsync(id);
            var errorMsg = await _ticketService.CheckTicketAccessAsync(ticket, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            //To check the status of Voided and Delivered
            if (ticket.OrderStatusId == 4 || ticket.OrderStatusId == 5)
                return BadRequest("Cannot update TicketProduct when ticket status is either Delivered or Voided");
            List<TicketProductView> ticketProductUpdateAfterViewList = new List<TicketProductView>();
            foreach (var ticketProduct in ticketProducts)
            {
                var dbTicketProduct = _context.TicketProduct.AsNoTracking().FirstOrDefault(x => x.Id == ticketProduct.Id);
                if (dbTicketProduct?.TicketId != id)
                    return BadRequest("TicketProduct is not for this ticket id");
            }

            foreach (var ticketProduct in ticketProducts)
            {
                TicketProduct ticketProductUpdateBefore =
                    _context.TicketProduct.AsNoTracking().FirstOrDefault(x => x.Id == ticketProduct.Id);
                TicketProductView ticketProductUpdateBeforeView =
                    _ticketService.PostTicketProduct(ticketProductUpdateBefore);

                // check that only quantity changed if role is DRIVER

                var role = _context.AppUser.FirstOrDefault(x => x.Role == User.FindFirst(ClaimTypes.Role).Value)?.Role.ToUpper();

                if (role == "DRIVER")
                {
                    var tProduct = _context.TicketProduct.FirstOrDefault(x => x.Id == ticketProduct.Id);
                    tProduct.Quantity = ticketProduct.Quantity;
                    _context.Entry(tProduct).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Entry(ticketProduct).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }


                var ticketProductUpdateAfterView = _ticketService.PostTicketProduct(ticketProduct);
                ticketProductUpdateAfterViewList.Add(_ticketService.PostTicketProduct(ticketProduct));
                //save product update change to log
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _ticketLogService.StoreTicketProductLogInformationPut(ticketProductUpdateAfterView, ticketProduct,
                    this.ControllerContext.RouteData.Values["action"].ToString(), userId, ticketProductUpdateBeforeView,
                    ticketProductUpdateBefore);
            }

            return Ok(ticketProductUpdateAfterViewList);
        }

        /// <summary>
        /// Delete  ticket product from ticket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ticketProductId"></param>
        /// <returns></returns>
        [HttpDelete("{id}/ticketproducts/{ticketProductId}")]
        public async Task<IActionResult> DeleteTicketProductAsync([FromRoute] int id, [FromRoute] int ticketProductId)
        {
            var ticket = await _context.Order.FindAsync(id);
            var errorMsg = await _ticketService.CheckTicketAccessAsync(ticket, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            var dbTicketProduct = await _context.TicketProduct.FindAsync(ticketProductId);
            if (dbTicketProduct == null)
                return BadRequest("TicketProduct not found");
            if (dbTicketProduct.TicketId != id)
                return BadRequest("TicketProduct is not for this ticket id");

            _context.TicketProduct.Remove(dbTicketProduct);
            await _context.SaveChangesAsync();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _ticketLogService.StoreLogInformationDelete(id, this.ControllerContext.RouteData.Values["action"].ToString(), userId);

            return Ok(dbTicketProduct);
        }
    }
}