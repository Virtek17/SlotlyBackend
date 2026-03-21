using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slotly.Entities
{
    [Table("Business")]
    public class Business
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        public string? Address { get; set; }

        public Guid OwnerId { get; set; }
        public virtual User Owner { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; }
    }
}
