using System;

namespace Quantis.WorkFlow.Services.DTOs.OracleAPI
{
    public class PslDTO
    {
        public Decimal sla_id { get; set; }
        public Decimal rule_id { get; set; }
        public Decimal provided { get; set; }
        public Decimal provided_c { get; set; }
        public Decimal provided_e { get; set; }
        public Decimal provided_ce { get; set; }
        public DateTime time_stamp_utc { get; set; }
        public string result { get; set; }
        public Decimal target { get; set; }
        public string relation { get; set; }
        public string symbol { get; set; }
    }
}