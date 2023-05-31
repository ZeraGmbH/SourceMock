using Microsoft.AspNetCore.Mvc;
using SourceMock.Model;

namespace SourceMock.Controllers
{

    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SourceController : Controller
    {
        [HttpPost("Set")]
        public IActionResult SetValues([FromBody] Loadpoint loadpoint)
        {
            return Ok();
        }

        [HttpPost("TurnOn")]
        public IActionResult TurnOn()
        {
            return Ok();
        }

        [HttpPost("TurnOff")]
        public IActionResult TurnOff()
        {
            return Ok();
        }
    }
}
