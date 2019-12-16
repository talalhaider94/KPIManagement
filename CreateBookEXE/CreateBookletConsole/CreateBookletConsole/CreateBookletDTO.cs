using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateBookletConsole
{
    public class CreateBookletDTO
    {
        public string MainPath { get; set; }
        public List<CreateBookletWebBaseDTO> BookletContracts { get; set; }
        public int UserId { get; set; }
        public List<string> MailSetup { get; set; }
    }
}
