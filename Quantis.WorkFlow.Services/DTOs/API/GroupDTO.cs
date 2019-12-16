using System;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class GroupDTO
    {
        public int group_id { get; set; }
        public string group_description { get; set; }
        public DateTime create_date { get; set; }
        public DateTime? modify_date { get; set; }
        public DateTime? delete_date { get; set; }
    }
}