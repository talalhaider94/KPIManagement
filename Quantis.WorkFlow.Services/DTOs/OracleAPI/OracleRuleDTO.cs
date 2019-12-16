namespace Quantis.WorkFlow.Services.DTOs.OracleAPI
{
    public class OracleRuleDTO
    {
        public int rule_id { get; set; }
        public string rule_name { get; set; }
        public int global_rule_id { get; set; }
        public int sla_id { get; set; }
        public string sla_name { get; set; }
        public int? service_level_target { get; set; }
        public int? escalation { get; set; }
        public string relation { get; set; }
        public string rule_period_time_unit { get; set; }
        public int? rule_period_interval_length { get; set; }
        public int domain_category_id { get; set; }
        public string domain_category_name { get; set; }
        public OracleRuleGranularityDTO granularity { get; set; }
    }

    public class OracleRuleGranularityDTO
    {
        public string hour_tu_calc_status { get; set; }
        public string day_tu_calc_status { get; set; }
        public string week_tu_calc_status { get; set; }
        public string month_tu_calc_status { get; set; }
        public string quarter_tu_calc_status { get; set; }
        public string year_tu_calc_status { get; set; }
    }
}