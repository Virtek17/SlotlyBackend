using Slotly.Entities;

namespace Slotly.Models.Appointment
{
    public class GetAppointmentDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public decimal FinalPrice { get; set; }

    }
}
