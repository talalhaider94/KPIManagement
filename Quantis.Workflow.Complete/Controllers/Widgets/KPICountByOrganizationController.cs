using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.Dashboard;
using Quantis.WorkFlow.Services.DTOs.Widgets;

namespace Quantis.Workflow.Complete.Controllers.Widgets
{
    public class KPICountByOrganizationController : BaseWidgetController
    {
        private IGlobalFilterService _globalfilterService;
        private IWidgetService _widgetService;

        public KPICountByOrganizationController(IGlobalFilterService globalfilterService, IWidgetService widgetService)
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
            vm.ChartTypes.Add(ChartType.BAR);
            vm.ChartTypes.Add(ChartType.LINE);
            vm.AggregationOptions.Add(AggregationOption.KPI);
            vm.AggregationOptions.Add(AggregationOption.CONTRACT);
            vm.AggregationOptions.Add(AggregationOption.CONTRACTPARTY);
        }

        internal override object GetData(WidgetParametersDTO props)
        {
            var dto = _globalfilterService.MapAggOptionWidget(props);
            var result = _widgetService.GetKPICountByOrganization(dto);
            return result;
        }
    }
}