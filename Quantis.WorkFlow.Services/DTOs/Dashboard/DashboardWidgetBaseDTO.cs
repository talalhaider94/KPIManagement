using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.Dashboard
{
    public class DashboardWidgetBaseDTO
    {
        public int Id { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public Dictionary<string, string> Filters { get; set; }
    }
}