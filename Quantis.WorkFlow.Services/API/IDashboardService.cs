using Quantis.WorkFlow.Services.DTOs.Dashboard;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.API
{
    public interface IDashboardService
    {
        List<DashboardDTO> GetDashboards(int userId);

        int AddUpdateDasboard(DashboardDetailDTO dto, int userId);

        List<WidgetDTO> GetAllWidgets();

        DashboardDetailDTO GetDashboardWigetsByDashboardId(int id);

        void SaveDashboardState(List<DashboardWidgetBaseDTO> dtos);

        void ActivateDashboard(int id);

        void DeactivateDashboard(int id);

        void SetDefaultDashboard(int id, int userId);

        int GetDefaultDashboardId(int userId);
        void DeleteDashboard(int Id);
    }
}