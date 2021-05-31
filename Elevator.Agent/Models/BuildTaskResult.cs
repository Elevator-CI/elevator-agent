using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;

namespace Elevator.Agent.Models
{
    public class BuildTaskResult
    {
        public ImmutableList<string> Logs { get; set; }
    }
}
