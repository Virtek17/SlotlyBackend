using Slotly.Entities;
using System.ComponentModel.DataAnnotations;

namespace Slotly.Models
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        public long? TelegramId { get; set; }

        public UserRole Role { get; set; } = UserRole.Client;
    }
}
