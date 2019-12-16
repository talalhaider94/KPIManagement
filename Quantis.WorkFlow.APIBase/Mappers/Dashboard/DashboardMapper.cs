using System;
using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models.Dashboard;
using Quantis.WorkFlow.Services.DTOs.Dashboard;

namespace Quantis.WorkFlow.APIBase.Mappers.Dashboard
{
    public class DashboardMapper : MappingService<DashboardDTO, DB_Dashboard>
    {
        public override DashboardDTO GetDTO(DB_Dashboard e)
        {
            return new DashboardDTO()
            {
                CreatedOn = e.CreatedOn,
                Id = e.Id,
                Name = e.Name,
                Owner = e.User.user_name,
                IsActive = e.IsActive,
                IsDefault = e.IsDefault,
                ModifiedOn = e.ModifiedOn
            };
        }

        public override DB_Dashboard GetEntity(DashboardDTO o, DB_Dashboard e)
        {
            e.Name = o.Name;
            e.ModifiedOn=DateTime.Now;
            return e;
        }
    }
}