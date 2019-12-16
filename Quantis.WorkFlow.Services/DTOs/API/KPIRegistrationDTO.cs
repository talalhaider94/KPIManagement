namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class KPIRegistrationDTO
    {
        public string BusinessLogicMethod { get; set; }
        public int EventTypeId { get; set; }
        public bool IsClustered { get; set; }
        public int RegistrationdId { get; set; }
        public string RegistrationType { get; set; }
        public int ResourceId { get; set; }
        public int ResourceGroupId { get; set; }
        public int ResourceTypeId { get; set; }
        public int SourceMetricId { get; set; }
    }
}