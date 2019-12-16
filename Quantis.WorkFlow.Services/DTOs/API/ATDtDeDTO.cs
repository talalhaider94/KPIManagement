using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class ATDtDeDTO
    {
        public int created_by { get; set; }
        public int event_type_id { get; set; }
        public DateTime reader_time_stamp { get; set; }
        public int resource_id { get; set; }
        public DateTime time_stamp { get; set; }
        public string data_source_id { get; set; }
        public int raw_data_id { get; set; }
        public DateTime create_date { get; set; }
        public int corrected_by { get; set; }
        public string data { get; set; }
        public DateTime modify_date { get; set; }
        public int reader_id { get; set; }
        public string event_source_type_id { get; set; }
        public int event_state_id { get; set; }
        public int partner_raw_data_id { get; set; }
        public string hash_data_key { get; set; }
        public string id_kpi { get; set; }
    }
}