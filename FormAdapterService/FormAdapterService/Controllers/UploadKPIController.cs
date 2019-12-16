using FormAdapterService.Models;
using ParKPI.bcf56sn9.Class1;
using ParKPI.bcf56sn9.Class4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FormAdapterService.Controllers
{
    public class UploadKPIController : ApiController
    {
        [HttpGet]
        public string UploadKPISample()
        {
            List<string> args = new List<string>() { "PostePay", "" ,"CARTE CRED - TELEPASS", "1 - Attività specifiche ed essenziali per la corretta esecuzione delle operazioni di pagamento - Processi operativi (Cumulato)" ,"IDticket12349" ,"0319", "Closed&Certified" };
            try
            {
                Class1 loader = new Class1(args.ToArray());

                KpiComponent kpiComponent = new KpiComponent();
                kpiComponent.loadStatus(loader.getKpiTable());
                return "TRUE";
            }
            catch (Exception e)
            {
               return e.Message;
            }
        }
        [HttpPost]
        public string UploadKPIPost([FromBody]UploadKPIDTO dto)
        {
            if (dto.arguments.Count < 7)
            {
                return "Arguments are less then 7";
            }
            try
            {
                Class1 loader = new Class1(dto.arguments.ToArray());

                KpiComponent kpiComponent = new KpiComponent();
                kpiComponent.loadStatus(loader.getKpiTable());
                return "TRUE";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
