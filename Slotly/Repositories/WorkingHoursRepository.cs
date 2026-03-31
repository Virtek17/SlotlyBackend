using Microsoft.EntityFrameworkCore;
using Slotly.Data;
using Slotly.Entities;
using Slotly.Interfaces;

namespace Slotly.Repositories
{
    public class WorkingHoursRepository : GenericRepository<WorkingHours>, IWorkingHoursRepository
    {

        public WorkingHoursRepository(SlotlyContext context) : base(context) { }

        public async Task<IEnumerable<WorkingHours>> GetByStaffIdAsync(Guid staffId)
        {
            return await _context.WorkingHours
                .Where(wh => wh.StaffId == staffId)
                .ToListAsync();
        }

        public async Task SyncIntervalsAsync(Guid staffId, IEnumerable<WorkingHours> intervals)
        {
            // Используем _context, который доступен из GenericRepository (protected)
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Находим и удаляем старые интервалы этого сотрудника
                var existing = _context.WorkingHours.Where(wh => wh.StaffId == staffId);
                _context.WorkingHours.RemoveRange(existing);

                // 2. Добавляем новые
                await _context.WorkingHours.AddRangeAsync(intervals);

                // 3. Сохраняем и фиксируем транзакцию
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Если что-то пошло не так (например, БД упала), откатываем всё назад
                await transaction.RollbackAsync();
                throw; // Пробрасываем ошибку дальше в контроллер
            }
        }
    }
}
