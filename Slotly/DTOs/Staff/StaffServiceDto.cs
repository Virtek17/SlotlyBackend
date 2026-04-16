namespace Slotly.Models.Staff
{
    public class StaffServiceDto
    {
        public Guid Id { get; set; }

        public Guid StaffId { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }

        public decimal Price { get; set; }

        public TimeOnly Duration { get; set; }

        public string ServiceName { get; set; }


    }
}
