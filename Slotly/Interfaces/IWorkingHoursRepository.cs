using Slotly.Entities;

namespace Slotly.Interfaces
{
    public interface IWorkingHoursRepository : IGenericRepository<WorkingHours>
    {
        Task SyncIntervalsAsync(Guid staffId, IEnumerable<WorkingHours> intervals);
        Task<IEnumerable<WorkingHours>> GetByStaffIdAsync(Guid staffId);

    }
}
