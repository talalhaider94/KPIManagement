using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.OracleAPI
{
    public class BookletLVDTO
    {
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public string TemplateName { get; set; }
        public int ContractContext { get; set; }
        public string ContractName { get; set; }
    }
}
