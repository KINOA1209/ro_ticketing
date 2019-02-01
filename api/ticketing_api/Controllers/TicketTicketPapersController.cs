using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using ticketing_api.Infrastructure;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    public partial class TicketsController : BaseController.BaseController
    {
        private string GenerateTicketPaper(int id, decimal startingGallons, string paperSize, out string message, out string filename)
        {
            message = null;
            filename = null;

            var ticketView = _orderService.GetOrderView(id);

            if (ticketView == null)
            {
                message = "Ticket does not exists";
                return null;
            }

            if (ticketView.MarketId == null)
            {
                message = "Ticket has no assigned market";
                return null;
            }

            var formatId = 1;

            if(paperSize!=null && paperSize.ToUpper() == "LETTER") formatId = 2;

            var ticketPaper = _context.TicketPaper.FirstOrDefault(x => x.MarketId == ticketView.MarketId.Id && x.FormatId == formatId);
            if (ticketPaper == null && formatId == 2)
                ticketPaper = _context.TicketPaper.FirstOrDefault(x => x.MarketId == ticketView.MarketId.Id && x.FormatId == 1);

            if (ticketPaper == null)
            {
                message = "Ticket paper does not exists";
                return null;
            }

            var ticketTaxService = new TicketTaxService(_context, _ticketService);
            var taxes = ticketTaxService.UpdateTaxForTicket(id);
            var ticketPaperService = new TicketPaperService(_context, _sieveProcessor);

            var filepath = ticketPaperService.GeneratePdf(ticketView, startingGallons, ticketPaper, out filename, paperSize);

            return filepath;
        }

        /// <summary>
        /// Generate pdf and return the pdf data as the response
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startingGallons"></param>
        /// <param name="paperSize"></param>
        /// <returns></returns>
        [HttpGet("{id}/ticketpaper")]
        public IActionResult GetTicketPaper([FromRoute] int id, [FromQuery]decimal startingGallons, [FromQuery]string paperSize)
        {
            var filepath = GenerateTicketPaper(id, startingGallons, paperSize, out string message, out string filename);
            if(message!=null)
                return BadRequest(message);

            var file = System.IO.File.ReadAllBytes(filepath);
            return File(file, "application/pdf", filename);
        }

        /// <summary>
        /// Generate pdf and return link to access the pdf
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startingGallons"></param>
        /// <param name="paperSize"></param>
        /// <returns></returns>
        [HttpGet("{id}/ticketpaperpath")]
        public IActionResult GetTicketPaperUrl([FromRoute] int id, [FromQuery]decimal startingGallons, [FromQuery]string paperSize)
        {
            var filepath = GenerateTicketPaper(id, startingGallons, paperSize, out string message, out string filename);
            if (message != null)
                return BadRequest(message);

            return Ok(new { id, path = filepath });
        }

        /// <summary>
        /// Get image of printed ticket paper
        /// </summary>
        /// <param name="id">ticket id</param>
        /// <returns></returns>
        [HttpGet("{id}/ticketpaperimage")]
        public IActionResult GetTicketPaperImage([FromRoute] int id)
        {
            var ticketPaperService = new TicketPaperService(_context, _sieveProcessor);
            var listImages = ticketPaperService.GetTicketPaperImages(id);
            if (listImages == null || listImages.Count == 0)
                return Content("No images available.");

            var url = $"{Request.Scheme}://{Request.Host}";

            listImages.ForEach(a => a.FilePath = $"{url}/{a.FilePath.Replace("\\", "/")}");

            return Ok(listImages);
        }

        /// <summary>
        /// post image of printed ticket paper
        /// </summary>
        /// <param name="id">ticket id</param>
        /// <param name="file">image file</param>
        /// <returns></returns>
        [HttpPost("{id}/ticketpaperimage")]
        [AddSwaggerFileUploadButton]
        public async Task<IActionResult> PostTicketPaperImageAsync([FromRoute] int id, IFormFile file)
        {
            if (file == null) return BadRequest("File not supplied");

            var ticket = await _context.Order.FindAsync(id);
            var errorMsg = await _ticketService.CheckTicketAccessAsync(ticket, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            var paperService = new TicketPaperService(_context, _sieveProcessor);
            var message = await paperService.SaveImage(ticket, file);

            if (!string.IsNullOrEmpty(message)) return BadRequest(message);

            return Content("Image Uploaded Successfully");
        }

        ///// <summary>
        ///// post multiple image of printed ticket paper
        ///// </summary>
        ///// <param name="id">ticket id</param>
        ///// <param name="formCollection"></param>
        ///// <returns></returns>
        //[HttpPost("{id}/ticketpaperimages")]
        //public async Task<IActionResult> PostTicketPaperImagesAsync([FromRoute] int id, IFormCollection formCollection)
        //{
        //    const string keyName = "files";
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    List<IFormFile> files = formCollection?.Files.Where(f => f.Name == keyName).ToList();
        //    if (files == null || files.Count < 0) return BadRequest("File not supplied");
        //    var paperService = new TicketPaperService(_context, _sieveProcessor);
        //    var message = await paperService.SaveImages(id, files);

        //    if (!string.IsNullOrEmpty(message)) return BadRequest(message);

        //    return Content("Image Uploaded Successfully");
        //}
    }
}