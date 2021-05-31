using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Elevator.Agent.Models;
using Elevator.Agent.Services;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable 1998
namespace Elevator.Agent.Controllers
{
    [ApiController]
    [Route("/status")]
    public class StatusController: Controller
    {
        private readonly StatusService statusService;

        public StatusController(StatusService statusService)
        {
            this.statusService = statusService;
        }

        public async Task<OperationResult<Status>> GetStatusAsync()
        {
            return OperationResult<Status>.Success(statusService.Status);
        }

        [HttpPost("free")]
        public async Task<VoidOperationResult> FreeAsync()
        {
            if (statusService.Status != Status.Finished)
                return VoidOperationResult.Failed("Agent is not in finished state");
            statusService.Status = Status.Free;
            return VoidOperationResult.Success();
        }
    }
}
#pragma warning restore 1998