using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.Monitoring
{
    public class MonitoringTicketDTO
    {
        public int GlobalRuleId { get; set; }
        public int ContractId { get; set; }
        public string ContractName { get; set; }
        public int ContractPartyId { get; set; }
        public string ContractPartyName { get; set; }
        public int? OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public int TicketId { get; set; }
        public int TicketRefNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ResultValue { get; set; }
    }
}
