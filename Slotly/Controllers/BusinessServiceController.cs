using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Slotly.Entities;
using Slotly.Interfaces;
using Slotly.Models;

namespace Slotly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessServiceController : ControllerBase
    {
        private readonly IGenericRepository<BusinessService> _businessServiceRepository;
        private readonly IGenericRepository<Business> _businessRepository;
        private readonly IGenericRepository<Service> _serviceRepository;
        private readonly IMapper _mapper;

        public BusinessServiceController(
            IGenericRepository<BusinessService> businessServiceRepository,
            IGenericRepository<Business> businessRepository,
            IGenericRepository<Service> serviceRepository,
            IMapper mapper)
        {
            _businessServiceRepository = businessServiceRepository;
            _businessRepository = businessRepository;
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusinessService([FromBody] CreateBusinessServiceDto createBusinessServiceDto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            // Проверяем, что бизнес существует
            var business = await _businessRepository.GetByIdAsync(createBusinessServiceDto.BusinessId);
            if (business == null)
            {
                return BadRequest("Бизнес не найдена");
            }

            // Проверяем, что услуга существует
            var service = await _serviceRepository.GetByIdAsync(createBusinessServiceDto.ServiceId);
            if (service == null)
            {
                return BadRequest("Услуга не найдена");
            }

            // Проверяем, что такая связь уже не существует
            var existingServices = await _businessServiceRepository.GetAllAsync();
            if (existingServices.Any(bs => bs.BusinessId == createBusinessServiceDto.BusinessId && 
                                        bs.ServiceId == createBusinessServiceDto.ServiceId))
            {
                return Conflict("This service is already linked to this business");
            }

            // Парсим время из строки
            if (!TimeOnly.TryParse(createBusinessServiceDto.BaseDuration, out var duration))
            {
                return BadRequest("Invalid duration format. Use format 'HH:mm'");
            }

            var businessService = new BusinessService
            {
                Id = Guid.NewGuid(),
                BusinessId = createBusinessServiceDto.BusinessId,
                ServiceId = createBusinessServiceDto.ServiceId,
                BasePrice = createBusinessServiceDto.BasePrice,
                BaseDuration = duration
            };

            await _businessServiceRepository.AddAsync(businessService);
            await _businessServiceRepository.SaveAsync();

            return CreatedAtAction(nameof(GetBusinessServiceById), new { id = businessService.Id }, businessService);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusinessServiceById(Guid id)
        {
            var businessService = await _businessServiceRepository.GetByIdAsync(id);
            if (businessService == null)
            {
                return NotFound();
            }

            return Ok(businessService);
        }

        [HttpGet("by-business/{businessId}")]
        public async Task<IActionResult> GetServicesByBusiness(Guid businessId)
        {
            // Проверяем, что бизнес существует
            var business = await _businessRepository.GetByIdAsync(businessId);
            if (business == null)
            {
                return NotFound("Business not found");
            }

            var businessServices = await _businessServiceRepository.GetAllAsync(bs => bs.Service);
            var services = businessServices
                .Where(bs => bs.BusinessId == businessId)
                .ToList();

            var servicesDto = _mapper.Map<List<BusinessServiceDto>>(services);

            return Ok(servicesDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBusinessServices()
        {
            var businessServices = await _businessServiceRepository.GetAllAsync();
            return Ok(businessServices);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteBusinessService(Guid Id)
        {
            var businessService = await _businessServiceRepository.GetByIdAsync(Id);
            if (businessService == null)
            {
                return NotFound();
            }

            _businessServiceRepository.Delete(businessService);
            await _businessServiceRepository.SaveAsync();

            return NoContent();
        }
    }
}
