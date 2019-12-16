using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.Monitoring
{
    public class MonitoringKPIDTO
    {
        public int GlobalRuleId { get; set; }
        public string GlobalRuleName { get; set; }
        public int ContractId { get; set; }
        public string ContractName { get; set; }
        public int ContractPartyId { get; set; }
        public string ContractPartyName { get; set; }
        public int? OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public int WorkflowDay { get; set; }
    }
}
