using System.ComponentModel.DataAnnotations;

namespace Slotly.Models
{
    public class CreateStaffServiceDto
    {
        [Required]
        public Guid StaffId { get; set; }

        [Required]
        public Guid BusinessServiceId { get; set; }

        [Required]
        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }

        [Required]
        public TimeOnly Duration { get; set; }
    }
}
