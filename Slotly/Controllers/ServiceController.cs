using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slotly.Data;
using Slotly.Entities;
using Slotly.Interfaces;

namespace Slotly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly SlotlyContext _context;

        public ServiceController(SlotlyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            // Сделать DTO
            var service = await _context.Services.ToListAsync();

            return Ok(service);
        }

        [HttpGet("by-staff/{staffId}")]
        public async Task<IActionResult> GetServiceByStaffId(Guid staffId)
        {
            var staffExists = await _context.Staffs.AnyAsync(s => s.Id == staffId);

            if (!staffExists) return NotFound("Staff not found");

            var service = await _context.StaffServices
                .Where(ss => ss.StaffId == staffId)
                .Include(ss => ss.BusinessService)
                    .ThenInclude(bs => bs.Service)
                .ToListAsync();
            var result = service.Select(ss => new
            {
                StaffServiceId = ss.Id,
                ServiceId = ss.BusinessService.Service.Id,
                Name = ss.BusinessService.Service.Name,
                Price = ss.Price,
                Duration = ss.Duration
            });

            return Ok(result);
        }
    }
}
