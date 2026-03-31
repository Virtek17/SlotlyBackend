using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slotly.Entities
{
    [Table("WorkingHours")]
    // Свободные часы сотрдуника
    public class WorkingHours
    {
        [Key]
        public Guid Id { get; set; }

        public Guid StaffId { get; set; }
        public virtual Staff Staff { get; set; } = null!;

        // Сотрудник Вася работает пн, ср, пт в с 9 до 17
        // Может быть вторая таблица где расписаны его рабочие часы на вторник и четверг с другим временем
        
        // ["понедельник", "среда", "пятница"]
        public DayOfWeek DayOfWeek { get; set; }

        // 09:00
        public TimeOnly StartTime { get; set; }
        // 17:00
        public TimeOnly EndTime { get; set; }
    }
}
