namespace Quantis.WorkFlow.Services.DTOs.OracleAPI
{
    public class LandingPageContractDTO
    {
        public int ContractId { get; set; }
        public string ContractName { get; set; }
        public int TotalKPIs { get; set; }
        public int ComplaintKPIs { get; set; }
        public int NonComplaintKPIs { get; set; }
        public double ComplaintPercentage { get; set; }
        public double AverageDeviation { get; set; }
    }
}