using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class CreateBookletWebServiceDTO
    {
        public string MainPath { get; set; }
        public List<CreateBookletWebBaseDTO> BookletContracts { get; set; }
        public int UserId { get; set; }
        public List<string> MailSetup { get; set; }
    }
}
