using ShiftsLogger.WebAPI.davetn657.Data;

namespace ShiftsLogger.WebAPI.davetn657.Services
{
    public interface ILoggingService
    {
        public List<Shift> GetAllShifts();
        public Shift? GetShiftById(int id);
        public Shift CreateShift(Shift shift);
        public Shift UpdateShift(int id, Shift updatedShift);
        public string DeleteShift(int id);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ShiftsLoggerDbContext _dbContext;

        public LoggingService(ShiftsLoggerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Shift> GetAllShifts()
        {
            return _dbContext.Shifts.ToList();
        }

        public Shift? GetShiftById(int id)
        {
            var savedFlight = _dbContext.Shifts.Find(id);
            return savedFlight;
        }

        public Shift CreateShift(Shift shift)
        {
            var savedShift = _dbContext.Shifts.Add(shift);
            _dbContext.SaveChanges();
            return savedShift.Entity;
        }

        public Shift UpdateShift(int id, Shift updatedShift)
        {
            var savedShift = _dbContext.Shifts.Find(id);

            if (savedShift == null) return null;

            _dbContext.Entry(savedShift).CurrentValues.SetValues(updatedShift);
            _dbContext.SaveChanges();

            return savedShift;
        }

        public string DeleteShift(int id)
        {
            var savedShift = _dbContext.Shifts.Find(id);

            if (savedShift == null) return null;

            _dbContext.Shifts.Remove(savedShift);
            _dbContext.SaveChanges();

            return $"Successfully deleted shift with id {savedShift.Id}";
        }
    }
}
