namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class ApiDetailsDTO
    {
        public int api_id { get; set; }
        public string server_address { get; set; }
        public string api_method { get; set; }
        public string return_type { get; set; }
        public string parameters { get; set; }
        public string details { get; set; }
        public string api_type { get; set; }
        public string db_source { get; set; }
        public string table_used { get; set; }
    }
}