using ParKPI.bcf56sn9.Class4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FormAdapterService.Controllers
{
    public class KPIRegistrationController : ApiController
    {
        public List<KpiRegistrationDTO> GetKPIRegistrations(int ruleId)
        {
            KpiRegistration kpiregistration = new KpiRegistration();
            return kpiregistration.loadRegistration(ruleId);
        }
    }
}
