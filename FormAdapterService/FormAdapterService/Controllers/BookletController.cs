using EventForm.bcfed9e1.Class5;
using FormAdapterService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

namespace FormAdapterService.Controllers
{
    public class BookletController : ApiController
    {
        [HttpPost]
        public string CreateBooklet([FromBody]CreateBookletDTO dto)
        {
            try
            {
                if (!dto.BookletContracts.Any())
                {
                    return "1";
                }
                if (!Directory.Exists(Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "BookletParams")))
                {
                    Directory.CreateDirectory(Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "BookletParams"));
                }

                var fileName = dto.BookletContracts.FirstOrDefault().BookletDocumentId + DateTime.Now.Ticks;
                var path = Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "BookletParams", fileName);
                var text = JsonConvert.SerializeObject(dto);
                System.IO.File.WriteAllText(path, text);
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "Bin", "CreateBookletConsole.exe");
                    proc.StartInfo.Arguments = path;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.Start();
                    //proc.WaitForExit();
                    //return proc.ExitCode+"";
                    return "1";
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
            
        }

        [HttpGet]
        public string CreateBookletSample()
        {
            if (!Directory.Exists(Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "BookletParams")))
            {
                Directory.CreateDirectory(Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "BookletParams"));
            }
            int returnMainRun = 0;
            //List<string> mailSetup = new List<string>() { "smtp.quantis.it", "crm@quantis.it", "7u6ndYMW", "crm@quantis.it", "marti@quantis.it" };
            List<string> mailSetup = new List<string>() { "asmtp.mail.hostpoint.ch", "git_cloud@quantis.it", "yYC3HcWs", "git_cloud@quantis.it", "shahzad744@gmail.com" };
            var listaContratti = new Dictionary<string, string>();
            listaContratti.Add("4971", "Assegni (SLA) (Mew)");
            listaContratti.Add("5980", "Centro Nazionale Assegni (SLA) (Mew)");
            int userId = 0;
            var MainPath = @"e:\booklet";
            Int32.TryParse("2095".Trim(), out userId);
            // bookletDocumentId = "1105".Trim();
            var bookletDocumentId = "1080".Trim();
            Class5 createbooklet = new Class5(MainPath);
            returnMainRun = createbooklet.CreateBooklet(listaContratti, userId, bookletDocumentId, mailSetup);
            return returnMainRun+"";
        }
    }
}
