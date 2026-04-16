using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Slotly.Services;

namespace Slotly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotController : ControllerBase
    {
        private readonly SlotService _slotService;
       public SlotController(SlotService slotService)
        {
            _slotService = slotService;
        }

        [HttpGet("slots")]
        public async Task<IActionResult> GetSlots(Guid staffId, Guid staffServiceId, DateTime date)
        {
            var slots = await _slotService.GetAvailabeSlots(staffId, staffServiceId, date);

            return Ok(slots);   
        }
    }
}
