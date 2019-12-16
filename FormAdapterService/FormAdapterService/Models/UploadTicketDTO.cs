using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormAdapterService.Models
{
    public class UploadTicketDTO
    {
        public string url { get; set; }
        public string action { get; set; }
        public string sid { get; set; }
        public string repositoryHandle { get; set; }
        public string objectHandle { get; set; }
        public string description { get; set; }
        public string fileName { get; set; }
        public byte[] fileData { get; set; }
    }
}