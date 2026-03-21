using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slotly.Entities
{
    [Table("WorkingHours")]
    public class WorkingHours
    {
        [Key]
        public Guid Id { get; set; }

        public Guid StaffId { get; set; }
        public virtual Staff Staff { get; set; } = null!;

        public int DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
