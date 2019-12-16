using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormAdapterService.Models
{
    public class CreateBookletWebBaseDTO
    {
        public string ContractVersionId { get; set; }
        public string ContractName { get; set; }
        public string BookletDocumentId { get; set; }
    }
}