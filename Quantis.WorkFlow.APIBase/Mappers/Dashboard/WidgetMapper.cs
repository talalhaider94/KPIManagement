using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models.Dashboard;
using Quantis.WorkFlow.Services.DTOs.Dashboard;
using System;

namespace Quantis.WorkFlow.APIBase.Mappers.Dashboard
{
    public class WidgetMapper : MappingService<WidgetDTO, DB_Widget>
    {
        public override WidgetDTO GetDTO(DB_Widget e)
        {
            return new WidgetDTO()
            {
                CreatedOn = e.CreatedOn,
                Name = e.Name,
                Id = e.Id,
                Help = e.Help,
                IconURL = e.IconURL,
                URL = e.URL,
                WidgetCategoryName = e.WidgetCategory.Name,
                WigetCategoryId = e.WidgetCategoryId,
                UIIdentifier = e.UIIdentifier
            };
        }

        public override DB_Widget GetEntity(WidgetDTO o, DB_Widget e)
        {
            throw new NotImplementedException();
        }
    }
}