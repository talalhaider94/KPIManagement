using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.Dashboard
{
    public class WidgetParametersDTO
    {
        public int GlobalFilterId { get; set; }
        public int UserId { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public Dictionary<string, string> Filters { get; set; }
    }
}