using Microsoft.AspNetCore.Mvc;
using Slotly.Models.WorkingHours;
using Slotly.Services;

[ApiController]
[Route("api/[controller]")]
public class WorkingHoursController : ControllerBase
{
    private readonly WorkingHoursService _service;

    public WorkingHoursController(WorkingHoursService service)
    {
        _service = service;
    }

    [HttpGet("staff/{staffId}")]
    public async Task<IActionResult> GetStaffSchedule(Guid staffId)
    {
        var result = await _service.GetByStaffIdAsync(staffId);
        return Ok(result);
    }

    [HttpPost("sync/{staffId}")]
    public async Task<IActionResult> SyncSchedule(Guid staffId, List<WorkingIntervalDto> dto)
    {
        try
        {
            await _service.SyncAsync(staffId, dto);
            return Ok(new { message = "Расписание обновлено" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}