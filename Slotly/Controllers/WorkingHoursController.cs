using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Slotly.Entities;
using Slotly.Interfaces;
using Slotly.Models;

namespace Slotly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkingHoursController : ControllerBase
    {
        private readonly IWorkingHoursRepository _repository;

        public WorkingHoursController(IWorkingHoursRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("staff/{staffId}")]
        public async Task<IActionResult> GetStaffSchedule(Guid staffId)
        {
            var schedule = await _repository.GetByStaffIdAsync(staffId);

            var result = schedule.Select(s => new WorkingIntervalDto
            {
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,

            });

            return Ok(result);
        }

        [HttpPost("sync/{staffId}")]
        public async Task<IActionResult> SyncShadule(Guid staffId, [FromBody] List<WorkingIntervalDto> dto)
        {
            if (dto == null) return BadRequest("Данные не переданы");

            var intervals = dto.Select(i => new WorkingHours
            {
                Id = Guid.NewGuid(),
                StaffId = staffId,
                DayOfWeek = i.DayOfWeek,
                StartTime = i.StartTime,
                EndTime = i.EndTime
            }).ToList();

            try
            {
                await _repository.SyncIntervalsAsync(staffId, intervals);
                return Ok(new { message = "расписание успешно обновлено" });
            } catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при сохранении расписания");
            } 
        }
    }
}
