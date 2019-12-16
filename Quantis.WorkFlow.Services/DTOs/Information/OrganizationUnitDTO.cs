namespace Quantis.WorkFlow.Services.DTOs.Information
{
    public class OrganizationUnitDTO
    {
        public int id { get; set; }
        public string organization_unit { get; set; }
        public int? workflow_day { get; set; }
        public int? cutoff_day { get; set; }
        public int? sla_id { get; set; }
    }
}