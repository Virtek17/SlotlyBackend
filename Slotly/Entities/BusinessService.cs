using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slotly.Entities
{
    [Table("BusinessServices")]
    public class BusinessService
    {
        [Key]
        public Guid Id { get; set; }

        public Guid BusinessId { get; set; }
        public virtual Business Business { get; set; } = null!;

        public Guid ServiceId { get; set; }
        public virtual Service Service { get; set; } = null!;

        public decimal BasePrice { get; set; }

        public TimeOnly BaseDuration { get; set; }
    }
}
