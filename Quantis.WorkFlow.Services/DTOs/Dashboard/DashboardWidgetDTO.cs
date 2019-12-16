namespace Quantis.WorkFlow.Services.DTOs.Dashboard
{
    public class DashboardWidgetDTO : DashboardWidgetBaseDTO
    {
        public int WidgetId { get; set; }
        public int DashboardId { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public string WidgetName { get; set; }
        public string UIIdentifier { get; set; }
        public string Note { get; set; }
    }
}