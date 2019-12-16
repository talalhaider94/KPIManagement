namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class DistributionPslDTO
    {
        public PslResultDTO previousPeriod { get; set; }
        public PslResultDTO currentPeriod { get; set; }
    }
}