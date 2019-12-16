using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models;
using Quantis.WorkFlow.Services.DTOs.API;

namespace Quantis.WorkFlow.APIBase.Mappers
{
    public class ApiMapper : MappingService<ApiDetailsDTO, T_APIDetail>
    {
        public override ApiDetailsDTO GetDTO(T_APIDetail e)
        {
            return new ApiDetailsDTO()
            {
                api_id = e.api_id,
                server_address = e.server_address,
                api_method = e.api_method,
                return_type = e.return_type,
                parameters = e.parameters,
                details = e.details,
                api_type = e.api_type,
                db_source = e.db_source,
                table_used = e.table_used
            };
        }

        public override T_APIDetail GetEntity(ApiDetailsDTO o, T_APIDetail e)
        {
            e.server_address = o.server_address;
            e.api_method = o.api_method;
            e.return_type = o.return_type;
            e.parameters = o.parameters;
            e.details = o.details;
            e.api_type = o.api_type;
            e.db_source = o.db_source;
            e.table_used = o.table_used;
            return e;
        }
    }
}