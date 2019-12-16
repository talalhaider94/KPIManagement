using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class TUserDTO
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public int user_locale_id { get; set; }
        public string user_status { get; set; }
        public string user_email { get; set; }
        public string user_organization_name { get; set; }
        public DateTime user_create_date { get; set; }
    }
}