using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models;
using Quantis.WorkFlow.Services.DTOs.API;

namespace Quantis.WorkFlow.APIBase.Mappers
{
    public class GroupMapper : MappingService<GroupDTO, T_Group>
    {
        public override GroupDTO GetDTO(T_Group e)
        {
            return new GroupDTO()
            {
                group_id = e.group_id,
                create_date = e.create_date,
                delete_date = e.delete_date,
                group_description = e.group_description,
                modify_date = e.modify_date
            };
        }

        public override T_Group GetEntity(GroupDTO o, T_Group e)
        {
            e.group_description = o.group_description;
            e.modify_date = o.modify_date;
            e.delete_date = o.delete_date;
            e.create_date = o.create_date;
            return e;
        }
    }
}