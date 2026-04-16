namespace Slotly.Models.Staff
{
    public class ReadStaffDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Position { get; set; }
        // TODO: Добавить отображение названия бизнеса + маппинг
        //public string BusinessName { get; set; }
    }
}
