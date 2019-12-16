using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class CreateBookletWebBaseDTO
    {
        public string ContractVersionId { get; set; }
        public string ContractName { get; set; }
        public string BookletDocumentId { get; set; }
    }
}
