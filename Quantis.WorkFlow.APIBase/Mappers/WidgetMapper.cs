using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models;
using Quantis.WorkFlow.Services.DTOs.API;

namespace Quantis.WorkFlow.APIBase.Mappers
{
    public class WidgetMapper : MappingService<WidgetDTO, T_Widget>
    {
        public override WidgetDTO GetDTO(T_Widget e)
        {
            return new WidgetDTO()
            {
                chart_description = e.chart_description,
                chart_description_see = e.chart_description_see,
                chart_option = e.chart_option,
                chart_type = e.chart_type,
                create_date = e.create_date,
                delete_date = e.delete_date,
                modify_date = e.modify_date,
                page_id = e.page_id,
                user_id = e.user_id,
                widget_configuration = e.widget_configuration,
                widget_data = e.widget_data,
                widget_id = e.widget_id
            };
        }

        public override T_Widget GetEntity(WidgetDTO o, T_Widget e)
        {
            e.chart_description = o.chart_description;
            e.chart_description_see = o.chart_description_see;
            e.chart_option = o.chart_option;
            e.chart_type = o.chart_type;
            e.create_date = o.create_date;
            e.delete_date = o.delete_date;
            e.modify_date = o.modify_date;
            e.page_id = o.page_id;
            e.user_id = o.user_id;
            e.widget_configuration = o.widget_configuration;
            e.widget_data = o.widget_data;
            return e;
        }
    }
}