using System;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class ArchiveKPIDTO
    {
        public string kpi_name { get; set; }
        public DateTime kpi_interval { get; set; }
        public int kpi_value { get; set; }
        public int ticket_id { get; set; }
        public DateTime ticket_close_timestamp { get; set; }
        public bool isarchived { get; set; }
        public List<int> raw_data_ids { get; set; }
    }
}