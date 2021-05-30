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
    }
}
