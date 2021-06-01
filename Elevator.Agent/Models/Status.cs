using Newtonsoft.Json;

namespace Elevator.Agent.Models
{
    public enum Status
    {
        Free,
        Working,
        Finished
    }

    [JsonObject]
    public class StatusResponse
    {
        public string Status { get; set; }
    }
}
