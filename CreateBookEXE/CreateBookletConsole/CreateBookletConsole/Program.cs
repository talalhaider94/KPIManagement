using System;
using System.Threading;
using System.Threading.Tasks;
using BoC.g8almgr2.Class5;
using Newtonsoft.Json;

namespace CreateBookletConsole
{
    class Program
    {
        private static volatile int currentNumber = 1;
        static int Main(string[] args)
        {
            string path = args[0];
            string text=System.IO.File.ReadAllText(path);
            var dto=JsonConvert.DeserializeObject<CreateBookletDTO>(text);
            var class5 = new Class5(dto.MainPath);
            int totalNumbers = dto.BookletContracts.Count;
            string requestId = DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss");
            foreach (var key in dto.BookletContracts)
            {
                try
                {
                    var intKey = int.Parse(key.ContractVersionId);
                    var value = key.ContractName;
                    class5.CreateSingleBooklet(intKey, value, dto.UserId, key.BookletDocumentId, dto.MailSetup, currentNumber, totalNumbers, requestId);
                }
                catch (Exception e)
                {

                }
                finally
                {
                    currentNumber++;
                }
            }
            return 1;

        }
    }
}
