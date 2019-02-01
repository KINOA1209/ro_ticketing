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
    public partial class BillOfLadingsController : BaseController.BaseController
    {
        /// <summary>
        /// Get image of bol
        /// </summary>
        /// <param name="id">bol Id / po number</param>
        /// <returns></returns>
        [HttpGet("{id}/image")]
        public IActionResult GetBolImage([FromRoute] int id)
        {
            var bolService = new BillOfLadingImageService(_context, _sieveProcessor);
            var listImages = bolService.GetBolImages(id);
            if (listImages == null || listImages.Count == 0)
                return Content("No images available.");

            var url = $"{Request.Scheme}://{Request.Host}";

            listImages.ForEach(a => a.FilePath = $"{url}/{a.FilePath.Replace("\\", "/")}");

            return Ok(listImages);
        }

        /// <summary>
        /// post image of bol
        /// </summary>
        /// <param name="id">bol Id / po number</param>
        /// <param name="file">image file</param>
        /// <returns></returns>
        [HttpPost("{id}/image")]
        [AddSwaggerFileUploadButton]
        public async Task<IActionResult> PostBolImageAsync([FromRoute] int id, IFormFile file)
        {
            if (file == null) return BadRequest("File not supplied");

            var bol = await _context.BillOfLading.FindAsync(id);
            var errorMsg = await _billOfLadingService.CheckBillOfLadingAccessAsync(bol, AppUser);
            if (errorMsg != null) return BadRequest(errorMsg);

            var imageService = new BillOfLadingImageService(_context, _sieveProcessor);
            var message = await imageService.SaveImage(bol, file);

            if (!string.IsNullOrEmpty(message)) return BadRequest(message);

            return Content("Image uploaded successfully");
        }
    }
}