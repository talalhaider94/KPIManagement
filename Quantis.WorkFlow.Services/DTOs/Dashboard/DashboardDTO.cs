using System;

namespace Quantis.WorkFlow.Services.DTOs.Dashboard
{
    public class DashboardDTO : BaseIdNameDTO
    {
        public string Owner { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}