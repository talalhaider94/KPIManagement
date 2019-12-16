using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class TRuleDTO
    {
        public int rule_id { get; set; }
        public int global_rule_id { get; set; }
        public string rule_name { get; set; }
        public string rule_description { get; set; }
        public DateTime create_date { get; set; }
        public DateTime modify_date { get; set; }
        public int sla_version_id { get; set; }
        public double? service_level_target { get; set; }
        public int sla_id { get; set; }
        public string sla_name { get; set; }
        public int version_number { get; set; }
        public int primary_contract_party_id { get; set; }
        public string primary_contract_party_name { get; set; }
        public int? secondary_contract_party_id { get; set; }
        public string secondary_contract_party_name { get; set; }
    }
}