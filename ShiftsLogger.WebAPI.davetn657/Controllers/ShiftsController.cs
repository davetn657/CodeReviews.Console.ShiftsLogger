using Microsoft.AspNetCore.Mvc;
using ShiftsLogger.WebAPI.davetn657.Data;
using ShiftsLogger.WebAPI.davetn657.Services;

namespace ShiftsLogger.WebAPI.davetn657.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftsController : ControllerBase
    {
        private readonly ILoggingService _loggingService;
        public ShiftsController(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        [HttpGet]
        public ActionResult<List<Shift>> GetAllShifts()
        {
            return Ok(_loggingService.GetAllShifts());
        }

        [HttpGet("{id}")]
        public ActionResult<Shift> GetShiftById(int id)
        {
            var result = _loggingService.GetShiftById(id);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public ActionResult<Shift> CreateShift(Shift shift)
        {
            return Ok(_loggingService.CreateShift(shift));
        }

        [HttpPut("{id}")]
        public ActionResult<Shift> UpdateShift(int id, Shift updatedShift)
        {
            var result = _loggingService.UpdateShift(id, updatedShift);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public ActionResult<string> DeleteShift(int id)
        {
            var result = _loggingService.DeleteShift(id);

            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
