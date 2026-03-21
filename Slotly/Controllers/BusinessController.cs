using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Slotly.Entities;
using Slotly.Interfaces;
using Slotly.Models;

namespace Slotly.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Business> _businessRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Category> _categoryRepository;

        public BusinessController(
            IMapper mapper,
            IGenericRepository<Business> businessRepository,
            IGenericRepository<User> userRepository,
            IGenericRepository<Category> categoryRepository)
        {
            _mapper = mapper;
            _businessRepository = businessRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusiness([FromBody] CreateBusinessDto createBusinessDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверяем, что владелец существует
            var owner = await _userRepository.GetByIdAsync(createBusinessDto.OwnerId);
            if (owner == null)
            {
                return BadRequest("Owner not found");
            }

            // Проверяем, что категория существует
            var category = await _categoryRepository.GetByIdAsync(createBusinessDto.CategoryId);
            if (category == null)
            {
                return BadRequest("Category not found");
            }

            // Проверяем, что у владельца еще нет бизнеса с таким названием
            var existingBusinesses = await _businessRepository.GetAllAsync();
            if (existingBusinesses.Any(b => b.OwnerId == createBusinessDto.OwnerId && b.Name == createBusinessDto.Name))
            {
                return Conflict("Business with this name already exists for this owner");
            }

            var business = new Business
            {
                Id = Guid.NewGuid(),
                Name = createBusinessDto.Name,
                Description = createBusinessDto.Description,
                Address = createBusinessDto.Address,
                OwnerId = createBusinessDto.OwnerId,
                CategoryId = createBusinessDto.CategoryId,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _businessRepository.AddAsync(business);
            await _businessRepository.SaveAsync();

            return CreatedAtAction(nameof(GetBusinessById), new { id = business.Id }, business);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusinessById(Guid id)
        {
            var business = await _businessRepository.GetByIdAsync(id);
            if (business == null)
            {
                return NotFound();
            }

            return Ok(business);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBusinesses()
        {
            var businesses = await _businessRepository.GetAllAsync(b => b.Category);

            var businessDto = _mapper.Map<IEnumerable<BusinessReadDto>>(businesses);

            return Ok(businessDto);
        }

        [HttpGet("by-owner/{ownerId}")]
        public async Task<IActionResult> GetBusinessesByOwner(Guid ownerId)
        {
            var businesses = await _businessRepository.GetAllAsync();
            var ownerBusinesses = businesses.Where(b => b.OwnerId == ownerId).ToList();
            return Ok(ownerBusinesses);
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetBusinessesByCategory(Guid categoryId)
        {
            var businesses = await _businessRepository.GetAllAsync();
            var categoryBusinesses = businesses.Where(b => b.CategoryId == categoryId).ToList();
            return Ok(categoryBusinesses);
        }

        
    }
}
