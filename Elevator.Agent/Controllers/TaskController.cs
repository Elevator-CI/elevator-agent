using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Elevator.Agent.Models;
using Elevator.Agent.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Elevator.Agent.Controllers
{
    [ApiController]
    [Route("task")]
    public class TaskController: Controller
    {
        private readonly TaskService taskService;
        private readonly ILogger<TaskController> logger;

        public TaskController(TaskService taskService, ILogger<TaskController> logger)
        {
            this.taskService = taskService;
            this.logger = logger;
        }

        [HttpPost("push")]
        public async Task<VoidOperationResult> PushAsync([FromBody] BuildTask buildTask)
        {
            logger.LogInformation("Pushing task...");
            await taskService.StartTask(buildTask);
            return VoidOperationResult.Success();
        }

#pragma warning disable 1998
        [HttpGet("pull")]
        public async Task<OperationResult<BuildTaskResult>> PullAsync()
        {
            return OperationResult<BuildTaskResult>.Success(taskService.BuildTaskResult);
        }
#pragma warning restore 1998

    }
}
