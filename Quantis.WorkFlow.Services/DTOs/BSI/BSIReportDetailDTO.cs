using Quantis.WorkFlow.Services.DTOs.Widgets;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.BSI
{
    public class BSIReportDetailDTO
    {
        public string Name { get; set; }
        public string ReportType { get; set; }
        public string ReportTitle { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ContractParty { get; set; }
        public string Contract { get; set; }
        public string Rule { get; set; }
        public string Application { get; set; }
        public string ServiceDomain { get; set; }
        public string DomainCategory { get; set; }
        public string Incomplete { get; set; }
        public string MetricType { get; set; }
        public string DataGranularity { get; set; }
        public string DefAgg { get; set; }
        public int LocaleId { get; set; }
        public int GlobalRuleId { get; set; }
        public string Units { get; set; }
        public string GridUnits { get; set; }
        public List<string> Messages { get; set; }
        public string CalculationStatusText { get; set; }
        public string CalculationStatusBookletText { get; set; }
        public string CalculationStatusLastDate { get; set; }
        public string XLabel { get; set; }
        public string YLabel { get; set; }
        public List<XYZDTO> Data { get; set; }
    }
}