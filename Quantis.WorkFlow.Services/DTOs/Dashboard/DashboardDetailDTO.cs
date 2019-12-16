using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.Dashboard
{
    public class DashboardDetailDTO : BaseIdNameDTO
    {
        public int? GlobalFilterId { get; set; }
        public List<DashboardWidgetDTO> DashboardWidgets { get; set; }
    }
}