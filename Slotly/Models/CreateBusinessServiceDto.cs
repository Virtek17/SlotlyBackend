using System.ComponentModel.DataAnnotations;

namespace Slotly.Models
{
    public class CreateBusinessServiceDto
    {
        [Required]
        public Guid BusinessId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }


        [Required]
        [Range(0.01, 10000.00)]
        public decimal BasePrice { get; set; }

        [Required]
        public string BaseDuration { get; set; } = null!;
    }
}
