using Microsoft.AspNetCore.Http;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using ticketing_api.Models.Views;
namespace ticketing_api.Services
{
    public class TicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISieveProcessor _sieveProcessor;

        public TicketService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<string> CheckTicketAccessAsync(Order ticket, AppUser user)
        {
            //if (ticket == null || ticket.OrderStatusId == AppConstants.OrderStatuses.Preticket)
            if (ticket == null)
            {
                return "Ticket Id not found";
            }

            var role = user.Role.ToUpper();
            if (role == "DRIVER")
            {
                var driver = _context.Driver.FirstOrDefault(x => x.AppUserId == user.Id);
                if (ticket.DriverId != driver?.Id)
                {
                    return "Driver not assigned to this ticket";
                }
            }
            else if (role == "SALES")
            {
                var sales = await _context.SalesRep.FirstAsync(x => x.AppUserId == user.Id);
                if (ticket.SalesRepId != sales.Id)
                {
                    return "Sales Rep not assigned to this ticket";
                }
            }

            return null;
        }

        public IEnumerable<TicketProductView> GetTicketProducts(int ticketId)
        {
            IEnumerable<TicketProductView> ticketProductView;

            ticketProductView = (from ticketproduct in _context.TicketProduct
                                 join product in _context.Product on ticketproduct.ProductId equals product.Id
                                 //join order in _context.Order on ticketproduct.TicketId equals order.Id
                                 join unit in _context.Unit on ticketproduct.UnitId equals unit.Id
                                 where ticketproduct.TicketId == ticketId
                                 select new TicketProductView { Id = ticketproduct.Id, TicketId = ticketId, ProductId = product, Quantity = ticketproduct.Quantity, UnitId = unit, Price = ticketproduct.Price }).AsEnumerable();

            return ticketProductView;
        }

        public TicketProductView PostTicketProduct(TicketProduct ticketProduct)
        {
            TicketProductView ticketProductView = (from ticketproduct in _context.TicketProduct
                                                   join product in _context.Product on ticketproduct.ProductId equals product.Id
                                                   join order in _context.Order on ticketproduct.TicketId equals order.Id
                                                   join unit in _context.Unit on ticketproduct.UnitId equals unit.Id
                                                   where ticketProduct.Id == ticketproduct.Id
                                                   select new TicketProductView { Id = ticketproduct.Id, TicketId = ticketproduct.TicketId, ProductId = product, Quantity = ticketproduct.Quantity, UnitId = unit, Price = ticketproduct.Price }).FirstOrDefault();

            return ticketProductView;
        }

        public string PostTicketScan(IFormFile file)
        {
            if (file != null)
            {
                var fileExt = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();

                if (!fileExt.Contains("png") && !fileExt.Contains("jpeg") && !fileExt.Contains("jpg"))
                {
                    return "Invalid file type";
                }

                string fileName = Guid.NewGuid() + fileExt;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "./Images/TicketImage/", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyToAsync(stream);
                }

                return fileName;
            }

            return "file are not selected";
        }
    }
}

