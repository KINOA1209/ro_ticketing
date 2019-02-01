using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SelectPdf;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;
using ticketing_api.Models.Views;
using OrderView = ticketing_api.Models.Views.OrderView;

namespace ticketing_api.Services
{
    public class TicketPaperService
    {
        private const string ImageDirectory = "Images\\TicketPaper\\image";

        private readonly ApplicationDbContext _context;
        private readonly ApplicationSieveProcessor _sieveProcessor;

        public TicketPaperService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        public async Task<PagingResults<TicketPaperView>> GetListViewAsync(SieveModel sieveModel)
        {
            var query = _context.TicketPaper.Join(_context.Market,
                ticketPaper => ticketPaper.MarketId,
                market => market.Id,
                (ticketPaper, market) => new TicketPaperView
                {
                    Id = ticketPaper.Id,
                    Name = ticketPaper.Name,
                    FormatId = ticketPaper.FormatId,
                    Content = ticketPaper.Content,
                    MarketId = market,
                    IsEnabled = ticketPaper.IsEnabled,
                    IsDeleted = ticketPaper.IsDeleted
                }).AsQueryable();

            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
            return data;
        }

        private string HtmlReplace(string html, string token, string data)
        {
            return Regex.Replace(html, token, data.Replace("$0", "$$0"), RegexOptions.IgnoreCase);
        }

