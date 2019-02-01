using Microsoft.AspNetCore.Http;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using ticketing_api.Models.Views;
namespace ticketing_api.Services
{
    public class BillOfLadingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationSieveProcessor _sieveProcessor;

        public BillOfLadingService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        public async Task<string> CheckBillOfLadingAccessAsync(BillOfLading bol, AppUser user)
        {
            //if (ticket == null || ticket.OrderStatusId == AppConstants.OrderStatuses.Preticket)
            if (bol == null)
            {
                return "BOL Id not found";
            }

            //var role = user.Role.ToUpper();
            //if (role == "DRIVER")
            //{
            //    var driver = _context.Driver.FirstOrDefault(x => x.AppUserId == user.Id);
            //    if (bol.DriverId != driver?.Id)
            //    {
            //        return "Driver not assigned to this ticket";
            //    }
            //}
            //else if (role == "SALES")
            //{
            //    var sales = await _context.SalesRep.FirstAsync(x => x.AppUserId == user.Id);
            //    if (bol.SalesRepId != sales.Id)
            //    {
            //        return "Sales Rep not assigned to this ticket";
            //    }
            //}

            return null;
        }

        private IQueryable<BillOfLadingView> BillOfLadingViewQuery =>

        (from bol in _context.BillOfLading
         join Market in _context.Market on bol.MarketId equals Market.Id into m
         from Market in m.DefaultIfEmpty()
         join Driver in _context.Driver on bol.DriverId equals Driver.Id into d
         from Driver in d.DefaultIfEmpty()
         join driverUser in _context.AppUser on Driver.AppUserId equals driverUser.Id into du
         from driverUser in du.DefaultIfEmpty()
         join Truck in _context.Truck on bol.TruckId equals Truck.Id into t
         from Truck in t.DefaultIfEmpty()
         join bolstatus in _context.BillOfLadingStatus on bol.BolStatusId equals bolstatus.Id into o
         from bolstatus in o.DefaultIfEmpty()
         join destination in _context.LoadOrigin on bol.DestinationId equals destination.Id into dest
         from destination in dest.DefaultIfEmpty()
         join finalDestination in _context.LoadOrigin on bol.FinalDestinationId equals finalDestination.Id into finaldest
         from finalDestination in finaldest.DefaultIfEmpty()
            join terminal in _context.Terminal on bol.TerminalId equals terminal.Id into term
            from terminal in term.DefaultIfEmpty()
            join refinery in _context.Refinery on bol.RefineryId equals refinery.Id into refin
            from refinery in refin.DefaultIfEmpty()
            join vendor in _context.Vendor on bol.VendorId equals vendor.Id into v
            from vendor in v.DefaultIfEmpty()
            join carrier in _context.Carrier on bol.CarrierId equals carrier.Id into c
            from carrier in c.DefaultIfEmpty()
            join refineryAddress in _context.RefineryAddress on bol.CarrierId equals refineryAddress.Id into ra
            from refineryAddress in ra.DefaultIfEmpty()
         select new BillOfLadingView
         {
             Id = bol.Id,
             OrderDate = bol.OrderDate,
             RequestDate = bol.RequestDate,
             BolStatusId = bolstatus,
             TruckId = Truck,
             DriverId = Driver == null || driverUser == null
                 ? new DriverView()
                 : new DriverView
                 {
                     Id = Driver.Id,
                     FirstName = driverUser.FirstName ?? "",
                     MiddleName = driverUser.MiddleName,
                     LastName = driverUser.LastName ?? "",
                     Email = driverUser.Email,
                     EmployeeCode = driverUser.EmployeeCode,
                     EffectiveDate = driverUser.EffectiveDate,
                     IsEnabled = Driver.IsEnabled && driverUser.IsEnabled,
                     IsVisible = Driver.IsVisible && driverUser.IsVisible,
                     PhoneNumber = driverUser.PhoneNumber,
                     Username = driverUser.Username,
                     Position = driverUser.Position,
                     AppUserId = Driver.AppUserId,
                     AspNetUserId = driverUser.AspNetUserId
                 },
             MarketId = Market,
             TerminalId = terminal,
             RefineryId = refinery,
             RefineryAddressId = refineryAddress,
             Notes = bol.Notes,
             DestinationId = destination,
             VendorId = vendor,
             CarrierId = carrier,
             FinalDestinationId = finalDestination,
             DeliveredDate = bol.DeliveredDate,
             IsEnabled = bol.IsEnabled,
             ImageExists = bol.ImageExists,
             ReceivedBol = bol.ReceivedBol,
             ReceivedInvoice = bol.ReceivedInvoice,
             PriceCheck = bol.PriceCheck,
             TaxCheck = bol.TaxCheck,
             Paid = bol.Paid,
             BolNumber = bol.BolNumber,
             LoadDate = bol.LoadDate
         })
            .AsQueryable();

