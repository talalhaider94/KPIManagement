using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.BSI
{
    public class BSIReportMainDTO
    {
        public string Name { get; set; }
        public string ResultType { get; set; }
        public List<BSIReportDetailDTO> Reports { get; set; }
    }
}