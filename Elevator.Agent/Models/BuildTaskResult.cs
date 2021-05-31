using System.Collections.Immutable;

namespace Elevator.Agent.Models
{
    public class BuildTaskResult
    {
        public ImmutableList<string> Logs { get; set; }

        public TaskStatus Status { get; set; }
    }

    public enum TaskStatus
    {
        InProgress,
        Success,
        Failed
    }
}
