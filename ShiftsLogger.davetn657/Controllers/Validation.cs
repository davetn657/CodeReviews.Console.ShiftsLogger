namespace ShiftsLogger.davetn657.Controllers
{
    internal class Validation
    {
        public Validation()
        {

        }

        internal DateTime? IsValidTime(string time)
        {
            if (DateTime.TryParse(time, out DateTime convertedTime)) return convertedTime;

            return null;
        }

        internal DateTime? IsValidTime(DateTime? startTime, DateTime? endTime)
        {
            if (endTime < startTime) return null;

            return endTime;
        }
    }
}
