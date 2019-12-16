using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class FormDetialsDTO
    {
        public int form_id { get; set; }
        public int attachment_count { get; set; }
        public DateTime latest_modified_date { get; set; }
    }
}