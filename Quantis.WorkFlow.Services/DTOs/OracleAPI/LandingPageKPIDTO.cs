namespace Quantis.WorkFlow.Services.DTOs.OracleAPI
{
    public class LandingPageKPIDTO
    {
        public int KPIID { get; set; }
        public string KPIName { get; set; }
        public double? Target { get; set; }
        public double? Value { get; set; }
        public string Unit { get; set; }
        public string Operator { get; set; }
    }
}