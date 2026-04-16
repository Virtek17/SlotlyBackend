using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slotly.Data;
using Slotly.Entities;
using Slotly.Interfaces;
using Slotly.Models.Business;

namespace Slotly.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly SlotlyContext _context;


        public BusinessController(
            IMapper mapper,
            SlotlyContext context
            )
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusiness([FromBody] CreateBusinessDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ////////
            //TODO: вынести в Service
            ////////
            // Проверяем, что владелец существует
            var ownerExists = await _context.Users.AnyAsync(u => u.Id == dto.OwnerId);
            if (!ownerExists)
                return BadRequest("Owner not found");

            // Проверяем, что категория существует
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
               return BadRequest("Category not found");

            // Проверяем, что у владельца еще нет бизнеса с таким названием
            var exists = await _context.Businesses.AnyAsync(b => b.OwnerId == dto.OwnerId && b.Name == dto.Name);
            if (exists)
                return Conflict("Business with this name already exists for this owner");

            var business = new Business
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address,
                OwnerId = dto.OwnerId,
                CategoryId = dto.CategoryId,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<BusinessReadDto>(business);

            return CreatedAtAction(
                nameof(GetBusinessById), 
                new { id = business.Id }, 
                result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusinessById(Guid id)
        {
            var business = await _context.Businesses.FirstOrDefaultAsync(b => b.Id == id);
            if (business == null)
                return NotFound();

            var result = _mapper.Map<BusinessReadDto>(business);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBusinesses()
        {
            var businesses = await _context.Businesses.ToListAsync();

            var result = _mapper.Map<List<BusinessReadDto>>(businesses);
            return Ok(result);
        }

        [HttpGet("by-owner/{ownerId}")]
        public async Task<IActionResult> GetBusinessesByOwner(Guid ownerId)
        {
            var businesses = await _context.Businesses.Where(b => b.OwnerId == ownerId).ToListAsync();

            var result = _mapper.Map<List<BusinessReadDto>>(businesses);

            return Ok(result);
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetBusinessesByCategory(Guid categoryId)
        {
            var businesses = await _context.Businesses.Where(b => b.CategoryId ==  categoryId).ToListAsync();

            var result = _mapper.Map<List<BusinessReadDto>>(businesses);
            return Ok(result);
        }

        
    }
}
