using System.Text.Json.Serialization;

namespace ShiftsLogger.davetn657.Models
{
    internal class Shifts
    {
        public List<Shift?> UserShifts { get; set; } = new List<Shift>();
    }

    internal class Shift
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("loggerName")]
        public string LoggerName { get; set; } = string.Empty;
        [JsonPropertyName("shiftStart")]
        public DateTime? ShiftStart { get; set; }
        [JsonPropertyName("shiftEnd")]
        public DateTime? ShiftEnd { get; set; }
        [JsonPropertyName("duration")]
        public TimeSpan? Duration { get; set; }
    }
}