        public async Task<PagingResults<BillOfLadingView>> GetBillOfLadingViewAsync(AppUser appUser, SieveModel sieveModel, int view = 0)
        {
            var query = BillOfLadingViewQuery;
            //query = view == 1 ? query.Where(o => o.BolStatusId.Id != AppConstants.OrderStatuses.Preticket).AsQueryable()
            //    : view == 0 ? query.Where(o => o.BolStatusId.Id == AppConstants.OrderStatuses.Preticket).AsQueryable() : query;

            //var role = appUser.Role.ToUpper();
            //switch (role)
            //{
            //    case "DRIVER":
            //        var driver = await _context.Driver.FirstAsync(x => x.AppUserId == appUser.Id);
            //        query = query.Where(x => x.DriverId.Id == driver.Id && x.BolStatusId.Id == AppConstants.OrderStatuses.Assigned).AsQueryable();

            //        break;
            //    case "SALES":
            //        var sales = await _context.SalesRep.FirstAsync(x => x.AppUserId == appUser.Id);
            //        query = query.Where(x => x.SalesRepId.Id == sales.Id).AsQueryable();
            //        break;
            //}
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return data;
        }

        public IEnumerable<BillOfLadingProductView> GetBillOfLadingProducts(int bolId)
        {
            IEnumerable<BillOfLadingProductView> billOfLadingProductView;

            billOfLadingProductView = (from bolproduct in _context.BillOfLadingProduct
                                 join product in _context.Product on bolproduct.ProductId equals product.Id
                                 join unit in _context.Unit on bolproduct.UnitId equals unit.Id
                                 where bolproduct.BillOfLadingId == bolId
                                       select new BillOfLadingProductView { Id = bolproduct.Id, BillOfLadingId = bolId, ProductId = product,
                                     Quantity = bolproduct.Quantity, UnitId = unit, Cost = bolproduct.Cost }).AsEnumerable();

            return billOfLadingProductView;
        }

        public BillOfLadingProductView PostBillOfLadingProduct(BillOfLadingProduct billOfLadingProduct)
        {
            BillOfLadingProductView bolProductView = (from bolproduct in _context.BillOfLadingProduct
                                                   join product in _context.Product on bolproduct.ProductId equals product.Id
                                                   //join bol in _context.Order on bolproduct.BillOfLadingId equals bol.Id
                                                   join unit in _context.Unit on bolproduct.UnitId equals unit.Id
                                                   where bolproduct.Id == billOfLadingProduct.Id
                                                   select new BillOfLadingProductView { Id = bolproduct.Id, BillOfLadingId = bolproduct.BillOfLadingId,
                                                       ProductId = product, Quantity = bolproduct.Quantity, UnitId = unit, Cost = bolproduct.Cost }).FirstOrDefault();

            return bolProductView;
        }

        //public string PostBolScan(IFormFile file)
        //{
        //    if (file != null)
        //    {
        //        var fileExt = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();

        //        if (!fileExt.Contains("png") && !fileExt.Contains("jpeg") && !fileExt.Contains("jpg"))
        //        {
        //            return "Invalid file type";
        //        }

        //        string fileName = Guid.NewGuid() + fileExt;
        //        var path = Path.Combine(Directory.GetCurrentDirectory(), "./Images/BolImage/", fileName);

        //        using (var stream = new FileStream(path, FileMode.Create))
        //        {
        //            file.CopyToAsync(stream);
        //        }

        //        return fileName;
        //    }

        //    return "file are not selected";
        //}
    }
}

