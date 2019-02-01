using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SelectPdf;
using Sieve.Models;
using Sieve.Services;
using ticketing_api.Data;
using ticketing_api.Models;
using ticketing_api.Models.Views;
using OrderView = ticketing_api.Models.Views.OrderView;

namespace ticketing_api.Services
{
    public class ShippingPaperService
    {
        private const string ImageDirectory = "Images\\ShippingPaper\\image";

        private readonly ApplicationDbContext _context;
        private readonly ApplicationSieveProcessor _sieveProcessor;

        public ShippingPaperService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        public async Task<PagingResults<ShippingPaperView>> GetListViewAsync(SieveModel sieveModel)
        {
            var query = _context.ShippingPaper.Join(_context.Market,
                shippingPaper => shippingPaper.MarketId,
                market => market.Id,
                (shippingPaper, market) => new ShippingPaperView
                {
                    Id = shippingPaper.Id,
                    Name = shippingPaper.Name,
                    Content = shippingPaper.Content,
                    MarketId = market,
                    IsEnabled = shippingPaper.IsEnabled,
                    IsDeleted = shippingPaper.IsDeleted
                }).AsQueryable();

            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return data;
        }

        public string GeneratePdf(OrderView ticketView, decimal startingGallons, ShippingPaper shippingPaper, out string filename, string paperSize = "A6")
        {

            paperSize = paperSize ?? "A6";
            var folder = DateTime.Now.Date.AddDays(-1).ToString("yyyyMMdd");
            var currentTime = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff");
            filename = $"shippingpaper_{ticketView.Id}_{currentTime}.pdf";
            var filepath = $"Images/ShippingPaper/pdf/{folder}/{filename}";

            var html = shippingPaper.Content;
            html = TokenService.Instance.ReplaceOrderTokens(ticketView, html);

            //html = Regex.Replace(html, "\\[STARTING GALLONS\\]", startingGallons.ToString("N0"), RegexOptions.IgnoreCase);
            //html = Regex.Replace(html, "\\[MM/DD/YYYY\\]", DateTime.Now.ToString("MM/dd/yyyy"), RegexOptions.IgnoreCase);
            //html = Regex.Replace(html, "\\[CUSTOMER\\]", ticketView.CustomerId?.Name ?? string.Empty, RegexOptions.IgnoreCase);
            //html = Regex.Replace(html, "\\[RIG\\]", ticketView.RigLocationId?.Name ?? string.Empty, RegexOptions.IgnoreCase);
            //html = Regex.Replace(html, "\\[WELL\\]", ticketView.WellName ?? string.Empty, RegexOptions.IgnoreCase);
            //html = Regex.Replace(html, "\\[DRIVER\\]", ticketView.DriverId?.FullName ?? string.Empty, RegexOptions.IgnoreCase);
            //html = Regex.Replace(html, "\\[TRUCK\\]", ticketView.TruckId?.Name ?? string.Empty, RegexOptions.IgnoreCase);
            //html = Regex.Replace(html, "\\[MARKET\\]", ticketView.MarketId?.Name ?? string.Empty, RegexOptions.IgnoreCase);

            HtmlToPdf converter = new HtmlToPdf();
            try
            {
                Enum.TryParse(paperSize, out PdfPageSize myPdfNewSize);
                converter.Options.PdfPageSize = myPdfNewSize;
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

        public async Task<string> SaveImage(int id, IFormFile file)
        {
            var ticket = await _context.Order.FindAsync(id);
            if (ticket == null) return "Ticket not found.";

            //Valid extension list for images
            string[] validFileExtensions = { ".png", ".jpeg", ".jpg" };

            var fileExt = Path.GetExtension(file.FileName).ToLower();

            //Check for valid extension
            var isValidExt = validFileExtensions.Contains(fileExt);
            if (!isValidExt)
                return "Invalid file type";  // return and skip insertion if any invalid file found.

            var todayDate = DateTime.Now; //Get current date;
            //to fetch the setting paper image time
            var shippingPaperTime = _context.Setting.FirstOrDefault(s => s.Key == "PaperTime");
            var timesetting = shippingPaperTime?.Value ?? "11pm";
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
            var filename = $"shippingpaper_{id}_{currentTime}{fileExt}";
            var filesubpath = $"{folder}\\{filename}";
            var path = Path.Combine(ImageDirectory, filesubpath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            //Create physical file to given path
            using (var stream = new FileStream(path, FileMode.Create))
                await file.CopyToAsync(stream);

            //Save image file details to the database.
            ShippingPaperImage shippingPaperImage = new ShippingPaperImage { };
            shippingPaperImage.TicketId = id;
            shippingPaperImage.FileName = filename;
            shippingPaperImage.FilePath = path;
            _context.ShippingPaperImage.Add(shippingPaperImage);

            ticket.ShippingPaperExists = true;
            _context.Entry(ticket).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            //Returns empty string as no error.
            return string.Empty;
        }

        public async Task<string> SaveImages(int id, List<IFormFile> files)
        {
            var errorMsg = string.Empty;
            foreach (var file in files)
            {
                var message = await SaveImage(id, file);
                if (!string.IsNullOrEmpty(message)) errorMsg = message;
            }

            //Returns empty string as no error.
            return errorMsg;
        }

        public List<ShippingPaperImage> GetShippingPaperImages(int id)
        {
            var list = _context.ShippingPaperImage
                               .Where(o => o.TicketId == id)
                               .ToList();

            return list;
        }
    }
}

