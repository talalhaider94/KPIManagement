using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class FormAttachmentDTO
    {
        public int form_attachment_id { get; set; }
        public int form_id { get; set; }
        public byte[] content { get; set; }
        public string period { get; set; }
        public int year { get; set; }
        public string doc_name { get; set; }
        public string checksum { get; set; }
        public DateTime create_date { get; set; }
    }
}