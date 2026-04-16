using Slotly.Entities;
using System.ComponentModel.DataAnnotations;

namespace Slotly.Models.Business
{
    public class CreateBusinessDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [Required]
        public Guid OwnerId { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
    }
}
