using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slotly.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        [Required]
        public string PasswordHash { get; set; } = null!;

        public long? TelegramId { get; set; }

        public UserRole Role { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }

    public enum UserRole
    {
        Client,
        BusinessOwner,
        Staff
    }
}
