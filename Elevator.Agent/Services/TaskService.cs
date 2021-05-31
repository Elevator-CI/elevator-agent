using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elevator.Agent.Models;
using Git;
using Microsoft.Extensions.Logging;
using Shell;
using TaskStatus = Elevator.Agent.Models.TaskStatus;

namespace Elevator.Agent.Services
{
    public class TaskService
    {
        public BuildTaskResult BuildTaskResult { get; private set; }
        public BuildTask Task { get; private set; }

        private readonly StatusService statusService;

        public TaskService(StatusService statusService)
        {
            this.statusService = statusService;
        }

        public async Task StartTask(BuildTask task)
        {
            if (statusService.Status != Status.Free)
                throw new InvalidOperationException("Agent is not free");

            statusService.Status = Status.Working;

            Task = task;
            BuildTaskResult = new BuildTaskResult
            {
                Logs = ImmutableList<string>.Empty,
                Status = TaskStatus.InProgress
            };
        }

        
    }
}
