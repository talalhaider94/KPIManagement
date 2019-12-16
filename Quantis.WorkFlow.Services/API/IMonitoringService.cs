using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Quantis.WorkFlow.Services.DTOs.Monitoring;

namespace Quantis.WorkFlow.Services.API
{
    public interface IMonitoringService
    {
        List<MonitoringDTO> GetTicketsMonitoringByPeriod(string period);
        List<MonitoringDayLevelDTO> GetDayLevelTicketsMonitoring(string period);
        DataTable ExecuteLocalDatabase(ExecuteLocalDatabaseDTO dto);
    }
}
