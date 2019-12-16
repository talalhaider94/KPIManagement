using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class EmailNotifierDTO
    {
        public int id { get; set; }
        public string type { get; set; }
        public string user_domain { get; set; }
        public string period { get; set; }
        public DateTime notify_date { get; set; }
        public string email_body { get; set; }
        public string form_name { get; set; }
        public string recipient { get; set; }
    }
}