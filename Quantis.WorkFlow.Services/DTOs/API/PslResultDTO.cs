namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class PslResultDTO
    {
        public int? Compliant { get; set; }
        public int? NonCompliant { get; set; }
        public int? NonCalcolato { get; set; }
        public int? Escalation { get; set; }
    }
}