using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class CreateBookletDTO
    {
        public List<CreateBookletBaseDTO> BookletContracts { get; set; }
        public string RecipientEmail { get; set; }
    }
}
