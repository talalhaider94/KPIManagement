using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Quantis.WorkFlow.Services.DTOs.Widgets
{
    public class WidgetViewModel
    {
        public bool ShowMeasure { get; set; }
        public bool ShowChartType { get; set; }
        public bool ShowAggregationOption { get; set; }
        public bool ShowDateType { get; set; }
        public bool ShowDateRangeFilter { get; set; }
        public bool ShowDateFilter { get; set; }
        public bool ShowOrganization { get; set; }
        public bool ShowLevelWiseOrganization { get; set; }
        public bool ShowIncompletePeriodCheck { get; set; }
        public bool ShowPropertyTab { get; set; }
        public bool ShowFilterTab { get; set; }
        public string DefaultDateRange { get; set; }

        public Dictionary<int, string> Measures { get; set; }
        public Dictionary<string, string> ChartTypes { get; set; }
        public Dictionary<string, string> AggregationOptions { get; set; }
        public Dictionary<int, string> DateTypes { get; set; }

        public WidgetViewModel()
        {
            ShowAggregationOption = false;
            ShowChartType = false;
            ShowDateRangeFilter = false;
            ShowMeasure = false;
            ShowDateFilter = false;
            ShowPropertyTab = true;
            ShowFilterTab = true;
            ShowOrganization = true;
            ShowLevelWiseOrganization = false;
            ShowIncompletePeriodCheck = false;
            Measures = new Dictionary<int, string>();
            ChartTypes = new Dictionary<string, string>();
            AggregationOptions = new Dictionary<string, string>();
            DateTypes = new Dictionary<int, string>();
        }

        public void AddMeasure(Measures m)
        {
            Measures.Add((int)m, GetDescription(m));
        }

        private static string GetDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description
                ?? value.ToString();
        }
    }
}