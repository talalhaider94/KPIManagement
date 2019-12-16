using System;

namespace Quantis.WorkFlow.Services.DTOs.BSI
{
    public class BSIReportLVDTO
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int UserId { get; set; }
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public bool IsParameterized { get; set; }
        public string ReportType { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsExecutable { get; set; }
    }
}