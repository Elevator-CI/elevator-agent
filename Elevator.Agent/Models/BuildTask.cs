using System;
using System.Collections.Generic;

namespace Elevator.Agent.Models
{ 
    public class BuildTask
    {
        public Guid Id { get; set; }
        public Guid BuildId { get; set; }
        public IList<BuildCommand> Commands { get; set; }
        public string ProjectUrl { get; set; }
        public string GitToken { get; set; }
    }
}