using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.OracleAPI
{
    public class PersonalReportFilterDTO
    {
        public int GlobalRuleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 
        public string AggregationOption { get; set; }
    }
}
