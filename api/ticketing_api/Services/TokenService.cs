using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ticketing_api.Models;
using ticketing_api.Models.Views;

namespace ticketing_api.Services
{
    public class TokenService
    {
        private static TokenService _instance;
        public static TokenService Instance => _instance ?? (_instance = new TokenService());

        private string HtmlReplace(string html, string token, string data)
        {
            return Regex.Replace(html, token, data.Replace("$0", "$$0"), RegexOptions.IgnoreCase);
        }

        public string ReplaceUserTokens(AppUser appUser, string html)
        {
            html = HtmlReplace(html, @"\[CREATION EMAIL\]", appUser.Email ?? string.Empty);
            html = HtmlReplace(html, @"\[FIRST NAME\]", appUser.FirstName ?? string.Empty);
            html = HtmlReplace(html, @"\[LAST NAME\]", appUser.LastName ?? string.Empty);

            return html;
        }

        public string ReplaceOrderTokens(OrderView order, string html)
        {
            html = HtmlReplace(html, @"\[MM/DD/YYYY\]", DateTime.Now.ToString("MM/dd/yyyy"));

            html = Regex.Replace(html, "\\[ORDER NUM\\]", order.Id.ToString(), RegexOptions.IgnoreCase);
            html = HtmlReplace(html, @"\[TICKET NUM\]", order.Id.ToString());

            html = Regex.Replace(html, "\\[CUSTOMER\\]", order.CustomerId?.Name ?? string.Empty, RegexOptions.IgnoreCase);

            html = Regex.Replace(html, "\\[WELL\\]", order.WellName ?? string.Empty, RegexOptions.IgnoreCase);
            html = HtmlReplace(html, @"\[DRIVER\]", order.DriverId?.FullName ?? string.Empty);

            html = HtmlReplace(html, @"\[SALES REP\]", order.SalesRepId?.FullName ?? string.Empty);

            html = HtmlReplace(html, @"\[SALES REP EMAIL\]", order.SalesRepId?.Email ?? string.Empty);

            html = HtmlReplace(html, @"\[TRUCK\]", order.TruckId?.Name ?? string.Empty);
            html = Regex.Replace(html, "\\[MARKET\\]", order.MarketId?.Name ?? string.Empty, RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "\\[COUNTY\\]", order.CountyId?.Name ?? string.Empty, RegexOptions.IgnoreCase);

            html = HtmlReplace(html, @"\[RIG\]", order.RigLocationId?.Name ?? string.Empty);
            html = HtmlReplace(html, @"\[RIG/LOCATION\]", order.RigLocationId?.Name ?? string.Empty);
            html = HtmlReplace(html, @"\[RIG LOCATION\]", order.RigLocationId?.Name ?? string.Empty);

            html = HtmlReplace(html, @"\[JOB TYPE\]", order.JobTypeId?.Name ?? string.Empty);

            html = HtmlReplace(html, @"\[WELL CODE\]", order.WellCode ?? string.Empty);

            html = HtmlReplace(html, @"\[AFE/PO\]", order.AFEPO ?? string.Empty);
            html = HtmlReplace(html, @"\[AFE PO\]", order.AFEPO ?? string.Empty);

            html = HtmlReplace(html, @"\[CONTACT NAME\]", order.PointOfContactName ?? string.Empty);
            html = HtmlReplace(html, @"\[CONTACT PHONE\]", order.PointOfContactNumber ?? string.Empty);
            html = HtmlReplace(html, @"\[CONTACT EMAIL\]", order.PointOfContactEmail ?? string.Empty);
            if (order.OrderDate > DateTime.MinValue) html = HtmlReplace(html, @"\[ORDER DATE\]", order.OrderDate.ToString("MM/dd/yyyy"));
            if (order.RequestDate > DateTime.MinValue) html = HtmlReplace(html, @"\[REQUESTED DATE\]", order.RequestDate.ToString("MM/dd/yyyy"));
            html = HtmlReplace(html, @"\[REQUESTED TIME\]", order.RequestTime ?? string.Empty);
            html = HtmlReplace(html, @"\[REQUESTED TIME NLT\]", order.RequestTime ?? string.Empty);

            html = HtmlReplace(html, @"\[REQUESTED DELIVERY TIME SLOT\]", order.RequestDeliveryTimeSlotId?.Name ?? string.Empty);

            var deliveredDate = order.DeliveredDate > DateTime.MinValue && order.DeliveredDate.Year > 2000 ? order.DeliveredDate.ToString("MM/dd/yyyy") : DateTime.Now.ToString("MM/dd/yyyy");
            html = HtmlReplace(html, @"\[DELIVERED DATE\]", deliveredDate);


            html = HtmlReplace(html, @"\[ORDER DESCRIPTION\]", order.OrderDescription ?? string.Empty);
            html = HtmlReplace(html, @"\[DIRECTIONS\]", order.WellDirection ?? string.Empty);
            html = HtmlReplace(html, @"\[CUSTOMER NOTES\]", order.CustomerNotes ?? string.Empty);
            html = HtmlReplace(html, @"\[INTERNAL NOTES\]", order.InternalNotes ?? string.Empty);
            html = HtmlReplace(html, @"\[FORT BERTHOLD\]", order.SpecialHandling == "ND-FB" ? "FORT BERTHOLD" : "");

            return html;
        }
    }
}
