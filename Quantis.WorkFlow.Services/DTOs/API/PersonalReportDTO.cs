using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class PersonalReportDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public int global_rule_id { get; set; }
        public string report_type { get; set; }
        public string aggregation_option { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public DateTime modification_date { get; set; }
        public int? global_rule2_id { get; set; }
        public string aggregation_option2 { get; set; }
        public string start_date2 { get; set; }
        public string end_date2 { get; set; }
    }
}
