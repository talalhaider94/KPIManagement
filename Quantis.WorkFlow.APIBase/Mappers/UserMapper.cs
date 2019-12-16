using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models;
using Quantis.WorkFlow.Services.DTOs.API;

namespace Quantis.WorkFlow.APIBase.Mappers
{
    public class UserMapper : MappingService<UserDTO, T_CatalogUser>
    {
        public override UserDTO GetDTO(T_CatalogUser e)
        {
            return new UserDTO()
            {
                id = e.id,
                ca_bsi_account = e.ca_bsi_account,
                ca_bsi_user_id = e.ca_bsi_user_id,
                name = e.name,
                surname = e.surname,
                organization = e.organization,
                mail = e.mail,
                userid = e.userid,
                manager = e.manager,
                password = e.password
            };
        }

        public override T_CatalogUser GetEntity(UserDTO o, T_CatalogUser e)
        {
            e.ca_bsi_account = o.ca_bsi_account;
            if (e.id == 0)
            {
                e.ca_bsi_user_id = o.ca_bsi_user_id;
            }
            e.name = o.name;
            e.surname = o.surname;
            e.organization = o.organization;
            e.mail = o.mail;
            e.userid = o.userid;
            e.manager = o.manager;
            if (e.id == 0)
            {
                e.password = o.password;
            }
            return e;
        }
    }
}