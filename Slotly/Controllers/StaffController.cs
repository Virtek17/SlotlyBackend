using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;
using Slotly.Entities;
using Slotly.Interfaces;
using Slotly.Models;
using Slotly.Repositories;
using System.Diagnostics;

namespace Slotly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Staff> _staffRepository;
        private readonly IGenericRepository<Business> _businessRepository;
        private readonly IGenericRepository<BusinessService> _businessServiceRepository;
        private readonly IGenericRepository<StaffService> _staffServiceRepository;

        public StaffController(
            IMapper mapper,
            IGenericRepository<Staff> staffRepository,
            IGenericRepository<Business> businessRepository,
            IGenericRepository<BusinessService> businessServiceRepository,
            IGenericRepository<StaffService> staffServiceRepository
            )
        {
            _mapper = mapper;
            _staffRepository = staffRepository;
            _businessRepository = businessRepository;
            _businessServiceRepository = businessServiceRepository;
            _staffServiceRepository = staffServiceRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto createStaffDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверяем что бизнес существует
            var business = await _businessRepository.GetByIdAsync(createStaffDto.BusinessId);
            if (business == null)
            {
                return BadRequest("Business not found");
            }

            var staff = _mapper.Map<Staff>( createStaffDto );
            staff.Id = Guid.NewGuid();  

            await _staffRepository.AddAsync(staff);
            await _staffRepository.SaveAsync();

            return CreatedAtAction(nameof(GetStaffById), new {id = staff.Id}, staff);
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> GetStaffById(Guid Id)
        {
            var staff = await _staffRepository.GetByIdAsync(Id);

            if (staff == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<ReadStaffDto>(staff);

            return Ok(result);   
        }


        // Все сотрудники бизнеса
        
        [HttpGet("by-business/{Id}")]
        public async Task<IActionResult> GetAllStaffsByBusiness(Guid Id)
        {
            // Проверяем что бизнес существует
            var business = await _businessRepository.GetByIdAsync(Id);
            if (business == null)
            {
                return NotFound("Business not found");
            }

            var allStaffs = await _staffRepository.GetAllAsync();
            var businessStaffs = allStaffs.Where(s => s.BusinessId == Id).ToList();

            var result = _mapper.Map<List<ReadStaffDto>>(businessStaffs);

            return Ok(result);
        }

        [HttpPost("assign-service")]
        public async Task<IActionResult> AssignServiceToStaff([FromBody] CreateStaffServiceDto dto)
        {
            var staff = await _staffRepository.GetByIdAsync(dto.StaffId);
            if (staff == null)
            {
                NotFound("Staff Not Found");
            }

            var bService = await _businessServiceRepository.GetByIdAsync(dto.BusinessServiceId);
            if (bService == null)
            {
                return NotFound("Business service not found");
            }

            var staffService = new StaffService
            {
                Id = Guid.NewGuid(),
                StaffId = dto.StaffId,
                BusinessServiceId = dto.BusinessServiceId,
                Duration = dto.Duration,
                Price = dto.Price,
            };

            await _staffServiceRepository.AddAsync(staffService);
            await _staffServiceRepository.SaveAsync();

            return Ok(staffService);    
        }

        // Получить список сотрудников определенной услуги бизнеса
        [HttpGet("by-service/{businessServiceId}")]
        public async Task<IActionResult> GetAllStaffsByBusinessServiceId(Guid businessServiceId)
        {
            var staffServices = await _staffServiceRepository.GetAllAsync(
                ss => ss.Staff,
                ss => ss.BusinessService,
                ss => ss.BusinessService.Service // Чтобы достать ServiceName
            );

            // Фильтруем уже загруженные данные (раз пока не добавили FindAsync с фильтром)
            var filtered = staffServices.Where(ss => ss.BusinessServiceId == businessServiceId).ToList();

            var result = _mapper.Map<List<StaffServiceDto>>(filtered);

            return Ok(result);


        }

        // Редактирование сотрудника
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateStaff(Guid Id, [FromBody] CreateStaffDto updateStaffDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var staff = await _staffRepository.GetByIdAsync(Id);
            if (staff == null)
            {
                return NotFound();
            }

            // Проверяем что бизнес существует
            var business = await _businessRepository.GetByIdAsync(updateStaffDto.BusinessId);
            if (business == null)
            {
                return BadRequest("Business not found");
            }

            // Обновляем данные
            staff.Name = updateStaffDto.Name;
            staff.Position = updateStaffDto.Position;
            staff.BusinessId = updateStaffDto.BusinessId;
            staff.UserId = updateStaffDto.UserId;

            _staffRepository.Update(staff);
            await _staffRepository.SaveAsync();

            var result = _mapper.Map<ReadStaffDto>(staff);
            return Ok(result);
        }

        // Удаление сотрудника
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteStaff(Guid Id)
        {
            var staff = await _staffRepository.GetByIdAsync(Id);
            if (staff == null)
            {
                return NotFound();
            }

            _staffRepository.Delete(staff);
            await _staffRepository.SaveAsync();

            return NoContent();
        }
    }
}
