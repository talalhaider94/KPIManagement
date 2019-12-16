using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models;
using Quantis.WorkFlow.Services.DTOs.API;

namespace Quantis.WorkFlow.APIBase.Mappers
{
    public class FormUsersMapper : MappingService<FormUsersDTO, T_FormUsers>
    {
        public override FormUsersDTO GetDTO(T_FormUsers e)
        {
            return new FormUsersDTO()
            {
                id = e.id,
                form_id = e.form_id,
                //form_body = e.form_body,
                //end_date = e.end_date,
                //start_date = e.start_date
            };
        }

        public override T_FormUsers GetEntity(FormUsersDTO o, T_FormUsers e)
        {
            e.form_id = o.form_id;
            //e.form_body = o.form_body;
            //e.end_date = o.end_date;
            //e.start_date = o.start_date;
            return e;
        }
    }
}