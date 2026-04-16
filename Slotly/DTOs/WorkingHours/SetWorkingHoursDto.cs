namespace Slotly.Models.WorkingHours
{
    public class SetWorkingHoursDto
    {
        public Guid StaffId { get; set; }
        public List<WorkingIntervalDto> Intervals { get; set; } = new();
    }
}
