namespace Quantis.WorkFlow.Services.DTOs.Information
{
    public class UserProfilingDTO
    {
        public int ContractPartyId { get; set; }
        public string ContractPartyName { get; set; }
        public int ContractId { get; set; }
        public string ContractName { get; set; }
        public int GlobalRuleId { get; set; }
        public string GlobalRuleName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}