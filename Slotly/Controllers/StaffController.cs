using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using Slotly.Data;
using Slotly.Entities;
using Slotly.Interfaces;
using Slotly.Models.Staff;
using Slotly.Repositories;
using System.Diagnostics;

namespace Slotly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly SlotlyContext _context;

        public StaffController(
            IMapper mapper,
            SlotlyContext context
            )
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var business = await _context.Businesses.AnyAsync(b => b.Id == dto.BusinessId);
            if (!business) return BadRequest("Business not found");

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                BusinessId = dto.BusinessId,
                Position = dto.Position,
                UserId = dto?.UserId,
            };

            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();

            var resutl = _mapper.Map<ReadStaffDto>( staff );

            return CreatedAtAction(nameof(GetStaffById), new {id = staff.Id}, resutl);
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> GetStaffById(Guid Id)
        {
            var staff = await _context.Staffs.FindAsync(Id);
                
            if (staff == null) return NotFound();

            var result = _mapper.Map<ReadStaffDto>(staff);

            return Ok(result);   
        }


        // Все сотрудники бизнеса
        [HttpGet("by-business/{Id}")]
        public async Task<IActionResult> GetAllStaffsByBusiness(Guid Id)
        {
            var staffs = await _context.Staffs
                .Where(s => s.BusinessId ==  Id)
                .ToListAsync();

            var result = _mapper.Map<List<ReadStaffDto>>(staffs);

            return Ok(result);
        }

        [HttpPost("assign-service")]
        public async Task<IActionResult> AssignServiceToStaff([FromBody] CreateStaffServiceDto dto)
        {
            var staffExists = await _context.Staffs.AnyAsync(s => s.Id == dto.StaffId);
            if (!staffExists) NotFound("Staff Not Found");

            var bServiceExists = await _context.BusinessServices.AnyAsync(bs => bs.Id == dto.BusinessServiceId);
            if (!bServiceExists) return NotFound("Business service not found");

            var staffService = new StaffService
            {
                Id = Guid.NewGuid(),
                StaffId = dto.StaffId,
                BusinessServiceId = dto.BusinessServiceId,
                Duration = dto.Duration,
                Price = dto.Price,
            };

            _context.StaffServices.Add(staffService);
            await _context.SaveChangesAsync();

            return Ok(staffService);    
        }

        // Получить список сотрудников определенной услуги бизнеса
        [HttpGet("by-service/{businessServiceId}")]
        public async Task<IActionResult> GetAllStaffsByBusinessServiceId(Guid businessServiceId)
        {
            var staffServices = await _context.StaffServices
                .Where(ss => ss.BusinessServiceId == businessServiceId)
                .Include(ss => ss.Staff)
                .Include(ss => ss.BusinessService)
                    .ThenInclude(bs => bs.Service)
                .ToListAsync();
                

            var result = _mapper.Map<List<StaffServiceDto>>(staffServices);

            return Ok(result);


        }

        // Редактирование сотрудника
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateStaff(Guid Id, [FromBody] CreateStaffDto dto)
        {
            var staff = await _context.Staffs.FindAsync(Id);
            if (staff == null) return NotFound();
           
            // Обновляем данные
            staff.Name = dto.Name;
            staff.Position = dto.Position;
            staff.BusinessId = dto.BusinessId;
            staff.UserId = dto.UserId;

            await _context.SaveChangesAsync();

            var result = _mapper.Map<ReadStaffDto>(staff);
            return Ok(result);
        }

        // Удаление сотрудника
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteStaff(Guid Id)
        {
            var staff = await _context.Staffs.FindAsync(Id);
            if (staff==null) return NotFound();

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
