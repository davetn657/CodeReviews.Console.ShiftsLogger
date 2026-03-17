using Microsoft.EntityFrameworkCore;

namespace ShiftsLogger.WebAPI.davetn657.Data
{
    public class ShiftsLoggerDbContext: DbContext
    {
        public ShiftsLoggerDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Shift> Shifts { get; set; }
    }

    public class Shift
    {
        public int Id { get; set; }
        public string LoggerName { get; set; } = string.Empty;
        public DateTime? ShiftStart { get; set; }
        public DateTime? ShiftEnd { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}