        public string GeneratePdf(OrderView ticketView, decimal startingGallons, TicketPaper ticketPaper, out string filename, string paperSize = "A6")
        {
            paperSize = paperSize ?? "A6";
            var folder = DateTime.Now.Date.AddDays(-1).ToString("yyyyMMdd");
            var currentTime = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff");
            filename = $"ticketpaper_{ticketView.Id}_{currentTime}.pdf";
            var filepath = $"Images/TicketPaper/pdf/{folder}/{filename}";
            var html = ticketPaper.Content;

            html = HtmlReplace(html, @"\[STARTING GALLONS\]", startingGallons.ToString("N0"));
            html = TokenService.Instance.ReplaceOrderTokens(ticketView, html);

            //html = HtmlReplace(html, @"\[TICKET NUM\]", ticketView.Id.ToString());
            //html = HtmlReplace(html, @"\[MM/DD/YYYY\]", DateTime.Now.ToString("MM/dd/yyyy"));
            //html = HtmlReplace(html, @"\[CUSTOMER\]", ticketView.CustomerId?.Name ?? string.Empty);
            //html = HtmlReplace(html, @"\[RIG\]", ticketView.RigLocationId?.Name ?? string.Empty);
            //html = HtmlReplace(html, @"\[RIG/LOCATION\]", ticketView.RigLocationId?.Name ?? string.Empty);
            //html = HtmlReplace(html, @"\[WELL\]", ticketView.WellName ?? string.Empty);
            //html = HtmlReplace(html, @"\[MARKET\]", ticketView.MarketId.Name ?? string.Empty);
            //html = HtmlReplace(html, @"\[WELL CODE\]", ticketView.WellCode ?? string.Empty);
            //html = HtmlReplace(html, @"\[DRIVER\]", ticketView.DriverId?.FullName ?? string.Empty);
            //html = HtmlReplace(html, @"\[TRUCK\]", ticketView.TruckId?.Name ?? string.Empty);
            //html = HtmlReplace(html, @"\[AFE/PO\]", ticketView.AFEPO ?? string.Empty);
            //html = HtmlReplace(html, @"\[CONTACT NAME\]", ticketView.PointOfContactName ?? string.Empty);
            //html = HtmlReplace(html, @"\[CONTACT PHONE\]", ticketView.PointOfContactNumber ?? string.Empty);
            //if (ticketView.OrderDate > DateTime.MinValue)
            //    html = HtmlReplace(html, @"\[ORDER DATE\]", ticketView.OrderDate.ToString("MM/dd/yyyy"));
            //if (ticketView.RequestDate > DateTime.MinValue)
            //    html = HtmlReplace(html, @"\[REQUESTED DATE\]", ticketView.RequestDate.ToString("MM/dd/yyyy"));
            //html = HtmlReplace(html, @"\[REQUESTED TIME\]", ticketView.RequestTime ?? string.Empty);

            //var deliveredDate = ticketView.DeliveredDate > DateTime.MinValue && ticketView.DeliveredDate.Year > 2000 ? ticketView.DeliveredDate.ToString("MM/dd/yyyy") : DateTime.Now.ToString("MM/dd/yyyy");
            //html = HtmlReplace(html, @"\[DELIVERED DATE\]", deliveredDate);


            //html = HtmlReplace(html, @"\[ORDER DESCRIPTION\]", ticketView.OrderDescription ?? string.Empty);
            //html = HtmlReplace(html, @"\[CUSTOMER NOTES\]", ticketView.CustomerNotes ?? string.Empty);
            //html = HtmlReplace(html, @"\[FORT BERTHOLD\]", ticketView.SpecialHandling == "ND-FB" ? "FORT BERTHOLD" : "");

            var productTotal = 0.0M;
            var taxTotal = 0.0M;

            var ticketService = new TicketService(_context, _sieveProcessor);
            var products = ticketService.GetTicketProducts(ticketView.Id).ToList();
            if (products.Any())
            {
                var headers = new[] { "Product", "Unit", "Quantity", "Price", "Total" };
                var productsHtml = "<table  class='product-table' style='width:100%'><thead align='left'><tr>";
                foreach (var header in headers)
                    productsHtml += "<th>" + header + "</th>";

                productsHtml += "</tr></thead><tbody>";
                foreach (var p in products)
                {
                    productsHtml += $"<tr class='product-row'><td class='product-name'>{p.ProductId.Name}</td><td class='product-unit'>{p.UnitId.Name}</td><td class='product-quantity'>{p.Quantity.ToString("N2")}</td><td  class='product-price'>{p.Price.ToString("N5")}</td><td  class='product-total'>{@"$" + (p.Quantity * p.Price).ToString("N5")}</td></tr>";
                    productTotal += p.Quantity * p.Price;
                }
                productsHtml += "</tbody></table>";
                html = HtmlReplace(html, @"\[PRODUCTS\]", productsHtml);
            }

            html = HtmlReplace(html, @"\[PRODUCTS\]", "");

            var ticketTax = _context.TicketTax.Where(x => x.TicketId == ticketView.Id && x.IsEnabled).OrderBy(x => x.TaxId).AsNoTracking().ToList();

            if (ticketTax.Any())
            {
                var headers = new[] { "Description", "Type", "Total" };
                var taxHtml = "<table  class='tax-table' style='width:100%'><thead align='left'><tr>";
                foreach (var header in headers) taxHtml += "<th>" + header + "</th>";
                taxHtml += "</tr></thead><tbody>";
                foreach (var t in ticketTax)
                {
                    taxHtml += $"<tr><td class='tax-description'>{t.TaxDescription}</td><td class='tax-type'>{t.TaxType.ToUpper()}</td><td  class='tax-amount'>{@"$" + t.TaxAmount.ToString("N5")}</td></tr>";
                    taxTotal += t.TaxAmount;
                }
                taxHtml += "</tbody></table>";
                html = HtmlReplace(html, @"\[TAXES\]", taxHtml);
            }

            html = HtmlReplace(html, @"\[TAXES\]", "");

            var productTotalLabel = @"$" + (productTotal).ToString("N2");
            html = HtmlReplace(html, @"\[TOTAL\]", productTotalLabel);
            html = HtmlReplace(html, @"\[TAX TOTAL\]", @"$" + (taxTotal).ToString("N2"));
            html = HtmlReplace(html, @"\[GRAND TOTAL\]", @"$" + (productTotal + taxTotal).ToString("N2"));

            HtmlToPdf converter = new HtmlToPdf();
            try
            {
                Enum.TryParse(paperSize, out PdfPageSize newSize);
                converter.Options.PdfPageSize = newSize;

                if (paperSize.ToUpper() == "LETTER")
                {
                    converter.Options.MarginLeft = 12;
                    converter.Options.MarginRight = 12;
                }
            }
            catch (Exception ex)
            {
                converter.Options.PdfPageSize = PdfPageSize.A6;
            }

            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.ShrinkOnly;
            converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.NoAdjustment;

            PdfDocument doc = converter.ConvertHtmlString(html);
            doc.Save(filepath);
            doc.Close();

            return filepath;
        }

