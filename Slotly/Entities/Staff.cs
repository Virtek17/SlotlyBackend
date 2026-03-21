using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slotly.Entities
{
    [Table("Staffs")]
    public class Staff
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;  

        [Required]
        public string Position { get; set; } = null!;
       
        public Guid BusinessId { get; set; }
        public virtual Business Business { get; set; } = null!;

        // Связь с реальным пользователем опициональна 
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
