using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace ticketing_api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class DeployController : Controller
    {
        [HttpPost("")]
        public async Task<IActionResult> Receive()
        {
            Request.Headers.TryGetValue("X-GitHub-Event", out StringValues eventName);
            Request.Headers.TryGetValue("X-Hub-Signature", out StringValues signature);
            Request.Headers.TryGetValue("X-GitHub-Delivery", out StringValues delivery);

            using (var reader = new StreamReader(Request.Body))
            {
                var txt = await reader.ReadToEndAsync();

                //check and make sure branch is master.

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "deploy.bat",
                        Arguments = txt,
                        RedirectStandardOutput = false,
                        UseShellExecute = true,
                        CreateNoWindow = true,
                    }
                };
                process.Start();
            }

            return Ok("received");
        }
    }
}