using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class FormRuleDTO
    {
        public int id { get; set; }
        public int form_id { get; set; }
        public string form_body { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
    }
}