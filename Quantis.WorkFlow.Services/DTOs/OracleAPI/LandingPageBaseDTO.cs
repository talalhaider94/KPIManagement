namespace Quantis.WorkFlow.Services.DTOs.OracleAPI
{
    public class LandingPageBaseDTO
    {
        public int ContractPartyId { get; set; }
        public string ContractPartyName { get; set; }
        public int ContractId { get; set; }
        public string ContractName { get; set; }
        public int GlobalRuleId { get; set; }
        public string GlobalRuleName { get; set; }
        public double Target { get; set; }
        public double Actual { get; set; }
        public string Result { get; set; }
        public double Deviation { get; set; }
        public string Unit { get; set; }
        public string Operator { get; set; }
    }
}