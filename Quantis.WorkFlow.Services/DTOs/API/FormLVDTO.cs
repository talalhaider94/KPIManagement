using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class FormLVDTO
    {
        public int form_id { get; set; }
        public string form_name { get; set; }
        public string form_description { get; set; }
        public int reader_id { get; set; }
        public int form_owner_id { get; set; }
        public string day_cuttoff { get; set; }
        public int rules_count { get; set; }
        public DateTime create_date { get; set; }
        public DateTime modify_date { get; set; }
        public DateTime latest_input_date { get; set; }
    }
}