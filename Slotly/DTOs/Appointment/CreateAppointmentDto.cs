namespace Slotly.DTOs.Appointment
{
    public class CreateAppointmentDto
    {
        public Guid StaffId { get; set; }

        public Guid? ClientId { get; set; }

        public Guid StaffServiceId { get; set; }

        public DateTime StartTime { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }
    }
}
