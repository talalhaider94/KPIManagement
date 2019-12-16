using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class OracleSlaDTO
    {
        public int sla_id { get; set; }
        public string sla_name { get; set; }
        public int sla_version_id { get; set; }
        public int last_version { get; set; }
        public string sla_status { get; set; }
        public DateTime sla_valid_from { get; set; }
        public DateTime sla_valid_to { get; set; }
        public int customer_id { get; set; }
        public string customer_name { get; set; }
    }
}