using System;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Tesseract;

namespace ticketing_api.Services
{
    public class OcrScan
    {
        public int ImageScan(IFormFile file, IHostingEnvironment environment)
        {
            var ticketNo = 0;

            //using (var engine = new TesseractEngine(Path.Combine(environment.ContentRootPath, "Tessdata"), "eng", EngineMode.Default))
            //{
            //    using (var image = new Bitmap(file.OpenReadStream()))
            //    {
            //        using (var pix = PixConverter.ToPix(image))
            //        {
            //            using (var page = engine.Process(image))
            //            {
            //                var ticketText = page.GetText();
            //                string startPositionString = "mm";
            //                if (ticketText.Contains(startPositionString))
            //                {
            //                    int startPosition = ticketText.IndexOf(startPositionString);
            //                    string remainString = ticketText.Substring(startPosition);
            //                    int endPosition = remainString.IndexOf("\n") - 4;
            //                    string ticket = remainString.Substring(endPosition, 4);
            //                    if (startPosition > 0 && endPosition > 0)
            //                    {
            //                        ticketNo = Convert.ToInt32(ticket);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            return ticketNo;
        }
    }
}
