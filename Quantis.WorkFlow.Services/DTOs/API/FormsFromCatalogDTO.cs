using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class FormsFromCatalogDTO
    {
        public int? id { get; set; }
        public int? form_id { get; set; }
        public string form_name { get; set; }
        public string form_description { get; set; }
        public int? AttachmentsCount { get; set; }
        public int? form_owner_id { get; set; }
        public int? user_group_id { get; set; }
        public string user_group_name { get; set; }
        public int? day_cutoff { get; set; }
        public bool cutoff { get; set; }
        public DateTime latest_input_date { get; set; }
        public string global_rule_name { get; set; }
        public string referent { get; set; }
        public string monthtrigger { get; set; }
        public string sla_name { get; set; }
        public int? global_rule_id { get; set; }
        public string tracking_period { get; set; }
    }
}