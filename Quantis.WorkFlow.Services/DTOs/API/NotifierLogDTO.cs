using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class NotifierLogDTO
    {
        public int id_form { get; set; }
        public DateTime notify_timestamp { get; set; }
        public DateTime? remind_timestamp { get; set; }
        public bool is_ack { get; set; }
        public string period { get; set; }
        public int year { get; set; }
        public string email_body { get; set; }
    }
}