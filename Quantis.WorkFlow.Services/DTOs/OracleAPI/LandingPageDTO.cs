using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.OracleAPI
{
    public class LandingPageDTO
    {
        public int ContractPartyId { get; set; }
        public string ContractPartyName { get; set; }
        public int TotalContracts { get; set; }
        public int TotalKPIs { get; set; }
        public int ComplaintContracts { get; set; }
        public int NonComplaintContracts { get; set; }
        public int ComplaintKPIs { get; set; }
        public int NonComplaintKPIs { get; set; }
        public List<LandingPageContractDTO> BestContracts { get; set; }
        public List<LandingPageContractDTO> WorstContracts { get; set; }
    }
}