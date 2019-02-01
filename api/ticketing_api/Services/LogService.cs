using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ticketing_api.Controllers;
using ticketing_api.Controllers.BaseController;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using Microsoft.EntityFrameworkCore;
using ticketing_api.Models.Views;
using OrderView = ticketing_api.Models.Views.OrderView;

namespace ticketing_api.Services
{
    public class LogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationSieveProcessor _sieveProcessor;

        public LogService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        public async Task StoreLogInformationPost(OrderView orderView, Order order, string method, string userId)
        {
            Log orderLog = new Log { };
            orderLog.OrderId = order.Id;
            orderLog.UserId = userId;
            orderLog.EventType = "Add Order";
            orderLog.IsTable = "Order";

            var orderViewResult = orderView;

            OrderLogView orderlogView = new OrderLogView { };

            orderlogView.Id = orderViewResult.Id;
            orderlogView.OrderDate = orderViewResult.OrderDate;
            orderlogView.RequestDate = orderViewResult.RequestDate;
            orderlogView.RequestTime = orderViewResult.RequestTime;
            orderlogView.OrderStatus = orderViewResult.OrderStatusId.Name;
            orderlogView.JobTypeName = order.JobTypeId == 0 ? null : orderViewResult.JobTypeId.Name;
            orderlogView.CustomerName = order.CustomerId == 0 ? null: orderViewResult.CustomerId.Name;
            orderlogView.CustomerNotes = orderViewResult.CustomerNotes;
            orderlogView.RigLocationName = order.RigLocationId == 0 ? null : orderViewResult.RigLocationId.Name;
            orderlogView.RigLocationNotes = orderViewResult.RigLocationNotes;
            orderlogView.WellName = order.WellName;
            //orderlogView.WellName = order.WellId == 0 ? null: orderViewResult.WellId.Name;
            orderlogView.WellCode = order.WellCode;
            orderlogView.AFEPO = orderViewResult.AFEPO;
            orderlogView.PointOfContactName = orderViewResult.PointOfContactName;
            orderlogView.PointOfContactEmail = orderViewResult.PointOfContactEmail;
            orderlogView.PointOfContactNumber = orderViewResult.PointOfContactNumber;
            orderlogView.MarketId = order.MarketId == 0 ? null : orderViewResult.MarketId.Description;
            orderlogView.SalesRepName = order.SalesRepId == 0 ? null : orderViewResult.SalesRepId.FirstName + " " + orderViewResult.SalesRepId.LastName;
            orderlogView.OrderDescription = orderViewResult.OrderDescription;
            orderlogView.WellDirection = orderViewResult.WellDirection;
            orderlogView.InternalNotes = orderViewResult.InternalNotes;

            LogKey logKey = new LogKey { };

            logKey.Action = "Add";
            logKey.Data = orderlogView;

            var description = JsonConvert.SerializeObject(logKey);

            orderLog.Description = description;
            orderLog.CreatedTime = DateTime.UtcNow;
            _context.OrderLog.Add(orderLog);
            await _context.SaveChangesAsync();
        }

        public async Task StoreLogInformationCreateOrderFromTicket(int id, string orderStatusBefore, string orderStatusAfter, string method, string userId)
        {
            Log orderLog = new Log { };
            orderLog.OrderId = id;
            orderLog.UserId = userId;
            orderLog.EventType = method;

            LogUpdate logUpdate = new LogUpdate { };
            logUpdate.FieldName = "OrderStatus";
            logUpdate.Old = orderStatusBefore;
            logUpdate.New = orderStatusAfter;

            LogKey logKey = new LogKey { };
            logKey.Action = "Convert";
            logKey.Data = logUpdate;
            var description = JsonConvert.SerializeObject(logKey);

            if (orderStatusBefore == "OPEN" && method == "PostOrderTicket")
            {
                orderLog.EventType = "Change Order to Ticket";
                orderLog.IsTable = "Order";
            }
            else if (orderStatusBefore == "ASSIGNED" && method == "PostTicketOrder")
            {
                orderLog.EventType = "Change Ticket to Order";
                orderLog.IsTable = "Ticket";
            }
            else
            {
                orderLog.EventType = "Change Ticket to VoidTicket";
                orderLog.IsTable = "Ticket";
            }

            orderLog.Description = description;
            orderLog.CreatedTime = DateTime.UtcNow;
            _context.OrderLog.Add(orderLog);
            await _context.SaveChangesAsync();
        }

