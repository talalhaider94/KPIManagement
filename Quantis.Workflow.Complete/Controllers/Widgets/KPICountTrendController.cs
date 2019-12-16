using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.Dashboard;
using Quantis.WorkFlow.Services.DTOs.Widgets;

namespace Quantis.Workflow.Complete.Controllers.Widgets
{
    public class KPICountTrendController : BaseWidgetController
    {
        private IGlobalFilterService _globalfilterService;
        private IWidgetService _widgetService;

        public KPICountTrendController(IGlobalFilterService globalfilterService, IWidgetService widgetService)
        {
            _globalfilterService = globalfilterService;
            _widgetService = widgetService;
        }

        internal override void FillWidgetParameters(WidgetViewModel vm)
        {
            vm.DefaultDateRange = _globalfilterService.GetDefualtDateRange();
            vm.ShowMeasure = true;
            vm.ShowChartType = true;
            vm.ShowAggregationOption = true;
            vm.ShowDateType = true;
            vm.ShowDateRangeFilter = true;
            vm.AddMeasure(Measures.Number_of_ticket_in_KPI_in_Verifica);
            vm.AddMeasure(Measures.Number_of_ticket_of_KPI_Compliant);
            vm.AddMeasure(Measures.Number_of_ticket_of_KPI_Non_Calcolato);
            vm.AddMeasure(Measures.Number_of_ticket_of_KPI_Non_Compliant);
            vm.AddMeasure(Measures.Number_of_Total_KPI_compliant);
            vm.AddMeasure(Measures.Number_of_Total_KPI_not_compliant);
            vm.ChartTypes.Add(ChartType.BAR);
            vm.ChartTypes.Add(ChartType.LINE);
            vm.AggregationOptions.Add(AggregationOption.ANNAUL);
            vm.AggregationOptions.Add(AggregationOption.PERIOD);
        }

        internal override object GetData(WidgetParametersDTO props)
        {
            var dto = _globalfilterService.MapAggOptionWidget(props);
            var result = _widgetService.GetKPICountTrend(dto);
            return result;
        }
    }
}