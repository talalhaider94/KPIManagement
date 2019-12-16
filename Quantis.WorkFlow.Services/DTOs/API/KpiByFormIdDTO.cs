namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class KPIOnlyContractDTO
    {
        public string id_kpi { get; set; }
        public string contract { get; set; }
        public int global_rule_id { get; set; }
        public string kpi_name_bsi { get; set; }
        public string target { get; set; }
    }
}