using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slotly.Entities
{
    [Table("StaffServices")]
    public class StaffService
    {
        [Key]
        public Guid Id { get; set; }

        public Guid StaffId { get; set; }
        public virtual Staff Staff { get; set; } = null!;

        public Guid BusinessServiceId { get; set; }
        public virtual BusinessService BusinessService { get; set; } = null!;

        public decimal Price { get; set; }

        public TimeOnly Duration { get; set; }
    }
}
