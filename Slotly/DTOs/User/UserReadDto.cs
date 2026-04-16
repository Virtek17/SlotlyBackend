using Slotly.Entities;

namespace Slotly.Models.User
{
    public class UserReadDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public UserRole Role{ get; set; }
    }
}
