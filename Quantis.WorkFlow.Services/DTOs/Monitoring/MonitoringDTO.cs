using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.Monitoring
{
    public class MonitoringDTO
    {
        public int ContractId { get; set; }
        public string ContractName { get; set; }
        public int ContractPartyId { get; set; }
        public string ContractPartyName { get; set; }
        public int? OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public int NoOfTicketsToBeOpenedForCompletePeriod { get; set; }
        public List<MonitoringKPIDTO> TicketsToBeOpenedForCompletePeriod { get; set; }
        public int NoOfTicketsToBeOpenedTillToday { get; set; }
        public List<MonitoringKPIDTO> TicketsToBeOpenedTillToday { get; set; }
        public int NoOfTicketsOpenedTillToday { get; set; }
        public List<MonitoringTicketDTO> TicketsOpenedTillToday { get; set; }
    }
}
