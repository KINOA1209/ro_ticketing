using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using ticketing_api.Models.Views;

namespace ticketing_api.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationSieveProcessor _sieveProcessor;

        public OrderService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        public async Task<string> CreateTicketFromOrderAsync(Order order)
        {
            order.OrderStatusId = AppConstants.OrderStatuses.Open;

            _context.Order.Update(order);

            await _context.SaveChangesAsync();

            var orderStatus = from o in _context.Order
                              join status in _context.OrderStatus
                                  on o.OrderStatusId equals status.Id
                              where o.Id == order.Id
                              select new { status.Name };

            string orderStatusName = orderStatus.FirstOrDefault()?.Name;

            return orderStatusName;
        }

        public async Task<string> CreateOrderFromTicketAsync(Order order)
        {
            order.OrderStatusId = AppConstants.OrderStatuses.Preticket;

            _context.Order.Update(order);

            await _context.SaveChangesAsync();

            var orderStatus = from o in _context.Order
                              join status in _context.OrderStatus
                                  on o.OrderStatusId equals status.Id
                              where o.Id == order.Id
                              select new { status.Name };

            string orderStatusName = orderStatus.FirstOrDefault()?.Name;

            return orderStatusName;
        }

        public async Task<string> CreateVoidTicketAsync(Order order)
        {
            order.OrderStatusId = AppConstants.OrderStatuses.Voided;

            _context.Order.Update(order);

            await _context.SaveChangesAsync();

            var orderStatus = from o in _context.Order
                              join status in _context.OrderStatus
                                  on o.OrderStatusId equals status.Id
                              where o.Id == order.Id
                              select new { status.Name };

            string orderStatusName = orderStatus.FirstOrDefault()?.Name;

            return orderStatusName;
        }


        private IQueryable<OrderView> OrderViewQuery =>

                (from order in _context.Order
                 join Market in _context.Market on order.MarketId equals Market.Id into m
                 from Market in m.DefaultIfEmpty()
                 join county in _context.County on order.CountyId equals county.Id into c
                 from county in c.DefaultIfEmpty()
                 join timeslot in _context.RequestDeliveryTimeSlot on order.RequestDeliveryTimeSlotId equals timeslot.Id into ts
                 from timeslot in ts.DefaultIfEmpty()

                     //join well in _context.Well on order.WellId equals well.Id into w
                     //from well in w.DefaultIfEmpty()
                 join Driver in _context.Driver on order.DriverId equals Driver.Id into d
                 from Driver in d.DefaultIfEmpty()
                 join driverUser in _context.AppUser on Driver.AppUserId equals driverUser.Id into du
                 from driverUser in du.DefaultIfEmpty()
                 join Truck in _context.Truck on order.TruckId equals Truck.Id into t
                 from Truck in t.DefaultIfEmpty()
                 join customer in _context.Customer on order.CustomerId equals customer.Id into cs
                 from customer in cs.DefaultIfEmpty()
                 join riglocation in _context.RigLocation on order.RigLocationId equals riglocation.Id into rig
                 from riglocation in rig.DefaultIfEmpty()
                 join jobtype in _context.JobType on order.JobTypeId equals jobtype.Id into job
                 from jobtype in job.DefaultIfEmpty()
                 join orderstatus in _context.OrderStatus on order.OrderStatusId equals orderstatus.Id into o
                 from orderstatus in o.DefaultIfEmpty()
                 join salesrep in _context.SalesRep on order.SalesRepId equals salesrep.Id into sp
                 from salesrep in sp.DefaultIfEmpty()
                 join salesrepUser in _context.AppUser on salesrep.AppUserId equals salesrepUser.Id into su
                 from salesrepUser in su.DefaultIfEmpty()
                 join loadOrigin in _context.LoadOrigin on order.LoadOriginId equals loadOrigin.Id into lo
                 from loadOrigin in lo.DefaultIfEmpty()
                 where !order.IsDeleted
                 select new OrderView
                 {
                     Id = order.Id,
                     OrderDate = order.OrderDate,
                     RequestDate = order.RequestDate,
                     RequestTime = order.RequestTime,
                     OrderStatusId = orderstatus,
                     JobTypeId = jobtype,
                     CustomerId = customer,
                     CustomerNotes = order.CustomerNotes,
                     AFEPO = order.AFEPO,
                     RigLocationId = riglocation,
                     RigLocationNotes = order.RigLocationNotes,
                     //WellId = well,
                     WellName = order.WellName,
                     WellCode = order.WellCode,
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
                     PointOfContactName = order.PointOfContactName,
                     PointOfContactEmail = order.PointOfContactEmail,
                     PointOfContactNumber = order.PointOfContactNumber,
                     MarketId = Market,
                     CountyId = county,
                     LoadOriginId = loadOrigin,
                     SalesRepId = salesrep == null || salesrepUser == null
                         ? new SalesRepView()
                         : new SalesRepView()
                         {
                             Id = salesrep.Id,
                             FirstName = salesrepUser.FirstName ?? "",
                             MiddleName = salesrepUser.MiddleName ?? "",
                             LastName = salesrepUser.LastName ?? "",
                             Email = salesrepUser.Email,
                             EmployeeCode = salesrepUser.EmployeeCode,
                             EffectiveDate = salesrepUser.EffectiveDate,
                             IsEnabled = salesrep.IsEnabled && salesrepUser.IsEnabled,
                             IsVisible = salesrep.IsVisible && salesrepUser.IsVisible,
                             PhoneNumber = salesrepUser.PhoneNumber,
                             Username = salesrepUser.Username,
                             Position = salesrepUser.Position,
                             Role = salesrepUser.Role,
                             AppUserId = salesrep.AppUserId,
                             AspNetUserId = salesrepUser.AspNetUserId,
                             Notes = salesrep.Notes
                         },
                     OrderDescription = order.OrderDescription,
                     WellDirection = order.WellDirection,
                     InternalNotes = order.InternalNotes,
                     ShippingPaperNA = order.ShippingPaperNA,
                     FuelSurchargeNA = order.FuelSurchargeNA,
                     ApprovedForBilling = order.ApprovedForBilling,
                     ShippingPaperExists = order.ShippingPaperExists,
                     TicketImageExists = order.TicketImageExists,
                     SpecialHandling = order.SpecialHandling,
                     DeliveredDate = order.DeliveredDate,
                     RequestDeliveryTimeSlotId = timeslot,
                     IsEnabled = order.IsEnabled
                 })
                    .AsQueryable();

        public OrderView GetOrderView(int id)
        {
            var orderView = OrderViewQuery.FirstOrDefault(o => o.Id == id);
            return orderView;
        }

        // view = 0 orders only, 1 tickets only, 2 all history
        public async Task<PagingResults<OrderView>> GetOrderViewAsync(AppUser appUser, SieveModel sieveModel, int view = 0)
        {
            var query = OrderViewQuery;
            query = view == 1 ? query.Where(o => o.OrderStatusId.Id != AppConstants.OrderStatuses.Preticket).AsQueryable()
                : view == 0 ? query.Where(o => o.OrderStatusId.Id == AppConstants.OrderStatuses.Preticket).AsQueryable() : query;

            var role = appUser.Role.ToUpper();
            switch (role)
            {
                case "DRIVER":
                    var driver = await _context.Driver.FirstAsync(x => x.AppUserId == appUser.Id);
                    query = query.Where(x => x.DriverId.Id == driver.Id && x.OrderStatusId.Id == AppConstants.OrderStatuses.Assigned).AsQueryable();

                    break;
                case "SALES":
                    var sales = await _context.SalesRep.FirstAsync(x => x.AppUserId == appUser.Id);
                    query = query.Where(x => x.SalesRepId.Id == sales.Id).AsQueryable();
                    break;
            }

            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return data;
        }

        public OrderView PostOrderView(Order order)
        {
            //var driverService = new DriverService(_context, _sieveProcessor);
            //var salesRepService = new SalesRepService(_context, _sieveProcessor);

            OrderView orderView = (from Order in _context.Order
                                   join Market in _context.Market on order.MarketId equals Market.Id into m
                                   from Market in m.DefaultIfEmpty()
                                   join county in _context.County on order.CountyId equals county.Id into c
                                   from county in c.DefaultIfEmpty()
                join timeslot in _context.RequestDeliveryTimeSlot on order.RequestDeliveryTimeSlotId equals timeslot.Id into ts
                from timeslot in ts.DefaultIfEmpty()

                                       //join well in _context.Well on order.WellId equals well.Id into w
                                       //from well in w.DefaultIfEmpty()
                                   join Driver in _context.Driver on order.DriverId equals Driver.Id into d
                                   from Driver in d.DefaultIfEmpty()
                                   join driverUser in _context.AppUser on Driver.AppUserId equals driverUser.Id into du
                                   from driverUser in du.DefaultIfEmpty()
                                   join Truck in _context.Truck on order.TruckId equals Truck.Id into t
                                   from Truck in t.DefaultIfEmpty()
                                   join customer in _context.Customer on order.CustomerId equals customer.Id into cs
                                   from customer in cs.DefaultIfEmpty()
                                   join riglocation in _context.RigLocation on order.RigLocationId equals riglocation.Id into rig
                                   from riglocation in rig.DefaultIfEmpty()
                                   join jobtype in _context.JobType on order.JobTypeId equals jobtype.Id into job
                                   from jobtype in job.DefaultIfEmpty()
                                   join orderstatus in _context.OrderStatus on order.OrderStatusId equals orderstatus.Id into o
                                   from orderstatus in o.DefaultIfEmpty()
                                   join salesrep in _context.SalesRep on order.SalesRepId equals salesrep.Id into sp
                                   from salesrep in sp.DefaultIfEmpty()
                                   join salesrepUser in _context.AppUser on salesrep.AppUserId equals salesrepUser.Id into su
                                   from salesrepUser in su.DefaultIfEmpty()
                                   join loadOrigin in _context.LoadOrigin on order.LoadOriginId equals loadOrigin.Id into lo
                                   from loadOrigin in lo.DefaultIfEmpty()
                                   where Order.Id == order.Id && !Order.IsDeleted
                                   select new OrderView
                                   {
                                       Id = order.Id,
                                       OrderDate = order.OrderDate,
                                       RequestDate = order.RequestDate,
                                       RequestTime = order.RequestTime,
                                       OrderStatusId = orderstatus,
                                       JobTypeId = jobtype,
                                       CustomerId = customer,
                                       CustomerNotes = order.CustomerNotes,
                                       AFEPO = order.AFEPO,
                                       RigLocationId = riglocation,
                                       RigLocationNotes = order.RigLocationNotes,
                                       WellName = order.WellName,
                                       //WellId = well,
                                       WellCode = order.WellCode,
                                       TruckId = Truck,
                                       DriverId = Driver == null || driverUser == null
                                           ? new DriverView()
                                           : new DriverView()
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
                                               AspNetUserId = salesrepUser.AspNetUserId
                                           },
                                       PointOfContactName = order.PointOfContactName,
                                       PointOfContactEmail = order.PointOfContactEmail,
                                       PointOfContactNumber = order.PointOfContactNumber,
                                       MarketId = Market,
                                       CountyId = county,
                                       LoadOriginId = loadOrigin,
                                       SalesRepId = salesrep == null || salesrepUser == null
                                           ? new SalesRepView()
                                           : new SalesRepView()
                                           {
                                               Id = salesrep.Id,
                                               FirstName = salesrepUser.FirstName ?? "",
                                               MiddleName = salesrepUser.MiddleName ?? "",
                                               LastName = salesrepUser.LastName ?? "",
                                               Email = salesrepUser.Email,
                                               EmployeeCode = salesrepUser.EmployeeCode,
                                               EffectiveDate = salesrepUser.EffectiveDate,
                                               IsEnabled = salesrep.IsEnabled && salesrepUser.IsEnabled,
                                               IsVisible = salesrep.IsVisible && salesrepUser.IsVisible,
                                               PhoneNumber = salesrepUser.PhoneNumber,
                                               Username = salesrepUser.Username,
                                               Position = salesrepUser.Position,
                                               Role = salesrepUser.Role,
                                               AppUserId = salesrep.AppUserId,
                                               AspNetUserId = salesrepUser.AspNetUserId,
                                               Notes = salesrep.Notes
                                           },
                                       OrderDescription = order.OrderDescription,
                                       WellDirection = order.WellDirection,
                                       InternalNotes = order.InternalNotes,
                                       ShippingPaperNA = order.ShippingPaperNA,
                                       FuelSurchargeNA = order.FuelSurchargeNA,
                                       ApprovedForBilling = order.ApprovedForBilling,
                                       ShippingPaperExists = order.ShippingPaperExists,
                                       TicketImageExists = order.TicketImageExists,
                                       SpecialHandling = order.SpecialHandling,
                                       DeliveredDate = order.DeliveredDate,
                                       RequestDeliveryTimeSlotId = timeslot,
                                       IsEnabled = order.IsEnabled,
                                   }).FirstOrDefault();

            return orderView;
        }
    }
}
