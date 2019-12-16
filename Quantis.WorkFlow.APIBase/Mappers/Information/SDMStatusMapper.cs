using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models.SDM;
using Quantis.WorkFlow.Services.DTOs.Information;

namespace Quantis.WorkFlow.APIBase.Mappers.Information
{
    public class SDMStatusMapper : MappingService<SDMStatusDTO, SDM_TicketStatus>
    {
        public override SDMStatusDTO GetDTO(SDM_TicketStatus e)
        {
            return new SDMStatusDTO()
            {
                name = e.name,
                handle = e.handle,
                id = e.id,
                step = e.step,
                code = e.code
            };
        }

        public override SDM_TicketStatus GetEntity(SDMStatusDTO o, SDM_TicketStatus e)
        {
            e.name = o.name;
            e.handle = o.handle;
            e.step = o.step;
            e.code = o.code;
            return e;
        }
    }
}