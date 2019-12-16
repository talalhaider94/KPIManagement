using System;
using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.Dashboard;
using Quantis.WorkFlow.Services.DTOs.Widgets;

namespace Quantis.Workflow.Complete.Controllers.Widgets
{
    public class KPIReportTrendController : BaseWidgetController
    {
        private IGlobalFilterService _globalfilterService;
        private IWidgetService _widgetService;

        public KPIReportTrendController(IGlobalFilterService globalfilterService, IWidgetService widgetService)
        {
            _globalfilterService = globalfilterService;
            _widgetService = widgetService;
        }

        internal override void FillWidgetParameters(WidgetViewModel vm)
        {
            vm.DefaultDateRange = _globalfilterService.GetDefualtDateRange();
            vm.ShowChartType = false;
            vm.ShowDateType = true;
            vm.ShowDateRangeFilter = true;
            vm.ShowAggregationOption = true;
            vm.ShowIncompletePeriodCheck = true;
            vm.AggregationOptions.Add(AggregationOption.PERIOD);
            vm.AggregationOptions.Add(AggregationOption.TRACKINGPERIOD);

            vm.ShowLevelWiseOrganization = true;
            vm.ShowOrganization = false;
        }

        internal override object GetData(WidgetParametersDTO props)
        {
            
            var dto = _globalfilterService.MapAggOptionWidget(props);
            if (dto.IncompletePeriod && dto.DateRange.Item2.Year == DateTime.Now.Year &&
                dto.DateRange.Item2.Month == DateTime.Now.Month)
            {
                dto.DateRange = new Tuple<DateTime, DateTime>(dto.DateRange.Item1, dto.DateRange.Item2.AddMonths(-1));
                var result = _widgetService.GetKPIReportTrend(dto);
                dto.DateRange = new Tuple<DateTime, DateTime>(DateTime.Now, DateTime.Now);
                var result2 = _widgetService.GetKPIReportTrend(dto, 0);
                result.AddRange(result2);
                return result;
            }
            else
            {
                var result = _widgetService.GetKPIReportTrend(dto);
                return result;
            }
            
        }
    }
}