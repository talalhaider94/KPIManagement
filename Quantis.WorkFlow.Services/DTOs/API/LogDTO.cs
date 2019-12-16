using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class LogDTO
    {
        public int id { get; set; }
        public string message { get; set; }
        public string innerexceptions { get; set; }
        public string stacktrace { get; set; }
        public string loglevel { get; set; }
        public DateTime ex_timestamp { get; set; }
    }
}