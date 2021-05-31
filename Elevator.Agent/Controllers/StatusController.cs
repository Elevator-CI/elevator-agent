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
        private readonly TaskService taskService;

        public StatusController(StatusService statusService, TaskService taskService)
        {
            this.statusService = statusService;
            this.taskService = taskService;
        }

        public async Task<OperationResult<Status>> GetStatusAsync()
        {
            return OperationResult<Status>.Success(statusService.Status);
        }

        [HttpPost("free")]
        public async Task<OperationResult<BuildTaskResult>> FreeAsync()
        {
            if (statusService.Status != Status.Finished)
                return OperationResult<BuildTaskResult>.Failed("Agent is not in finished state");
            statusService.Status = Status.Free;
            return OperationResult<BuildTaskResult>.Success(taskService.BuildTaskResult);
        }
    }
}
#pragma warning restore 1998