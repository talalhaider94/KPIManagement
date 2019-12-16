using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models.SDM;
using Quantis.WorkFlow.Services.DTOs.Information;

namespace Quantis.WorkFlow.APIBase.Mappers.Information
{
    public class SDMGroupMapper : MappingService<SDMGroupDTO, SDM_TicketGroup>
    {
        public override SDMGroupDTO GetDTO(SDM_TicketGroup e)
        {
            return new SDMGroupDTO()
            {
                name = e.name,
                handle = e.handle,
                id = e.id,
                step = e.step,
                category_id = e.category_id,
                category_name = e.category.customer_name
            };
        }

        public override SDM_TicketGroup GetEntity(SDMGroupDTO o, SDM_TicketGroup e)
        {
            e.name = o.name;
            e.handle = o.handle;
            e.step = o.step;
            e.category_id = o.category_id;
            return e;
        }
    }
}