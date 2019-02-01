using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;

namespace ticketing_api.Services
{
    public class TicketTaxService
    {
        private readonly ApplicationDbContext _context;
        private readonly TicketService _ticketService;

        public TicketTaxService(ApplicationDbContext context, TicketService ticketService)
        {
            _context = context;
            _ticketService = ticketService;
        }

        public List<TicketTax> UpdateTaxForTicket(int ticketId)
        {
            var ticket = _context.Order.Find(ticketId);
            //if delivered or voided then taxes are locked down
            if (ticket.OrderStatusId == AppConstants.OrderStatuses.Delivered ||
                ticket.OrderStatusId == AppConstants.OrderStatuses.Voided)
            {
                return _context.TicketTax.Where(x => x.TicketId == ticketId && x.IsEnabled && x.TaxAmount != 0.0M).AsNoTracking().ToList();
            }

            TicketTax ticketSurcharge = null;
            var jobTypeName = string.Empty;
            if (ticket.JobTypeId > 0)
            {
                var jobType = _context.JobType.Find(ticket.JobTypeId);
                jobTypeName = jobType.Name.ToUpper();
            }

            //check customer surcharges
            if (ticket.CustomerId > 0 && !jobTypeName.Contains("FRAC") && !ticket.FuelSurchargeNA)
            {
                var customer = _context.Customer.FirstOrDefault(x => x.Id == ticket.CustomerId);
                if (customer != null && customer.FuelSurchargeFee != 0.0M)
                {
                    ticketSurcharge = new TicketTax
                    {
                        TicketId = ticket.Id,
                        TaxDescription = "FUEL SURCHARGE",
                        TaxType = "FIXED",
                        TaxId = 0,
                        IsEnabled = true,
                        TaxValue = customer.FuelSurchargeFee,
                        TaxAmount = customer.FuelSurchargeFee
                    };
                }
            }

            var existingSurcharge = _context.TicketTax.Where(x => x.TicketId == ticketId && x.TaxId == 0).AsNoTracking().FirstOrDefault();
            if (existingSurcharge != null)
            {
                if (ticketSurcharge == null)
                {
                    existingSurcharge.IsEnabled = false;
                }
                else
                {
                    existingSurcharge.IsEnabled = true;
                    existingSurcharge.TaxValue = ticketSurcharge.TaxValue;
                    existingSurcharge.TaxDescription = ticketSurcharge.TaxDescription;
                    existingSurcharge.TaxType = ticketSurcharge.TaxType;
                    existingSurcharge.TaxAmount = ticketSurcharge.TaxAmount;
                }

                _context.Entry(existingSurcharge).State = EntityState.Modified;
            }
            else if (ticketSurcharge != null)
            {
                _context.TicketTax.Add(ticketSurcharge);
            }

            //now check for product and category taxes
            var products = _ticketService.GetTicketProducts(ticketId).ToList();
            var newTaxes = new Dictionary<int, TicketTax>();

            foreach (var product in products)
            {
                var taxApplied = new List<int>();

                var productTaxes = (from p in _context.ProductTax
                                    join tax in _context.Tax on p.TaxId equals tax.Id
                                    where p.ProductId == product.ProductId.Id && tax.MarketId == ticket.MarketId
                                    select tax).AsNoTracking().ToList();

                var categoryTaxes = (from p in _context.CategoryTax
                                     join tax in _context.Tax on p.TaxId equals tax.Id
                                     where p.CategoryId == product.ProductId.ProductCategoryId && tax.MarketId == ticket.MarketId
                                     select tax).AsNoTracking().ToList();

                productTaxes.AddRange(categoryTaxes);
                foreach (var tax in productTaxes)
                {
                    if (taxApplied.Contains(tax.Id)) continue;
                    taxApplied.Append(tax.Id);

                    decimal taxAmount = 0.0M;

                    switch (tax.TaxType)
                    {
                        case "percentage":
                            taxAmount = (product.Price * product.Quantity) * (tax.TaxValue / 100.0M);
                            break;
                        case "per unit":
                            taxAmount = product.Quantity * tax.TaxValue;
                            break;
                    }

                    var ticketTax = new TicketTax
                    {
                        TicketId = ticket.Id,
                        TaxDescription = tax.Name,
                        TaxType = tax.TaxType,
                        TaxId = tax.Id,
                        IsEnabled = true,
                        TaxValue = tax.TaxValue,
                        TaxAmount = taxAmount
                    };

                    if (newTaxes.ContainsKey(tax.Id))
                    {
                        ticketTax.TaxAmount += newTaxes[tax.Id].TaxAmount;
                    }

                    newTaxes[tax.Id] = ticketTax;
                }
            }

            var tempNewTaxes = newTaxes.Select(x => x.Value).ToList();
            var saveNewTaxes = new List<TicketTax>();
            if (ticketSurcharge != null)
                saveNewTaxes.Add(ticketSurcharge);

            foreach (var tax in tempNewTaxes)
            {
                if (tax.TaxAmount != 0.0M)
                    saveNewTaxes.Add(tax);
            }

            //mark all the existing ticketTaxes is disabled
            var existingTaxes = _context.TicketTax.Where(x => x.TicketId == ticketId && x.TaxId > 0).AsNoTracking().ToList();

            foreach (var existing in existingTaxes)
            {
                TicketTax newTax = null;
                if (newTaxes.ContainsKey(existing.TaxId))
                {
                    newTax = newTaxes[existing.TaxId];
                }

                if (newTax != null)
                {
                    existing.IsEnabled = true;
                    existing.TaxValue = newTax.TaxValue;
                    existing.TaxDescription = newTax.TaxDescription;
                    existing.TaxType = newTax.TaxType;
                    existing.TaxAmount = newTax.TaxAmount;
                    newTaxes.Remove(existing.TaxId);

                    if (existing.TaxAmount == 0.0M)
                        existing.IsEnabled = false;
                }
                else
                {
                    existing.IsEnabled = false;
                }

                _context.Entry(existing).State = EntityState.Modified;
            }

            foreach (var key in newTaxes.Keys)
            {
                if(newTaxes[key].TaxAmount != 0.0M)
                    _context.TicketTax.Add(newTaxes[key]);
            }

            _context.SaveChanges();

            return saveNewTaxes;
        }
    }
}

