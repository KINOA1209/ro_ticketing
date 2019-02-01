using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;

namespace ticketing_api.Services
{
    public class BillOfLadingImageService
    {
        private const string ImageDirectory = "Images\\BillOfLading\\image";

        private readonly ApplicationDbContext _context;
        private readonly ApplicationSieveProcessor _sieveProcessor;

        public BillOfLadingImageService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        //public async Task<PagingResults<TicketPaperView>> GetListViewAsync(SieveModel sieveModel)
        //{
        //    var query = _context.TicketPaper.Join(_context.Market,
        //        ticketPaper => ticketPaper.MarketId,
        //        market => market.Id,
        //        (ticketPaper, market) => new TicketPaperView
        //        {
        //            Id = ticketPaper.Id,
        //            Name = ticketPaper.Name,
        //            FormatId = ticketPaper.FormatId,
        //            Content = ticketPaper.Content,
        //            MarketId = market,
        //            IsEnabled = ticketPaper.IsEnabled,
        //            IsDeleted = ticketPaper.IsDeleted
        //        }).AsQueryable();

        //    var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);
        //    return data;
        //}

        public async Task<string> SaveImage(BillOfLading bol, IFormFile file)
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
            var filename = $"bol_{bol.Id}_{currentTime}{fileExt}";
            var filesubpath = $"{folder}\\{filename}";
            var path = Path.Combine(ImageDirectory, filesubpath);

            //Save image file details to the database.
            BillOfLadingImage bolImage = new BillOfLadingImage { };
            bolImage.BillOfLadingId = bol.Id;
            bolImage.FileName = filename;
            bolImage.FilePath = path;
            _context.BillOfLadingImage.Add(bolImage);

            if (bol.BolStatusId != AppConstants.OrderStatuses.Delivered)
            {
                bol.DeliveredDate = DateTime.Now;
                bol.BolStatusId = AppConstants.OrderStatuses.Delivered;
                _context.Entry(bol).State = EntityState.Modified;
            }

            bol.ImageExists = true;
            _context.Entry(bol).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            //Create physical file to given path
            using (var stream = new FileStream(path, FileMode.Create))
                await file.CopyToAsync(stream);

            //Returns empty string as no error.
            return string.Empty;
        }

        public async Task<string> SaveImages(BillOfLading bol, List<IFormFile> files)
        {
            var errorMsg = string.Empty;
            foreach (var file in files)
            {
                var message = await SaveImage(bol, file);
                if (!string.IsNullOrEmpty(message)) errorMsg = message;
            }

            //Returns empty string as no error.
            return errorMsg;
        }

        public List<BillOfLadingImage> GetBolImages(int id)
        {
            var list = _context.BillOfLadingImage.Where(o => o.BillOfLadingId == id).ToList();

            return list;
        }
    }
}

