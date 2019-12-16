using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models.Dashboard;
using Quantis.WorkFlow.Services.DTOs.Dashboard;
using System.Collections.Generic;

namespace Quantis.WorkFlow.APIBase.Mappers.Dashboard
{
    public class DashboardWidgetMapper : MappingService<DashboardWidgetDTO, DB_DashboardWidget>
    {
        public override DashboardWidgetDTO GetDTO(DB_DashboardWidget e)
        {
            var dto = new DashboardWidgetDTO()
            {
                DashboardId = e.DashboardId,
                Id = e.Id,
                LocationX = e.LocationX,
                LocationY = e.LocationY,
                SizeX = e.SizeX,
                SizeY = e.SizeY,
                WidgetId = e.WidgetId,
                WidgetName = e.WidgetName ?? e.Widget.Name,
                UIIdentifier = e.Widget.UIIdentifier,
                Note = e.Note
            };
            dto.Properties = new Dictionary<string, string>();
            dto.Filters = new Dictionary<string, string>();
            foreach (var val in e.DashboardWidgetSettings)
            {
                if (val.SettingType == 0)
                {
                    dto.Properties.Add(val.SettingKey, val.SettingValue);
                }
                else
                {
                    dto.Filters.Add(val.SettingKey, val.SettingValue);
                }
            }
            return dto;
        }

        public override DB_DashboardWidget GetEntity(DashboardWidgetDTO o, DB_DashboardWidget e)
        {
            e.LocationX = o.LocationX;
            e.LocationY = o.LocationY;
            e.SizeX = o.SizeX;
            e.SizeY = o.SizeY;
            e.WidgetName = o.WidgetName;
            e.Note = o.Note;
            if (e.Id == 0)
            {
                e.WidgetId = o.WidgetId;
                e.DashboardId = e.DashboardId;
            }
            return e;
        }
    }
}