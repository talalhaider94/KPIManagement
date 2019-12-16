using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class ARulesDTO
    {
        public string id_kpi { get; set; }
        public string name_kpi { get; set; }
        public DateTime interval_kpi { get; set; }
        public string value_kpi { get; set; }
        public int ticket_id { get; set; }
        public DateTime close_timestamp_ticket { get; set; }
        public bool archived { get; set; }
        public string customer_name { get; set; }
        public string contract_name { get; set; }
        public string kpi_name_bsi { get; set; }
        public int rule_id_bsi { get; set; }
        public int global_rule_id { get; set; }
        public string tracking_period { get; set; }
        public string symbol { get; set; }
        public bool progressive { get; set; }
        public string kpi_description_bsi { get; set; }
    }
}