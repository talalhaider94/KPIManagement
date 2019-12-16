using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.Monitoring
{
    public class MonitoringDayLevelDTO
    {
        public int DayNumber { get; set; }
        public int NoOfTicketsToBeOpenedToday { get; set; }
        public List<MonitoringKPIDTO> TicketsToBeOpenedToday { get; set; }
        public int NoOfTicketsOpenedToday { get; set; }
        public List<MonitoringTicketDTO> TicketsOpenedToday { get; set; }
    }
}
