using EventForm.bcfed9e1.Class3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FormAdapterService.Controllers
{
    public class OracleConController : ApiController
    {
        public Dictionary<string,string> GetOracleConnection()
        {
            Dictionary<string, string> ConnectionData = new Dictionary<string, string>();
            Class3 class3 = new Class3();
            ConnectionData = class3.GetdbHandler();
            return ConnectionData;
        }
    }
}
