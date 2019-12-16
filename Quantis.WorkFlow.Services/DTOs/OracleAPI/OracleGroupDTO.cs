using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.OracleAPI
{
    public class OracleGroupDTO
    {
        public int user_group_id { get; set; }
        public string user_group_name { get; set; }
        public List<OracleGroupUserDTO> users { get; set; }
    }

    public class OracleGroupUserDTO
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_email { get; set; }
    }
}