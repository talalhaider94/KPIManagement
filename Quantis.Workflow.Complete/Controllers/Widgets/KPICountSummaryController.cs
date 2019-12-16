using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.Dashboard;
using Quantis.WorkFlow.Services.DTOs.Widgets;

namespace Quantis.Workflow.Complete.Controllers.Widgets
{
    public class KPICountSummaryController : BaseWidgetController
    {
        private IGlobalFilterService _globalfilterService;
        private IWidgetService _widgetService;

        public KPICountSummaryController(IGlobalFilterService globalfilterService, IWidgetService widgetService)
        {
            _globalfilterService = globalfilterService;
            _widgetService = widgetService;
        }

        internal override void FillWidgetParameters(WidgetViewModel vm)
        {
            vm.DefaultDateRange = _globalfilterService.GetDefualtDateRange();
            vm.ShowMeasure = true;
            vm.ShowDateType = true;
            vm.ShowDateFilter = true;
            //vm.ShowIncompletePeriodCheck = true;
            vm.AddMeasure(Measures.Number_of_Total_KPI_in_verifica);
            vm.AddMeasure(Measures.Number_of_Total_KPI_compliant);
            vm.AddMeasure(Measures.Number_of_Total_KPI_not_compliant);
            vm.AddMeasure(Measures.Number_of_Total_KPI_non_calcolato);
            vm.DateTypes.Clear();
            vm.DateTypes.Add(0, "Intervallo");
            vm.DateTypes.Add(1, "Ultimo mese");
        }

        internal override object GetData(WidgetParametersDTO props)
        {
            var dto = _globalfilterService.MapBaseWidget(props);
            var result = _widgetService.GetKPICountSummary(dto);
            return result;
        }
    }
}