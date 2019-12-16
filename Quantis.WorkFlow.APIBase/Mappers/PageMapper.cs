using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models;
using Quantis.WorkFlow.Services.DTOs.API;

namespace Quantis.WorkFlow.APIBase.Mappers
{
    public class PageMapper : MappingService<PageDTO, T_Page>
    {
        public override PageDTO GetDTO(T_Page e)
        {
            return new PageDTO()
            {
                page_id = e.page_id,
                page_name = e.page_name,
                page_sequence = e.page_sequence,
                user_id = e.user_id
            };
        }

        public override T_Page GetEntity(PageDTO o, T_Page e)
        {
            e.page_name = o.page_name;
            e.page_sequence = o.page_sequence;
            e.user_id = o.user_id;
            return e;
        }
    }
}