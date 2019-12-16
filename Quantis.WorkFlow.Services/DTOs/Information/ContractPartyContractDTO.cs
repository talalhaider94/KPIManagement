using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.Information
{
    public class ContractPartyContractDTO
    {
        public int ContractPartyId { get; set; }
        public string ContractPartyName { get; set; }
        public int ContractId { get; set; }
        public string ContractName { get; set; }
        public int DayCuttOff { get; set; }
        public int DayWorkflow { get; set; }
    }
}
