using Microsoft.EntityFrameworkCore;
using Slotly.Data;
using Slotly.Entities;
using Slotly.Models.WorkingHours;

namespace Slotly.Services
{
    public class WorkingHoursService
    {
        private readonly SlotlyContext _context;

        public WorkingHoursService(SlotlyContext context)
        {
            _context = context;
        }

        public async Task<List<WorkingIntervalDto>> GetByStaffIdAsync(Guid staffId)
        {
            var shedule = await _context.WorkingHours
                .Where(x => x.StaffId == staffId)
                .ToListAsync();

            return shedule.Select(s => new WorkingIntervalDto
            {
                DayOfWeek = s.DayOfWeek,
                EndTime = s.EndTime,
                StartTime  = s.StartTime,   
            }).ToList();
        }

        public async Task SyncAsync (Guid staffId, List<WorkingIntervalDto> dto)
        {
            var staffExist = await _context.Staffs.AnyAsync(s => s.Id == staffId);
            if (!staffExist) throw new Exception("Staff not found");

            var intervals = dto.Select(i =>
            {
                if (i.StartTime >= i.EndTime)
                    throw new Exception("Invalid time interval");

                return new WorkingHours
                {
                    Id = Guid.NewGuid(),
                    StaffId = staffId,
                    DayOfWeek = i.DayOfWeek,
                    EndTime = i.EndTime,
                    StartTime = i.StartTime,
                };
            }).ToList();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existing = _context.WorkingHours.Where(x => x.StaffId == staffId);
                _context.WorkingHours.RemoveRange(existing);
                await _context.WorkingHours.AddRangeAsync(intervals);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            } catch 
            {
                await transaction.RollbackAsync();
                throw new Exception("Data base was not update");
            }
        }
    }
}