        public async Task StoreLogInformationDelete(int id, string method, string userId)
        {
            Log orderLog = new Log { };
            orderLog.UserId = userId;
            orderLog.CreatedTime = DateTime.UtcNow;
            orderLog.OrderId = id;

            LogDelete logDelete = new LogDelete { };
            logDelete.FieldName = "Id";
            logDelete.DeleteId = id;

            LogKey logKey = new LogKey { };
            logKey.Action = "Delete";
            logKey.Data = logDelete;
            var description = JsonConvert.SerializeObject(logKey);

            if (method == "DeleteOrder")
            {
                orderLog.IsTable = "Order";
                orderLog.EventType = "Delete Order";
            }
            else if (method == "DeleteTicket")
            {
                orderLog.IsTable = "Ticket";
                orderLog.EventType = "Delete Ticket";
            }
            else if (method == "DeleteCustomer")
            {
                orderLog.IsTable = "Customer";
                orderLog.EventType = "Delete Customer";
            }
            else if (method == "DeleteRigLocation")
            {
                orderLog.IsTable = "RigLocation";
                orderLog.EventType = "Delete RigLocation";
            }
            else
            {
                orderLog.IsTable = "Ticket";
                orderLog.EventType = "Delete TicketProduct";
            }

            orderLog.Description = description;
            _context.OrderLog.Add(orderLog);
            await _context.SaveChangesAsync();
        }

