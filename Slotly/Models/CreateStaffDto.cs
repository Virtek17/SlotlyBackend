using System.ComponentModel.DataAnnotations;

namespace Slotly.Models
{
    public class CreateStaffDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Position { get; set; } = null!;

        [Required]
        public Guid BusinessId { get; set; }

        public Guid? UserId { get; set; }
    }
}