        public async Task<string> SaveImage(Order ticket, IFormFile file)
        {
            string[] validFileExtensions = { ".png", ".jpeg", ".jpg" };

            var fileExt = Path.GetExtension(file.FileName).ToLower();

            var isValidExt = validFileExtensions.Contains(fileExt);
            if (!isValidExt)
                return "Invalid file type";  // return and skip insertion if any invalid file found.

            var todayDate = DateTime.Now; //Get current date;
            var paperTime = _context.Setting.FirstOrDefault(s => s.Key == "PaperTime");
            var timesetting = paperTime?.Value ?? "11pm";
            var todayHours = todayDate.ToString("HH");
            var todayMinute = todayDate.ToString("mm");
            var settingHour = Convert.ToInt64(timesetting.Remove(timesetting.Length - 2));
            var settingHourOld = settingHour;
            var settingReg = timesetting.Substring(timesetting.Length - 2, 2);

            settingHour = settingReg.ToLower() == "pm" ? settingHour + 12 : settingHour;

            var fromDate = todayDate;
            var toDate = DateTime.Now.AddDays(+1);
            if (!((Convert.ToInt64(todayHours) < settingHour) || ((Convert.ToInt64(todayHours) == settingHour) && (Convert.ToInt64(todayMinute) < 1))))
            {
                fromDate = toDate;
                toDate = DateTime.Now.AddDays(+2);
            }

            var folder = fromDate.ToString("yyyyMMdd") + "_" + settingHourOld + settingReg.ToUpper() + "_to_" + toDate.ToString("yyyyMMdd")
                         + "_" + settingHourOld + settingReg.ToUpper();
            var currentTime = fromDate.ToString("yyyyMMdd-HHmmss-fff");
            var filename = $"ticketpaper_{ticket.Id}_{currentTime}{fileExt}";
            var filesubpath = $"{folder}\\{filename}";
            var path = Path.Combine(ImageDirectory, filesubpath);

            //Save image file details to the database.
            TicketPaperImage paperImage = new TicketPaperImage { };
            paperImage.TicketId = ticket.Id;
            paperImage.FileName = filename;
            paperImage.FilePath = path;
            _context.TicketPaperImage.Add(paperImage);

            if (ticket.OrderStatusId != AppConstants.OrderStatuses.Delivered)
            {
                ticket.DeliveredDate = DateTime.Now;
                ticket.OrderStatusId = AppConstants.OrderStatuses.Delivered;
                _context.Entry(ticket).State = EntityState.Modified;
            }

            ticket.TicketImageExists = true;
            _context.Entry(ticket).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            //Create physical file to given path
            using (var stream = new FileStream(path, FileMode.Create))
                await file.CopyToAsync(stream);

            //Returns empty string as no error.
            return string.Empty;
        }

        public async Task<string> SaveImages(Order ticket, List<IFormFile> files)
        {
            var errorMsg = string.Empty;
            foreach (var file in files)
            {
                var message = await SaveImage(ticket, file);
                if (!string.IsNullOrEmpty(message)) errorMsg = message;
            }

            //Returns empty string as no error.
            return errorMsg;
        }

        public List<TicketPaperImage> GetTicketPaperImages(int id)
        {
            var list = _context.TicketPaperImage.Where(o => o.TicketId == id).ToList();

            return list;
        }
    }
}

