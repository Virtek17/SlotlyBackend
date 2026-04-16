using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slotly.Data;
using Slotly.Models.Business;

[Route("api/[controller]")]
[ApiController]
public class BusinessServiceController : ControllerBase
{
    private readonly SlotlyContext _context;
    private readonly BusinessServiceService _service;
    private readonly IMapper _mapper;

    public BusinessServiceController(
        SlotlyContext context,
        BusinessServiceService service,
        IMapper mapper)
    {
        _context = context;
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBusinessServiceDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result
            );
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _context.BusinessServices
            .Include(bs => bs.Service)
            .FirstOrDefaultAsync(bs => bs.Id == id);

        if (entity == null)
            return NotFound();

        return Ok(_mapper.Map<BusinessServiceDto>(entity));
    }

    [HttpGet("by-business/{businessId}")]
    public async Task<IActionResult> GetByBusiness(Guid businessId)
    {
        var result = await _service.GetByBusinessAsync(businessId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.BusinessServices
            .Include(bs => bs.Service)
            .ToListAsync();

        return Ok(_mapper.Map<List<BusinessServiceDto>>(list));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.BusinessServices.FindAsync(id);

        if (entity == null)
            return NotFound();

        _context.BusinessServices.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}