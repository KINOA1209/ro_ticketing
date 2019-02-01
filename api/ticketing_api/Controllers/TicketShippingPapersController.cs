using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Filters;
using ticketing_api.Services;

namespace ticketing_api.Controllers
{
    public partial class TicketsController : BaseController.BaseController
    {
        /// <summary>
        /// Generate shipping paper pdf for ticket and return pdf in response
        /// </summary>
        /// <param name="id">ticket id</param>
        /// <param name="startingGallons"></param>
        /// <param name="paperSize"></param>
        /// <returns></returns>
        [HttpGet("{id}/shippingpaper")]
        public IActionResult GetShippingPaper([FromRoute] int id, [FromQuery]decimal startingGallons, [FromQuery]string paperSize)
        {
            var ticketView = _orderService.GetOrderView(id);

            if (ticketView == null)
                return BadRequest("Ticket does not exist.");

            var shippingPaper = _context.ShippingPaper.FirstOrDefault(x=>x.MarketId == ticketView.MarketId.Id);

            if (shippingPaper == null)
                return BadRequest("Shipping paper does not exist for this market.");

            var shippingPaperService = new ShippingPaperService(_context, _sieveProcessor);

            var filepath = shippingPaperService.GeneratePdf(ticketView, startingGallons, shippingPaper, out string filename, paperSize);

            var file = System.IO.File.ReadAllBytes(filepath);
            return File(file, "application/pdf", filename);
        }

        /// <summary>
        /// Generate shipping paper pdf for ticket and return path in response
        /// </summary>
        /// <param name="id">ticket id</param>
        /// <param name="startingGallons"></param>
        /// <param name="paperSize"></param>
        /// <returns></returns>
        [HttpGet("{id}/shippingpaperpath")]
        public IActionResult GetShippingPaperUrl([FromRoute] int id, [FromQuery]decimal startingGallons, [FromQuery]string paperSize)
        {
            var ticketView = _orderService.GetOrderView(id);

            if (ticketView == null)
                return BadRequest("Ticket does not exist.");

            var shippingPaper = _context.ShippingPaper.FirstOrDefault(x => x.MarketId == ticketView.MarketId.Id);

            if (shippingPaper == null)
                return BadRequest("Shipping paper does not exist.");

            var shippingPaperService = new ShippingPaperService(_context, _sieveProcessor);

            var filepath = shippingPaperService.GeneratePdf(ticketView, startingGallons, shippingPaper, out string filename, paperSize);
            return Ok(new { id, path = filepath });
        }



        /// <summary>
        /// Get image of printed shipping paper
        /// </summary>
        /// <param name="id">ticket id</param>
        /// <returns></returns>
        [HttpGet("{id}/shippingpaperimage")]
        public IActionResult GetShippingPaperImage([FromRoute] int id)
        {
            var shippingPaperService = new ShippingPaperService(_context, _sieveProcessor);
            var listImages = shippingPaperService.GetShippingPaperImages(id);
            if (listImages == null || listImages.Count == 0)
                return Content("No images available.");

            var url = $"{Request.Scheme}://{Request.Host}";
                listImages.ForEach(a => a.FilePath = $"{url}/{a.FilePath.Replace("\\", "/")}");

                return Ok(listImages);
        }


        /// <summary>
        /// post image of printed shipping paper
        /// </summary>
        /// <param name="id">ticket id</param>
        /// <param name="file">image file</param>
        /// <returns></returns>
        [HttpPost("{id}/shippingpaperimage")]
        [AddSwaggerFileUploadButton]
        public async Task<IActionResult> PostShippingPaperImageAsync([FromRoute] int id, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (file == null) return BadRequest("File not supplied");
            var shippingPaperService = new ShippingPaperService(_context, _sieveProcessor);
            var message = await shippingPaperService.SaveImage(id, file);

            if (!string.IsNullOrEmpty(message)) return BadRequest(message);

            return Content("Image Uploaded Successfully");
        }

        ///// <summary>
        ///// post multiple image of printed shipping paper
        ///// </summary>
        ///// <param name="id">ticket id</param>
        ///// <param name="formCollection"></param>
        ///// <returns></returns>
        //[HttpPost("{id}/shippingpaperimages")]
        //public async Task<IActionResult> PostShippingPaperImagesAsync([FromRoute] int id, IFormCollection formCollection)
        //{
        //    const string keyName = "files";
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    List<IFormFile> files = formCollection?.Files.Where(f => f.Name == keyName).ToList();
        //    if (files == null || files.Count < 0) return BadRequest("File not supplied");
        //    var shippingPaperService = new ShippingPaperService(_context, _sieveProcessor);
        //    var message = await shippingPaperService.SaveImages(id, files);

        //    if (!string.IsNullOrEmpty(message)) return BadRequest(message);

        //    return Content("Image Uploaded Successfully");
        //}
    }
}