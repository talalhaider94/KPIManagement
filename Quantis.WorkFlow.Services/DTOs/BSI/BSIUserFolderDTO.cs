using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.BSI
{
    public class BSIUserFolderDTO
    {
        public string UserName { get; set; }
        public int ReportID { get; set; }
        public string ReportName { get; set; }
        public int UserId { get; set; }
        public string IsMyFolder { get; set; }

    }

}
