//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Sieve.Services;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Swashbuckle.AspNetCore.Filters;
//using ticketing_api.Data;
//using ticketing_api.Infrastructure;
//using ticketing_api.Models;
//using ticketing_api.Services;

//namespace ticketing_api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ScanQueueController : BaseController.BaseController
//    {
//        private readonly ILogger<ScanQueueController> _logger;
//        private readonly IHostingEnvironment _environment;
//        private readonly TicketService _ticketService;

//        public ScanQueueController(ApplicationDbContext context, 
//            ILogger<ScanQueueController> logger, 
//            IEmailSender emailSender,
//            ISieveProcessor sieveProcessor, 
//            IHostingEnvironment environment) : base(context, emailSender, sieveProcessor)
//        {
//            _logger = logger;
//            _environment = environment;
//            _ticketService = new TicketService(_context, sieveProcessor);
//        }

//        // GET: api/TicketImages
//        [HttpPost("ScanQueue")]
//        [AddSwaggerFileUploadButton]
//        public async Task<IActionResult> PostTicketImageScan(IFormFile file)
//        {
//            TicketImage ticketImage = new TicketImage();
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            //start file upload to call Ticketservice
//            if(file != null)
//            {
//                var fileName = _ticketService.PostTicketScan(file);
//                var userId = _context.AppUser.Where(c => c.Email == User.FindFirst(ClaimTypes.Email).Value).Select(c => c.AspNetUserId).Single();
//                ticketImage.UserId = userId;
//                ticketImage.FileName = fileName;

//                _context.TicketImage.Add(ticketImage);
//                await _context.SaveChangesAsync();
//                //End file upload successfully

//                //Start OCR to ticket image match
//                OcrScan ocrScan = new OcrScan();
//                var ticketNo = ocrScan.ImageScan(file, _environment);

//                var resultTicketMatch = from x in _context.Order
//                                        where x.Id == ticketNo
//                                        select new { x };

//                int countTicketRecordMatch = resultTicketMatch.Count();
//                if (countTicketRecordMatch != 0)
//                {
//                    var order = await _context.Order.FindAsync(ticketNo);
//                    //order.TicketImg = fileName;
//                    _context.Entry(order).State = EntityState.Modified;
//                    await _context.SaveChangesAsync();
//                    ticketImage = await _context.TicketImage.FindAsync(ticketImage.Id);
//                    ticketImage.IsMatch = true;
//                    _context.Entry(ticketImage).State = EntityState.Modified;
//                    await _context.SaveChangesAsync();
//                    return Ok("File Upload Successfully and Ticket Record are Match");
//                }
//                else
//                {
//                    ticketImage = await _context.TicketImage.FindAsync(ticketImage.Id);
//                    ticketImage.IsMatch = false;
//                    _context.Entry(ticketImage).State = EntityState.Modified;
//                    await _context.SaveChangesAsync();
//                    return Ok("File Upload Successfully and Ticket Record does not match");
//                }
//                //End OCR to ticket image match
//            }
//            else
//            {
//                return BadRequest("File are not selected");
//            }
//        }

//        // GET: api/TicketImages/ScanQueue
//        [HttpGet("ScanQueue")]
//        public async Task<IActionResult> GetTicketImageScanQueue()
//        {
//            var url = $"{Request.Scheme}://{Request.Host}";
//            var imagePath = $"{url}/Images/TicketImage/";

//            var ticket = await(from ticketimage in _context.TicketImage
//                                where ticketimage.IsMatch == false
//                                select new { ticketimage.Id, ticketimg = imagePath + ticketimage.FileName, ticketimage.UserId, ticketimage.IsMatch, ticketimage.IsEnabled }).ToListAsync();
//            return Ok(ticket);
//        }
//    }
//}
