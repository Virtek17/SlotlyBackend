using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Slotly.Data;
using Slotly.Entities;
using Slotly.Models.Business;

public class BusinessServiceService 
{
    private readonly SlotlyContext _context;
    private readonly IMapper _mapper;

    public BusinessServiceService(SlotlyContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BusinessServiceDto> CreateAsync(CreateBusinessServiceDto dto)
    {
        var businessExists = await _context.Businesses
            .AnyAsync(b => b.Id == dto.BusinessId);

        if (!businessExists)
            throw new Exception("Business not found");

        var serviceExists = await _context.Services
            .AnyAsync(s => s.Id == dto.ServiceId);

        if (!serviceExists)
            throw new Exception("Service not found");

        var exists = await _context.BusinessServices
            .AnyAsync(bs => bs.BusinessId == dto.BusinessId &&
                            bs.ServiceId == dto.ServiceId);

        if (exists)
            throw new Exception("Service already linked to business");

        if (!TimeOnly.TryParse(dto.BaseDuration, out var duration))
            throw new Exception("Invalid duration format");

        var entity = new BusinessService
        {
            Id = Guid.NewGuid(),
            BusinessId = dto.BusinessId,
            ServiceId = dto.ServiceId,
            BasePrice = dto.BasePrice,
            BaseDuration = duration
        };

        _context.BusinessServices.Add(entity);
        await _context.SaveChangesAsync();

        return _mapper.Map<BusinessServiceDto>(entity);
    }

    public async Task<List<BusinessServiceDto>> GetByBusinessAsync(Guid businessId)
    {
        var services = await _context.BusinessServices
            .Where(bs => bs.BusinessId == businessId)
            .Include(bs => bs.Service)
            .ToListAsync();

        return _mapper.Map<List<BusinessServiceDto>>(services);
    }
}