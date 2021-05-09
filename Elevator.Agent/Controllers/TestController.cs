using System;
using Microsoft.AspNetCore.Mvc;

namespace Elevator.Agent.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController: Controller
    {
        [HttpGet]
        public ActionResult<string> GetTestAsync()
        {
            return Ok(Guid.NewGuid().ToString());
        }
    }
}
