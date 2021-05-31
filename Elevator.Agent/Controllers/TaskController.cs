using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Elevator.Agent.Models;
using Elevator.Agent.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elevator.Agent.Controllers
{
    [ApiController]
    [Route("Task")]
    public class TaskController: Controller
    {
        private readonly TaskService taskService;

        public TaskController(TaskService taskService)
        {
            this.taskService = taskService;
        }

        [HttpPost("push")]
        public async Task<VoidOperationResult> PushAsync([FromBody] BuildTask buildTask)
        {
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