        public async Task StoreLogInformationPut(OrderView orderUpdateAfter, Order orderAfter, string method, string userId, Order orderBefore, OrderView orderUpdateBefore)
        {
            Log orderLog = new Log();
            OrderView orderUpdateBeforeView = orderUpdateBefore;
            OrderView orderUpdateAfterView = orderUpdateAfter;
            List<LogUpdate> listLogUpdate = new List<LogUpdate>();

            if (method == "PutOrderAsync" || method == "PutInternalNotes")
            {
                if (orderUpdateAfterView.OrderStatusId.Id == AppConstants.OrderStatuses.Preticket)
                {
                    orderLog.IsTable = "Ticket";
                    orderLog.EventType = "Update Ticket";
                }
                else
                {
                    orderLog.IsTable = "Order";
                    orderLog.EventType = "Update Order";
                }

                if (orderBefore.RequestDate != orderAfter.RequestDate)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "RequestDate";
                    logUpdate.Old = orderUpdateBeforeView.OrderDate;
                    logUpdate.New = orderUpdateAfterView.OrderDate;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.OrderStatusId != orderAfter.OrderStatusId)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "RequestTime";
                    logUpdate.Old = orderUpdateBeforeView.RequestTime;
                    logUpdate.New = orderUpdateAfterView.RequestTime;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.OrderStatusId != orderAfter.OrderStatusId)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "OrderStatus";
                    logUpdate.Old = orderUpdateBeforeView.OrderStatusId.Name;
                    logUpdate.New = orderUpdateAfterView.OrderStatusId.Name;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.JobTypeId != orderAfter.JobTypeId)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "JobTypeName";
                    logUpdate.Old = orderUpdateBeforeView.JobTypeId.Name;
                    logUpdate.New = orderUpdateAfterView.JobTypeId.Name;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.CustomerId != orderAfter.CustomerId)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "CustomerName";
                    logUpdate.Old = orderUpdateBeforeView.CustomerId.Name;
                    logUpdate.New = orderUpdateAfterView.CustomerId.Name;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.CustomerNotes != orderAfter.CustomerNotes)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "CustomerNotes";
                    logUpdate.Old = orderUpdateBeforeView.CustomerNotes;
                    logUpdate.New = orderUpdateAfterView.CustomerNotes;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.RigLocationId != orderAfter.RigLocationId)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "RigLocationName";
                    logUpdate.Old = orderUpdateBeforeView.RigLocationId.Name;
                    logUpdate.New = orderUpdateAfterView.RigLocationId.Name;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.RigLocationNotes != orderAfter.RigLocationNotes)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "RigLocationNotes";
                    logUpdate.Old = orderUpdateBeforeView.RigLocationNotes;
                    logUpdate.New = orderUpdateAfterView.RigLocationNotes;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.WellName != orderAfter.WellName)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "WellName";
                    logUpdate.Old = orderUpdateBeforeView.WellName;
                    logUpdate.New = orderUpdateAfterView.WellName;
                    listLogUpdate.Add(logUpdate);
                }

                //if (orderBefore.WellId != orderAfter.WellId)
                //{
                //    LogUpdate logUpdate = new LogUpdate { };
                //    logUpdate.FieldName = "WellName";
                //    logUpdate.Old = orderUpdateBeforeView.WellId.Name;
                //    logUpdate.New = orderUpdateAfterView.WellId.Name;
                //    listLogUpdate.Add(logUpdate);
                //}

                if (orderBefore.WellCode != orderAfter.WellCode)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "WellCode";
                    logUpdate.Old = orderUpdateBeforeView.WellCode;
                    logUpdate.New = orderUpdateAfterView.WellCode;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.OrderStatusId == AppConstants.OrderStatuses.Assigned)
                {
                    if (orderBefore.DriverId != orderAfter.DriverId)
                    {
                        LogUpdate logUpdate = new LogUpdate();
                        if (orderBefore.DriverId == 0)
                        {
                            logUpdate.FieldName = "DriverName";
                            logUpdate.Old = "Null";
                            logUpdate.New = orderUpdateAfterView.DriverId.FullName;
                            listLogUpdate.Add(logUpdate);
                        }
                        else
                        {
                            logUpdate.FieldName = "DriverName";
                            logUpdate.Old = orderUpdateBeforeView.DriverId.FullName;
                            logUpdate.New = orderUpdateAfterView.DriverId.FullName;
                            listLogUpdate.Add(logUpdate);
                        }
                    }

                    if (orderBefore.TruckId != orderAfter.TruckId)
                    {
                        LogUpdate logUpdate = new LogUpdate();
                        if (orderBefore.TruckId == 0)
                        {
                            logUpdate.FieldName = "TruckName";
                            logUpdate.Old = "Null";
                            logUpdate.New = orderUpdateAfterView.TruckId.Name;
                            listLogUpdate.Add(logUpdate);
                        }
                        else
                        {
                            logUpdate.FieldName = "TruckName";
                            logUpdate.Old = orderUpdateBeforeView.TruckId.Name;
                            logUpdate.New = orderUpdateAfterView.TruckId.Name;
                            listLogUpdate.Add(logUpdate);
                        }
                    }
                }

                if (orderBefore.AFEPO != orderAfter.AFEPO)
                {
                    LogUpdate logUpdate = new LogUpdate();
                    logUpdate.FieldName = "AFEPO";
                    logUpdate.Old = orderUpdateBeforeView.AFEPO;
                    logUpdate.New = orderUpdateAfterView.AFEPO;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.PointOfContactName != orderAfter.PointOfContactName)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "PointOfContactName";
                    logUpdate.Old = orderUpdateBeforeView.PointOfContactName;
                    logUpdate.New = orderUpdateAfterView.PointOfContactName;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.PointOfContactNumber != orderAfter.PointOfContactNumber)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "PointOfContactNumber";
                    logUpdate.Old = orderUpdateBeforeView.PointOfContactNumber;
                    logUpdate.New = orderUpdateAfterView.PointOfContactNumber;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.MarketId != orderAfter.MarketId)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    if (orderBefore.MarketId == 0)
                    {
                        logUpdate.FieldName = "MarketDescription";
                        logUpdate.Old = "Null";
                        logUpdate.New = orderUpdateAfterView.MarketId.Description;
                        listLogUpdate.Add(logUpdate);
                    }
                    else
                    {
                        logUpdate.FieldName = "MarketDescription";
                        logUpdate.Old = orderUpdateBeforeView.MarketId.Description;
                        logUpdate.New = orderUpdateAfterView.MarketId.Description;
                        listLogUpdate.Add(logUpdate);
                    }
                }

                if (orderBefore.SalesRepId != orderAfter.SalesRepId)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    if (orderBefore.SalesRepId == 0)
                    {
                        logUpdate.FieldName = "SalesRepName";
                        logUpdate.Old = "Null";
                        logUpdate.New = orderUpdateAfterView.SalesRepId.FirstName + " " + orderUpdateAfterView.SalesRepId.LastName;
                        listLogUpdate.Add(logUpdate);
                    }
                    else
                    {
                        logUpdate.FieldName = "SalesRepName";
                        logUpdate.Old = orderUpdateBeforeView.SalesRepId.FirstName + " " + orderUpdateBeforeView.SalesRepId.LastName;
                        logUpdate.New = orderUpdateAfterView.SalesRepId.FirstName + " " + orderUpdateAfterView.SalesRepId.LastName;
                        listLogUpdate.Add(logUpdate);
                    }
                }

                if (orderBefore.OrderDescription != orderAfter.OrderDescription)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "OrderDescription";
                    logUpdate.Old = orderUpdateBeforeView.OrderDescription;
                    logUpdate.New = orderUpdateAfterView.OrderDescription;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.WellDirection != orderAfter.WellDirection)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "WellDirection";
                    logUpdate.Old = orderUpdateBeforeView.WellDirection;
                    logUpdate.New = orderUpdateAfterView.WellDirection;
                    listLogUpdate.Add(logUpdate);
                }

                if (orderBefore.InternalNotes != orderAfter.InternalNotes)
                {
                    LogUpdate logUpdate = new LogUpdate { };
                    logUpdate.FieldName = "InternalNotes";
                    logUpdate.Old = orderUpdateBeforeView.InternalNotes;
                    logUpdate.New = orderUpdateAfterView.InternalNotes;
                    listLogUpdate.Add(logUpdate);
                }
            }

            LogKey logKey = new LogKey { };
            logKey.Action = "Update";
            logKey.Data = listLogUpdate;
            var description = JsonConvert.SerializeObject(logKey);

            if (listLogUpdate.Count != 0)
            {
                orderLog.OrderId = orderUpdateAfterView.Id;
                orderLog.UserId = userId;
                orderLog.Description = description;
                orderLog.CreatedTime = DateTime.UtcNow;
                _context.OrderLog.Add(orderLog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task StoreTicketLogInformation(int orderId, string ticketImageOld, string ticketImage, string method, string userId)
        {
            Log orderLog = new Log { };
            orderLog.OrderId = orderId;
            orderLog.UserId = userId;
            orderLog.EventType = "Update TicketImage";
            orderLog.IsTable = "Ticket";

            LogUpdate logUpdate = new LogUpdate { };
            logUpdate.FieldName = "Ticketimg";
            logUpdate.Old = ticketImageOld;
            logUpdate.New = ticketImage;

            LogKey logKey = new LogKey { };
            logKey.Action = "Update";
            logKey.Data = logUpdate;
            var description = JsonConvert.SerializeObject(logKey);

            orderLog.Description = description;
            orderLog.CreatedTime = DateTime.UtcNow;
            _context.OrderLog.Add(orderLog);
            await _context.SaveChangesAsync();
        }

        public async Task StoreTicketProductLogInformation(int orderId, TicketProductView ticketProductView, string method, string userid)
        {
            Log orderLog = new Log { };
            orderLog.UserId = userid;
            orderLog.EventType = "Add TicketProduct";
            orderLog.IsTable = "Ticket";

            orderLog.OrderId = orderId;

            TicketProductLogView ticketProductLogView = new TicketProductLogView { };
            ticketProductLogView.Id = ticketProductView?.Id ?? 0;
            ticketProductLogView.OrderId = ticketProductView?.TicketId ?? 0;
            ticketProductLogView.ProductName = ticketProductView?.ProductId.Name;
            ticketProductLogView.Quantity = ticketProductView.Quantity;
            ticketProductLogView.UnitName = ticketProductView.UnitId.Name;
            ticketProductLogView.Price = ticketProductView.Price;
         
            LogKey logKey = new LogKey { };
            logKey.Action = "Add";
            logKey.Data = ticketProductLogView;
            var description = JsonConvert.SerializeObject(logKey);

            orderLog.Description = description;
            orderLog.CreatedTime = DateTime.UtcNow;
            _context.OrderLog.Add(orderLog);
            await _context.SaveChangesAsync();
        }

        public async Task StoreTicketProductLogInformationPut(TicketProductView ticketProductViewAfter, TicketProduct ticketProductAfter, string method, string userId, TicketProductView ticketProductViewBefore, TicketProduct ticketProductBefore)
        {
            Log orderLog = new Log { };
            orderLog.EventType = "Update TicketProduct";   
            orderLog.IsTable = "Ticket";

            List<LogUpdate> listLogUpdate = new List<LogUpdate>();

            if (ticketProductBefore.TicketId != ticketProductAfter.TicketId)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "TicketId";
                logUpdate.Old = ticketProductViewBefore.TicketId;
                logUpdate.New = ticketProductViewAfter.TicketId;
                listLogUpdate.Add(logUpdate);
            }

            if (ticketProductBefore.ProductId != ticketProductAfter.ProductId)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "ProductId";
                logUpdate.Old = ticketProductViewBefore.ProductId.Name;
                logUpdate.New = ticketProductViewAfter.ProductId.Name;
                listLogUpdate.Add(logUpdate);
            }

            if (ticketProductBefore.Quantity != ticketProductAfter.Quantity)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "Quantity";
                logUpdate.Old = ticketProductViewBefore.Quantity;
                logUpdate.New = ticketProductViewAfter.Quantity;
                listLogUpdate.Add(logUpdate);
            }

            if (ticketProductBefore.UnitId != ticketProductAfter.UnitId)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "UnitId";
                logUpdate.Old = ticketProductViewBefore.UnitId.Name;
                logUpdate.New = ticketProductViewAfter.UnitId.Name;
                listLogUpdate.Add(logUpdate);
            }

            if (ticketProductBefore.Price != ticketProductAfter.Price)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "Price";
                logUpdate.Old = ticketProductViewBefore.Price;
                logUpdate.New = ticketProductViewAfter.Price;
                listLogUpdate.Add(logUpdate);
            }

            LogKey logKey = new LogKey { };
            logKey.Action = "Update";
            logKey.Data = listLogUpdate;
            var description = JsonConvert.SerializeObject(logKey);

            if (listLogUpdate.Count != 0)
            {
                orderLog.OrderId = ticketProductAfter.Id;
                orderLog.UserId = userId;
                orderLog.Description = description;
                orderLog.CreatedTime = DateTime.UtcNow;
                _context.OrderLog.Add(orderLog);
                await _context.SaveChangesAsync();
            }

        }

        public async Task StoreRigLocationNoteLoginformationPut(RigLocationNoteView rigNoteView, string method, string userid, RigLocationNote rigLocationNoteUpdateBefore)
        {
            Log orderLog = new Log();

            RigLocationNoteView rigLocationNoteUpdateAfter = rigNoteView;

            if (rigLocationNoteUpdateAfter != null)
            {
                orderLog.OrderId = rigLocationNoteUpdateAfter.Id;
                orderLog.EventType = method;
                orderLog.UserId = userid;
                orderLog.IsTable = "RigNote";
                var rigLocationId = "";
                var rigNote = "";

                if (rigLocationNoteUpdateBefore.RigLocationId != rigLocationNoteUpdateAfter.RigLocationId.Id)
                {
                    rigLocationId = "Riglocation Updated to " + rigLocationNoteUpdateAfter.RigLocationId.Name + ", ";
                }

                if (rigLocationNoteUpdateBefore.RigNote != rigLocationNoteUpdateAfter.RigNote)
                {
                    rigNote = "RigNote Updated to " + rigLocationNoteUpdateAfter.RigNote + "";
                }


                if (method == "PutRigLocationNote")
                {
                    orderLog.Description = rigLocationId + rigNote;
                }
            }

            orderLog.CreatedTime = DateTime.UtcNow;
            _context.OrderLog.Add(orderLog);
            await _context.SaveChangesAsync();

        }

        public async Task StoreCustomerNoteLogInformationPut(CustomerNoteView customerNoteView, string method, string userid, CustomerNote customerNoteUpdateBefore)
        {
            Log orderLog = new Log();
            CustomerNoteView customerNoteUpdateAfter = customerNoteView;

            if (customerNoteUpdateAfter != null)
            {
                orderLog.OrderId = customerNoteUpdateAfter.Id;
                orderLog.EventType = method;
                orderLog.UserId = userid;
                orderLog.IsTable = "CustomerNote";
                var customerId = "";
                var customerNote = "";

                if (customerNoteUpdateBefore.CustomerId != customerNoteUpdateAfter.CustomerId.Id)
                {
                    customerId = "Customer Updated to " + customerNoteUpdateAfter.CustomerId.Name + ", ";
                }

                if (customerNoteUpdateBefore.CustomerNotes != customerNoteUpdateAfter.CustomerNotes)
                {
                    customerNote = "CustomerNotes Updated to " + customerNoteUpdateAfter.CustomerNotes + "";
                }

                if (method == "PutCustomerNotes")
                {
                    orderLog.Description = customerId + customerNote;
                }
            }

            orderLog.CreatedTime = DateTime.UtcNow;
            _context.OrderLog.Add(orderLog);
            await _context.SaveChangesAsync();

        }

        public async Task StoreCustomerLogInformationPost(Customer customer, string method, string userId)
        {
            Log orderLog = new Log { };

            orderLog.OrderId = customer.Id;
            orderLog.UserId = userId;
            orderLog.EventType = "Add Customer";
            orderLog.IsTable = "Customer";

            LogKey logKey = new LogKey { };
            logKey.Action = "Add";
            logKey.Data = customer;
            var description = JsonConvert.SerializeObject(logKey);

            orderLog.Description = description;
            orderLog.CreatedTime = DateTime.UtcNow;
            _context.OrderLog.Add(orderLog);
            await _context.SaveChangesAsync();
        }

        public async Task StoreCustomerLogInformationPut(Customer customerUpdateAfter, string method, string userId, Customer customerUpdateBefore)
        {
            Log orderLog = new Log();

            List<LogUpdate> listLogUpdate = new List<LogUpdate>();

            if (customerUpdateBefore.Name != customerUpdateAfter.Name)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "Name";
                logUpdate.Old = customerUpdateBefore.Name;
                logUpdate.New = customerUpdateAfter.Name;
                listLogUpdate.Add(logUpdate);
            }

            if (customerUpdateBefore.ContactName != customerUpdateAfter.ContactName)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "ContactName";
                logUpdate.Old = customerUpdateBefore.ContactName;
                logUpdate.New = customerUpdateAfter.ContactName;
                listLogUpdate.Add(logUpdate);
            }

            if (customerUpdateBefore.Email != customerUpdateAfter.Email)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "Email";
                logUpdate.Old = customerUpdateBefore.Email;
                logUpdate.New = customerUpdateAfter.Email;
                listLogUpdate.Add(logUpdate);
            }

            if (customerUpdateBefore.ContactPhone != customerUpdateAfter.ContactPhone)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "ContactPhone";
                logUpdate.Old = customerUpdateBefore.ContactPhone;
                logUpdate.New = customerUpdateAfter.ContactPhone;
                listLogUpdate.Add(logUpdate);
            }

            if (customerUpdateBefore.FaxNumber != customerUpdateAfter.FaxNumber)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "Fax";
                logUpdate.Old = customerUpdateBefore.FaxNumber;
                logUpdate.New = customerUpdateAfter.FaxNumber;
                listLogUpdate.Add(logUpdate);
            }

            if (customerUpdateBefore.BillToAddress != customerUpdateAfter.BillToAddress)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "BillToAddress";
                logUpdate.Old = customerUpdateBefore.BillToAddress;
                logUpdate.New = customerUpdateAfter.BillToAddress;
                listLogUpdate.Add(logUpdate);
            }

            if (customerUpdateBefore.ShipToAddress != customerUpdateAfter.ShipToAddress)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "ShipToAddress";
                logUpdate.Old = customerUpdateBefore.ShipToAddress;
                logUpdate.New = customerUpdateAfter.ShipToAddress;
                listLogUpdate.Add(logUpdate);
            }

            if (customerUpdateBefore.FuelSurchargeFee != customerUpdateAfter.FuelSurchargeFee)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "FuelSurchargeFee";
                logUpdate.Old = customerUpdateBefore.FuelSurchargeFee;
                logUpdate.New = customerUpdateAfter.FuelSurchargeFee;
                listLogUpdate.Add(logUpdate);
            }

            if (customerUpdateBefore.Note != customerUpdateAfter.Note)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "Note";
                logUpdate.Old = customerUpdateBefore.Note;
                logUpdate.New = customerUpdateAfter.Note;
                listLogUpdate.Add(logUpdate);
            }

            LogKey logKey = new LogKey { };
            logKey.Action = "Update";
            logKey.Data = listLogUpdate;
            var description = JsonConvert.SerializeObject(logKey);

            if (listLogUpdate.Count != 0)
            {
                orderLog.OrderId = customerUpdateAfter.Id;
                orderLog.UserId = userId;
                orderLog.IsTable = "Customer";
                orderLog.EventType = "Update Customer";
                orderLog.Description = description;
                orderLog.CreatedTime = DateTime.UtcNow;
                _context.OrderLog.Add(orderLog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task StoreRigLocationLogInformationPost(RigLocationView rigLocationView, RigLocation rigLocation, string method, string userId)
        {
            Log orderLog = new Log { };

            orderLog.OrderId = rigLocation.Id;
            orderLog.UserId = userId;
            orderLog.EventType = "Add RigLocation";
            orderLog.IsTable = "RigLocation";

            RigLocationLogView rigLocationLogView = new RigLocationLogView { };
            rigLocationLogView.Id = rigLocation.Id;
            rigLocationLogView.CustomerName = rigLocationView.CustomerId.Name;
            rigLocationLogView.Name = rigLocation.Name;
            rigLocationLogView.Note = rigLocation.Note;

            LogKey logKey = new LogKey { };
            logKey.Action = "Add";
            logKey.Data = rigLocationLogView;
            var description = JsonConvert.SerializeObject(logKey);

            orderLog.Description = description;
            orderLog.CreatedTime = DateTime.UtcNow;
            _context.OrderLog.Add(orderLog);
            await _context.SaveChangesAsync();
        }

        public async Task StoreRigLocationLogInformationPut(RigLocation rigLocationUpdateAfter, RigLocationView rigLocationUpdateAfterView, string method, string userId, RigLocation rigLocationUpdateBefore, RigLocationView rigLocationUpdateBeforeView)
        {
            Log orderLog = new Log();

            List<LogUpdate> listLogUpdate = new List<LogUpdate>();

            if (rigLocationUpdateBefore.CustomerId != rigLocationUpdateAfter.CustomerId)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "CustomerName";
                logUpdate.Old = rigLocationUpdateBeforeView.CustomerId.Name;
                logUpdate.New = rigLocationUpdateAfterView.CustomerId.Name;
                listLogUpdate.Add(logUpdate);
            }

            if (rigLocationUpdateBefore.Name != rigLocationUpdateAfter.Name)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "Name";
                logUpdate.Old = rigLocationUpdateBefore.Name;
                logUpdate.New = rigLocationUpdateAfter.Name;
                listLogUpdate.Add(logUpdate);
            }

            if (rigLocationUpdateBefore.Note != rigLocationUpdateAfter.Note)
            {
                LogUpdate logUpdate = new LogUpdate { };
                logUpdate.FieldName = "Note";
                logUpdate.Old = rigLocationUpdateBefore.Note;
                logUpdate.New = rigLocationUpdateAfter.Note;
                listLogUpdate.Add(logUpdate);
            }

            LogKey logKey = new LogKey { };
            logKey.Action = "Update";
            logKey.Data = listLogUpdate;

            var description = JsonConvert.SerializeObject(logKey);

            if (listLogUpdate.Count != 0)
            {
                orderLog.OrderId = rigLocationUpdateAfter.Id;
                orderLog.UserId = userId;
                orderLog.IsTable = "RigLocation";
                orderLog.EventType = "Update RigLocation";
                orderLog.Description = description;
                orderLog.CreatedTime = DateTime.UtcNow;
                _context.OrderLog.Add(orderLog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagingResults<Log>> GetOrderLogViewAsync(SieveModel sieveModel)
        {
            var query = (from appuser in _context.AppUser
                            join orderlog in _context.OrderLog
                            on appuser.AspNetUserId equals orderlog.UserId
                            where orderlog.IsTable == "Order"
                            select new Log
                            {
                                Id = orderlog.Id,
                                OrderId = orderlog.OrderId,
                                UserId = appuser.FullName,
                                EventType = orderlog.EventType,
                                Description = orderlog.Description,
                                CreatedTime = orderlog.CreatedTime
                            }).AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return data;
        }

        public async Task<PagingResults<Log>> GetTicketLogViewAsync(SieveModel sieveModel)
        {
            var query = (from appuser in _context.AppUser
                             join orderlog in _context.OrderLog
                             on appuser.AspNetUserId equals orderlog.UserId
                             where orderlog.IsTable == "Ticket"
                             select new Log
                             {
                                 Id = orderlog.Id,
                                 OrderId = orderlog.OrderId,
                                 UserId = appuser.FullName,
                                 EventType = orderlog.EventType,
                                 Description = orderlog.Description,
                                 CreatedTime = orderlog.CreatedTime
                             }).AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return data;
        }

        public async Task<PagingResults<Log>> GetCustomerLogViewAsync(SieveModel sieveModel)
        {
            var query = (from appuser in _context.AppUser
                               join orderlog in _context.OrderLog
                               on appuser.AspNetUserId equals orderlog.UserId
                               where orderlog.IsTable == "Customer"
                               select new Log
                               {
                                   Id = orderlog.Id,
                                   OrderId = orderlog.OrderId,
                                   UserId = appuser.FullName,
                                   EventType = orderlog.EventType,
                                   Description = orderlog.Description,
                                   CreatedTime = orderlog.CreatedTime
                               }).AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return data;
        }

        public async Task<PagingResults<Log>> GetRigLocationLogViewAsync(SieveModel sieveModel)
        {
            var query = (from appuser in _context.AppUser
                                  join orderlog in _context.OrderLog
                                  on appuser.AspNetUserId equals orderlog.UserId
                                  where orderlog.IsTable == "RigLocation"
                                  select new Log
                                  {
                                      Id = orderlog.Id,
                                      OrderId = orderlog.OrderId,
                                      UserId = appuser.FullName,
                                      EventType = orderlog.EventType,
                                      Description = orderlog.Description,
                                      CreatedTime = orderlog.CreatedTime
                                  }).AsQueryable();
            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return data;
        }
    }
}

