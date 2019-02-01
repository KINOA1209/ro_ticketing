using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ticketing_api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class StatusController : Controller
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            //TODO: check database connection is good.

            return Ok();
        }
    }
}
