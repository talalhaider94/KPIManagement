using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class PersonalReportLVDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public int global_rule_id { get; set; }
        public string contract_party_name { get; set; }
        public string contract_name { get; set; }
        public string kpi_name { get; set; }
    }
}
