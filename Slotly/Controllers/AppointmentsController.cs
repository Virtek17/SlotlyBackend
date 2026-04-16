using Microsoft.AspNetCore.Mvc;
using Slotly.DTOs.Appointment;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentService _service;

    public AppointmentsController(AppointmentService service)
    {
        _service = service;
    }

    [HttpGet("staff/{staffId}")]
    public async Task<IActionResult> GetByStaff(Guid staffId)
    {
        try
        {
            var result = await _service.GetByStaffAsync(staffId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}