namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class BSIKPIUploadDTO
    {
        public string primary_contract_party { get; set; }
        public string secondary_contract_party { get; set; }
        public string contract_name { get; set; }
        public string kpi_name { get; set; }
        public string id_ticket { get; set; }
        public string period { get; set; }
        public string ticket_status { get; set; }
    }
}