using Microsoft.AspNetCore.Identity;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ticketing_api.Models;
using ticketing_api.Models.Views;

namespace ticketing_api.Services
{
    public class SieveCustomSortMethods : ISieveCustomSortMethods
    {
        #region Sorting OrderView Field
        public IQueryable<OrderView> SortId(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.Id) : source.OrderBy(o => o.Id);
        }

        public IQueryable<OrderView> SortOrderDate(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.OrderDate) : source.OrderBy(o => o.OrderDate);
        }

        public IQueryable<OrderView> SortRequestDate(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.RequestDate) : source.OrderBy(o => o.RequestDate);
        }

        public IQueryable<OrderView> SortDeliveredDate(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.DeliveredDate) : source.OrderBy(o => o.DeliveredDate);
        }

        public IQueryable<OrderView> SortRequestTime(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.RequestTime.PadLeft(5, '0')) : source.OrderBy(o => o.RequestTime.PadLeft(5, '0'));
        }

        public IQueryable<OrderView> SortJobTypeId(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.JobTypeId.Name.ToUpper()) : source.OrderBy(o => o.JobTypeId.Name.ToUpper());
        }

        public IQueryable<OrderView> SortCustomerId(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.CustomerId.Name.ToUpper()) : source.OrderBy(o => o.CustomerId.Name.ToUpper());
        }

        public IQueryable<OrderView> SortCustomerNotes(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.CustomerId.Note.ToUpper()) : source.OrderBy(o => o.CustomerId.Note.ToUpper());
        }

        public IQueryable<OrderView> SortRigLocationId(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.RigLocationId.Name.ToUpper()) : source.OrderBy(o => o.RigLocationId.Name.ToUpper());
        }

        public IQueryable<OrderView> SortRigLocationNotes(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.RigLocationId.Note.ToUpper()) : source.OrderBy(o => o.RigLocationId.Note.ToUpper());
        }

        public IQueryable<OrderView> SortWellId(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.WellId.Name.ToUpper()) : source.OrderBy(o => o.WellId.Name.ToUpper());
        }

        public IQueryable<OrderView> SortWellName(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.WellName.ToUpper()) : source.OrderBy(o => o.WellName.ToUpper());
        }

        public IQueryable<OrderView> SortOrderStatusId(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.OrderStatusId.Name.ToUpper()) : source.OrderBy(o => o.OrderStatusId.Name.ToUpper());
        }

        public IQueryable<OrderView> SortWellCode(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.WellCode.ToUpper()) : source.OrderBy(o => o.WellCode.ToUpper());
        }

        public IQueryable<OrderView> SortDriverId(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
                return desc ? source.OrderByDescending(o => o.DriverId.FullName.ToUpper()) : source.OrderBy(o => o.DriverId.FullName.ToUpper());
        }

        public IQueryable<OrderView> SortTruckId(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.TruckId.Name.ToUpper()) : source.OrderBy(o => o.TruckId.Name.ToUpper());
        }

        public IQueryable<OrderView> SortAfepo(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.AFEPO.ToUpper()) : source.OrderBy(o => o.AFEPO.ToUpper());
        }

        public IQueryable<OrderView> SortPointOfContactName(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.PointOfContactName.ToUpper()) : source.OrderBy(o => o.PointOfContactName.ToUpper());
        }

        public IQueryable<OrderView> SortPointOfContactNumber(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.PointOfContactNumber.ToUpper()) : source.OrderBy(o => o.PointOfContactNumber.ToUpper());
        }

        public IQueryable<OrderView> SortMarketId(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(l => l.MarketId.Name.ToUpper()) : source.OrderBy(l => l.MarketId.Name.ToUpper());
        }

        public IQueryable<OrderView> SortSalesRepId(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.SalesRepId.LastName ?? "") : source.OrderBy(o => o.SalesRepId.LastName ?? "");
        }

        public IQueryable<OrderView> SortOrderDescription(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.OrderDescription.ToUpper()) : source.OrderBy(o => o.OrderDescription.ToUpper());
        }

        public IQueryable<OrderView> SortWellDirection(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.WellDirection.ToUpper()) : source.OrderBy(o => o.WellDirection.ToUpper());
        }

        public IQueryable<OrderView> SortInternalNotes(IQueryable<OrderView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(o => o.InternalNotes.ToUpper()) : source.OrderBy(o => o.InternalNotes.ToUpper());
        }
        #endregion

        #region Sorting TicketPaperView Field

        public IQueryable<TicketPaperView> SortName(IQueryable<TicketPaperView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(tp => tp.Name.ToUpper()) : source.OrderBy(tp => tp.Name.ToUpper());
        }

        public IQueryable<TicketPaperView> SortContent(IQueryable<TicketPaperView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(tp => tp.Content.ToUpper()) : source.OrderBy(tp => tp.Content.ToUpper());
        }

        public IQueryable<TicketPaperView> SortMarketId(IQueryable<TicketPaperView> source, bool useThenBy, bool desc)
        {
            if (desc) return source.OrderByDescending(tp => tp.MarketId.Name.ToUpper());
            return source.OrderBy(tp => tp.MarketId.Name.ToUpper());
        }
        #endregion

        #region Sorting ShippingPaperView Field

        public IQueryable<ShippingPaperView> SortName(IQueryable<ShippingPaperView> source, bool useThenBy, bool desc)
        {
            if (desc) return source.OrderByDescending(tp => tp.Name.ToUpper());
            return source.OrderBy(tp => tp.Name.ToUpper());
        }

        public IQueryable<ShippingPaperView> SortContent(IQueryable<ShippingPaperView> source, bool useThenBy, bool desc)
        {
            if (desc) return source.OrderByDescending(tp => tp.Content.ToUpper());
            return source.OrderBy(tp => tp.Content.ToUpper());
        }

        public IQueryable<ShippingPaperView> SortMarketId(IQueryable<ShippingPaperView> source, bool useThenBy, bool desc)
        {
            if (desc) return source.OrderByDescending(tp => tp.MarketId.Name.ToUpper());
            return source.OrderBy(tp => tp.MarketId.Name.ToUpper());
        }

        #endregion

        #region Sorting WellView Field
        public IQueryable<WellView> SortName(IQueryable<WellView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(w => w.Name.ToUpper()) : source.OrderBy(w => w.Name.ToUpper());
        }

        public IQueryable<WellView> SortRigLocationId(IQueryable<WellView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(w => w.RigLocationId.Name.ToUpper()) : source.OrderBy(w => w.RigLocationId.Name.ToUpper());
        }

        public IQueryable<WellView> SortDirection(IQueryable<WellView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(w => w.Direction.ToUpper()) : source.OrderBy(w => w.Direction.ToUpper());
        }

        public IQueryable<WellView> SortIsVisible(IQueryable<WellView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(w => w.IsVisible) : source.OrderBy(w => w.IsVisible);
        }
        #endregion

        #region Sorting Category Field

        public IQueryable<ProductCategory> SortName(IQueryable<ProductCategory> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(c => c.Name.ToUpper()) : source.OrderBy(c => c.Name.ToUpper());
        }

        public IQueryable<ProductCategory> SortIsVisible(IQueryable<ProductCategory> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(c => c.IsVisible) : source.OrderBy(c => c.IsVisible);
        }

        #endregion

        #region Sorting JobType Field

        public IQueryable<JobType> SortName(IQueryable<JobType> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(j => j.Name.ToUpper()) : source.OrderBy(j => j.Name.ToUpper());
        }

        public IQueryable<JobType> SortIsVisible(IQueryable<JobType> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(j => j.IsVisible) : source.OrderBy(j => j.IsVisible);
        }

        #endregion

        #region Sorting Setting Field

        public IQueryable<Setting> SortName(IQueryable<Setting> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(s => s.Key.ToUpper()) : source.OrderBy(s => s.Key.ToUpper());
        }

        public IQueryable<Setting> SortValue(IQueryable<Setting> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(s => s.Value.ToUpper()) : source.OrderBy(s => s.Value.ToUpper());
        }

        #endregion

        #region Sorting Module Field

        public IQueryable<Module> SortModuleName(IQueryable<Module> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(m => m.ModuleName.ToUpper()) : source.OrderBy(m => m.ModuleName.ToUpper());
        }

        public IQueryable<Module> SortNormalizedName(IQueryable<Module> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(m => m.NormalizedName.ToUpper()) : source.OrderBy(m => m.NormalizedName.ToUpper());
        }

        #endregion

        #region Sorting Role Field

        public IQueryable<IdentityRole> SortRoleName(IQueryable<IdentityRole> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(r => r.Name.ToUpper()) : source.OrderBy(r => r.Name.ToUpper());
        }

        public IQueryable<IdentityRole> SortNormalizedName(IQueryable<IdentityRole> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(r => r.NormalizedName.ToUpper()) : source.OrderBy(r => r.NormalizedName.ToUpper());
        }

        #endregion

        #region Sorting Unit Field

        public IQueryable<Unit> SortName(IQueryable<Unit> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(u => u.Name.ToUpper()) : source.OrderBy(u => u.Name.ToUpper());
        }

        public IQueryable<Unit> SortIsVisible(IQueryable<Unit> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(u => u.IsVisible) : source.OrderBy(u => u.Name);
        }

        #endregion

        #region Sorting RigLocation Field

        public IQueryable<RigLocationView> SortName(IQueryable<RigLocationView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(rig => rig.Name.ToUpper()) : source.OrderBy(rig => rig.Name.ToUpper());
        }

        public IQueryable<RigLocationView> SortCustomerId(IQueryable<RigLocationView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(rig => rig.CustomerId.Name.ToUpper()) : source.OrderBy(rig => rig.CustomerId.Name.ToUpper());
        }

        public IQueryable<RigLocationView> SortNote(IQueryable<RigLocationView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(rig => rig.Note.ToUpper()) : source.OrderBy(rig => rig.Note.ToUpper());
        }

        public IQueryable<RigLocationView> SortIsVisible(IQueryable<RigLocationView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(rig => rig.IsVisible) : source.OrderBy(rig => rig.IsVisible);
        }

        #endregion

        #region Sorting ProductView Field

        public IQueryable<ProductView> SortName(IQueryable<ProductView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(p => p.Name.ToUpper()) : source.OrderBy(p => p.Name.ToUpper());
        }

        public IQueryable<ProductView> SortProductCategoryId(IQueryable<ProductView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(p => p.ProductCategoryId.Name.ToUpper()) : source.OrderBy(p => p.ProductCategoryId.Name.ToUpper());
        }

        public IQueryable<ProductView> SortUnitId(IQueryable<ProductView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(p => p.UnitId.Name.ToUpper()) : source.OrderBy(p => p.UnitId.Name.ToUpper());
        }

        public IQueryable<ProductView> SortUnitCost(IQueryable<ProductView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(p => p.UnitCost) : source.OrderBy(p => p.UnitCost);
        }

        public IQueryable<ProductView> SortUnitPrice(IQueryable<ProductView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(p => p.UnitPrice) : source.OrderBy(p => p.UnitPrice);
        }

        #endregion

        #region Sorting Truck Field

        public IQueryable<Truck> SortName(IQueryable<Truck> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(t => t.Name.ToUpper()) : source.OrderBy(t => t.Name.ToUpper());
        }

        public IQueryable<Truck> SortIsVisible(IQueryable<Truck> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(t => t.IsVisible) : source.OrderBy(t => t.Name);
        }

        #endregion

        #region Sorting Market Field

        public IQueryable<Market> SortName(IQueryable<Market> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(m => m.Name.ToUpper()) : source.OrderBy(m => m.Name.ToUpper());
        }

        public IQueryable<Market> SortDescription(IQueryable<Market> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(m => m.Description.ToUpper()) : source.OrderBy(m => m.Description.ToUpper());
        }

        public IQueryable<Market> SortIsVisible(IQueryable<Market> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(m => m.IsVisible) : source.OrderBy(m => m.IsVisible);
        }

        #endregion

        #region Sorting PaymentTerm Field

        public IQueryable<PaymentTerm> SortName(IQueryable<PaymentTerm> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(payment => payment.Name.ToUpper()) : source.OrderBy(payment => payment.Name.ToUpper());
        }

        public IQueryable<PaymentTerm> SortIsVisible(IQueryable<PaymentTerm> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(payment => payment.IsVisible) : source.OrderBy(payment => payment.IsVisible);
        }

        #endregion

        #region Sorting Customer Field

        public IQueryable<Customer> SortName(IQueryable<Customer> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(customer => customer.Name.ToUpper()) : source.OrderBy(customer => customer.Name.ToUpper());
        }

        public IQueryable<Customer> SortContactName(IQueryable<Customer> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(customer => customer.ContactName.ToUpper()) : source.OrderBy(customer => customer.ContactName.ToUpper());
        }

        public IQueryable<Customer> SortEmail(IQueryable<Customer> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(customer => customer.Email.ToUpper()) : source.OrderBy(customer => customer.Email.ToUpper());
        }

        public IQueryable<Customer> SortContactPhone(IQueryable<Customer> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(customer => customer.ContactPhone) : source.OrderBy(customer => customer.ContactPhone);
        }

        public IQueryable<Customer> SortFaxNumber(IQueryable<Customer> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(customer => customer.FaxNumber.ToUpper()) : source.OrderBy(customer => customer.FaxNumber.ToUpper());
        }

        public IQueryable<Customer> SortBillToAddress(IQueryable<Customer> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(customer => customer.BillToAddress.ToUpper()) : source.OrderBy(customer => customer.BillToAddress.ToUpper());
        }

        public IQueryable<Customer> SortShipToAddress(IQueryable<Customer> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(customer => customer.ShipToAddress.ToUpper()) : source.OrderBy(customer => customer.ShipToAddress.ToUpper());
        }

        public IQueryable<Customer> SortPaymentTermId(IQueryable<Customer> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(customer => customer.PaymentTermId) : source.OrderBy(customer => customer.PaymentTermId);
        }

        public IQueryable<Customer> SortFuelSurchargeFee(IQueryable<Customer> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(customer => customer.FuelSurchargeFee) : source.OrderBy(customer => customer.FuelSurchargeFee);
        }

        public IQueryable<Customer> SortNote(IQueryable<Customer> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(customer => customer.Note.ToUpper()) : source.OrderBy(customer => customer.Note.ToUpper());
        }

        public IQueryable<Customer> SortIsVisible(IQueryable<Customer> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(customer => customer.IsVisible) : source.OrderBy(customer => customer.IsVisible);
        }
        #endregion

        #region Sorting TaxView Field

        public IQueryable<TaxView> SortName(IQueryable<TaxView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(tax => tax.Name.ToUpper()) : source.OrderBy(tax => tax.Name.ToUpper());
        }

        public IQueryable<TaxView> SortMarketId(IQueryable<TaxView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(tax => tax.MarketId.Name.ToUpper()) : source.OrderBy(tax => tax.MarketId.Name.ToUpper());
        }

        public IQueryable<TaxView> SortTaxType(IQueryable<TaxView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(tax => tax.TaxType.ToUpper()) : source.OrderBy(tax => tax.TaxType.ToUpper());
        }

        public IQueryable<TaxView> SortTaxValue(IQueryable<TaxView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(tax => tax.TaxValue) : source.OrderBy(tax => tax.TaxValue);
        }

        #endregion

        #region Sorting User Field

        public IQueryable<AppUser> SortFirstName(IQueryable<AppUser> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(appuser => appuser.FirstName.ToUpper()) : source.OrderBy(appuser => appuser.FirstName.ToUpper());
        }

        public IQueryable<AppUser> SortMiddleName(IQueryable<AppUser> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(appuser => appuser.MiddleName.ToUpper()) : source.OrderBy(appuser => appuser.MiddleName.ToUpper());
        }

        public IQueryable<AppUser> SortLastName(IQueryable<AppUser> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(appuser => appuser.LastName.ToUpper()) : source.OrderBy(appuser => appuser.LastName.ToUpper());
        }

        public IQueryable<AppUser> SortPhoneNumber(IQueryable<AppUser> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(appuser => appuser.PhoneNumber.ToUpper()) : source.OrderBy(appuser => appuser.PhoneNumber.ToUpper());
        }

        public IQueryable<AppUser> SortPosition(IQueryable<AppUser> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(appuser => appuser.Position.ToUpper()) : source.OrderBy(appuser => appuser.Position.ToUpper());
        }

        public IQueryable<AppUser> SortEmployeeCode(IQueryable<AppUser> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(appuser => appuser.EmployeeCode.ToUpper()) : source.OrderBy(appuser => appuser.EmployeeCode.ToUpper());
        }

        public IQueryable<AppUser> SortEmail(IQueryable<AppUser> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(appuser => appuser.Email.ToUpper()) : source.OrderBy(appuser => appuser.Email.ToUpper());
        }

        public IQueryable<AppUser> SortUsername(IQueryable<AppUser> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(appuser => appuser.Username.ToUpper()) : source.OrderBy(appuser => appuser.Username.ToUpper());
        }

        public IQueryable<AppUser> SortRole(IQueryable<AppUser> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(appuser => appuser.Role.ToUpper()) : source.OrderBy(appuser => appuser.Role.ToUpper());
        }

        public IQueryable<AppUser> SortEffectiveDate(IQueryable<AppUser> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(appuser => appuser.EffectiveDate) : source.OrderBy(appuser => appuser.EffectiveDate);
        }
        #endregion

        #region Sorting SalesRepView Field

        public IQueryable<SalesRepView> SortFirstName(IQueryable<SalesRepView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(salesrep => salesrep.FirstName.ToUpper()) : source.OrderBy(salesrep => salesrep.FirstName.ToUpper());
        }

        public IQueryable<SalesRepView> SortMiddleName(IQueryable<SalesRepView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(salesrep => salesrep.MiddleName.ToUpper()) : source.OrderBy(salesrep => salesrep.MiddleName.ToUpper());
        }

        public IQueryable<SalesRepView> SortLastName(IQueryable<SalesRepView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(salesrep => salesrep.LastName.ToUpper()) : source.OrderBy(salesrep => salesrep.LastName.ToUpper());
        }

        public IQueryable<SalesRepView> SortPhoneNumber(IQueryable<SalesRepView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(salesrep => salesrep.PhoneNumber.ToUpper()) : source.OrderBy(salesrep => salesrep.PhoneNumber.ToUpper());
        }

        public IQueryable<SalesRepView> SortPosition(IQueryable<SalesRepView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(salesrep => salesrep.Position.ToUpper()) : source.OrderBy(salesrep => salesrep.Position.ToUpper());
        }

        public IQueryable<SalesRepView> SortEmployeeCode(IQueryable<SalesRepView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(salesrep => salesrep.EmployeeCode.ToUpper()) : source.OrderBy(salesrep => salesrep.EmployeeCode.ToUpper());
        }

        public IQueryable<SalesRepView> SortEmail(IQueryable<SalesRepView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(salesrep => salesrep.Email.ToUpper()) : source.OrderBy(salesrep => salesrep.Email.ToUpper());
        }

        public IQueryable<SalesRepView> SortUsername(IQueryable<SalesRepView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(salesrep => salesrep.Username.ToUpper()) : source.OrderBy(salesrep => salesrep.Username.ToUpper());
        }

        public IQueryable<SalesRepView> SortRole(IQueryable<SalesRepView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(salesrep => salesrep.Role.ToUpper()) : source.OrderBy(salesrep => salesrep.Role.ToUpper());
        }

        public IQueryable<SalesRepView> SortEffectiveDate(IQueryable<SalesRepView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(salesrep => salesrep.EffectiveDate) : source.OrderBy(salesrep => salesrep.EffectiveDate);
        }

        public IQueryable<SalesRepView> SortNotes(IQueryable<SalesRepView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(salesrep => salesrep.Notes.ToUpper()) : source.OrderBy(salesrep => salesrep.Notes.ToUpper());
        }

        public IQueryable<SalesRepView> SortIsVisible(IQueryable<SalesRepView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(salesrep => salesrep.IsVisible) : source.OrderBy(salesrep => salesrep.IsVisible);
        }
        #endregion

        #region Sorting DriverView Field

        public IQueryable<DriverView> SortFirstName(IQueryable<DriverView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(driver => driver.FirstName.ToUpper()) : source.OrderBy(driver => driver.FirstName.ToUpper());
        }

        public IQueryable<DriverView> SortMiddleName(IQueryable<DriverView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(driver => driver.MiddleName.ToUpper()) : source.OrderBy(driver => driver.MiddleName.ToUpper());
        }

        public IQueryable<DriverView> SortLastName(IQueryable<DriverView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(driver => driver.LastName.ToUpper()) : source.OrderBy(driver => driver.LastName.ToUpper());
        }

        public IQueryable<DriverView> SortPhoneNumber(IQueryable<DriverView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(driver => driver.PhoneNumber.ToUpper()) : source.OrderBy(driver => driver.PhoneNumber.ToUpper());
        }

        public IQueryable<DriverView> SortPosition(IQueryable<DriverView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(driver => driver.Position.ToUpper()) : source.OrderBy(driver => driver.Position.ToUpper());
        }

        public IQueryable<DriverView> SortEmployeeCode(IQueryable<DriverView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(driver => driver.EmployeeCode.ToUpper()) : source.OrderBy(driver => driver.EmployeeCode.ToUpper());
        }

        public IQueryable<DriverView> SortEmail(IQueryable<DriverView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(driver => driver.Email.ToUpper()) : source.OrderBy(driver => driver.Email.ToUpper());
        }

        public IQueryable<DriverView> SortUsername(IQueryable<DriverView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(driver => driver.Username.ToUpper()) : source.OrderBy(driver => driver.Username.ToUpper());
        }

        public IQueryable<DriverView> SortRole(IQueryable<DriverView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(driver => driver.Role.ToUpper()) : source.OrderBy(driver => driver.Role.ToUpper());
        }

        public IQueryable<DriverView> SortEffectiveDate(IQueryable<DriverView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(driver => driver.EffectiveDate) : source.OrderBy(driver => driver.EffectiveDate);
        }

        public IQueryable<DriverView> SortIsVisible(IQueryable<DriverView> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(driver => driver.IsVisible) : source.OrderBy(driver => driver.IsVisible);
        }
        #endregion

        #region Sorting LoadOrigin Field

        public IQueryable<LoadOrigin> SortName(IQueryable<LoadOrigin> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(loadOrigin => loadOrigin.Name.ToUpper()) : source.OrderBy(loadOrigin => loadOrigin.Name.ToUpper());
        }

        public IQueryable<LoadOrigin> SortDescription(IQueryable<LoadOrigin> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(loadOrigin => loadOrigin.Description.ToUpper()) : source.OrderBy(loadOrigin => loadOrigin.Description.ToUpper());
        }

        public IQueryable<LoadOrigin> SortIsVisible(IQueryable<LoadOrigin> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(loadOrigin => loadOrigin.IsVisible) : source.OrderBy(loadOrigin => loadOrigin.IsVisible);
        }
        #endregion

        #region Sorting MarketSpecialHandling Field

        public IQueryable<MarketSpecialHandling> SortMarketName(IQueryable<MarketSpecialHandling> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(ms => ms.MarketName.ToUpper()) : source.OrderBy(ms => ms.MarketName.ToUpper());
        }

        public IQueryable<MarketSpecialHandling> SortDescription(IQueryable<MarketSpecialHandling> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(ms => ms.Description.ToUpper()) : source.OrderBy(ms => ms.Description.ToUpper());
        }

        public IQueryable<MarketSpecialHandling> SortSpecialHandlingCode(IQueryable<MarketSpecialHandling> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(ms => ms.SpecialHandlingCode.ToUpper()) : source.OrderBy(ms => ms.SpecialHandlingCode.ToUpper());
        }

        public IQueryable<MarketSpecialHandling> SortIsVisible(IQueryable<MarketSpecialHandling> source, bool useThenBy, bool desc)
        {
            return desc ? source.OrderByDescending(ms => ms.IsVisible) : source.OrderBy(ms => ms.IsVisible);
        }
        #endregion
    }
}
