using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slotly.Entities
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? BusinessId { get; set; }
        public virtual Business Business { get; set; } = null!;

        public Guid? ClientId { get; set; }
        public virtual User? Client { get; set; } = null!;

        public Guid StaffId { get; set; }
        public virtual Staff Staff { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public AppointmentStatus Status { get; set; }

        public decimal FinalPrice { get; set; }

        public string? Description { get; set; }

        public string? Title { get; set; }
    }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed,
        NoShow
    }
}
