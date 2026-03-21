namespace Slotly.Models
{
    public class AssignServiceDto
    {
        public Guid StaffId { get; set; }

        public Guid BusinessServiceId { get; set; }

        public TimeOnly? Duration { get; set; }

        public decimal? Price { get; set; }
    }
}
