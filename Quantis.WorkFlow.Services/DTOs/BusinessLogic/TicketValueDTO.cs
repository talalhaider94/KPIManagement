namespace Quantis.WorkFlow.Services.DTOs.BusinessLogic
{
    public class TicketValueDTO
    {
        public int TicketId { get; set; }
        public float Value { get; set; }
        public string Sign { get; set; }
        public int Type { get; set; }
        public string Note { get; set; }
    }
}