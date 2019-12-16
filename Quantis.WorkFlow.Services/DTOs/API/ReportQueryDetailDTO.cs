using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class ReportQueryDetailDTO
    {
        public int Id { get; set; }
        public string QueryName { get; set; }
        public string QueryText { get; set; }
        public int OwnerId { get; set; }
        public string FolderName { get; set; }
        public string FolderOwnerName { get; set; }
        public List<KeyValuePairDTO> Parameters { get; set; }
    }
}