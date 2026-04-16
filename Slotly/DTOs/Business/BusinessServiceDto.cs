namespace Slotly.Models.Business
{
    public class BusinessServiceDto
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; }

        public decimal BasePrice { get; set; }

        public TimeOnly BaseDuration { get; set; }
    }
}
