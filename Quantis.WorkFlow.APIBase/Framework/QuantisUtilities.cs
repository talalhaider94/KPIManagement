using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Quantis.WorkFlow.APIBase.Framework
{
    public static class QuantisUtilities
    {
        public static Tuple<string, string> FixHttpURLForCall(string basePath, string apiPath)
        {
            if (Regex.Matches(basePath, "/").Count > 2)
            {
                var index = basePath.LastIndexOf('/');
                var newbasepath = basePath.Substring(0, index);
                var subpath = basePath.Substring(index);
                basePath = newbasepath;
                apiPath = subpath + apiPath;
            }
            return new Tuple<string, string>(basePath, apiPath);
        }

        public static string GetOracleConnectionString(WorkFlowPostgreSqlContext _dbcontext)
        {
            try
            {
                Dictionary<string, string> config = null;
                var bsiconf = _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_bsi" && o.key == "bsi_api_url");
                var oracleconf = _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_oracle" && o.key == "con_str");
                if (bsiconf == null || oracleconf == null)
                {
                    var e = new Exception("Configuration of BSI or Oracle does not exist");
                    throw e;
                }
                using (var client = new HttpClient())
                {
                    string basePath = bsiconf.value;
                    string apiPath = "/api/OracleCon/GetOracleConnection";
                    var output = QuantisUtilities.FixHttpURLForCall(basePath, apiPath);
                    client.BaseAddress = new Uri(output.Item1);
                    var response = client.GetAsync(output.Item2).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        config = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        var e = new Exception(string.Format("Connection to retrieve Orcle credentials cannot be created: basePath: {0} apipath: {1}", basePath, apiPath));
                        throw e;
                    }
                }
                string finalconfig = string.Format(oracleconf.value, config["datasource"], config["username"], config["password"]);
                //string finalconfig = string.Format(oracleconf.value, "oblicore", "oblicore", "oblicore");
                return finalconfig;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string GetOracleGlobalRuleInQuery(string variable, List<int> kpis)
        {
            var queries = new List<string>();
            int range = 990;
            int counter = 0;
            int currentindex = kpis.Count;
            while (currentindex > 0)
            {
                var currentkpis = kpis.Skip(counter * range).Take(range);
                queries.Add(string.Format(" {0} in ({1})", variable, string.Join(',', currentkpis)));
                currentindex = currentindex - range;
                counter++;
            }
            var result = string.Format("( {0} )", string.Join(" OR ", queries));
            return result;
        }
    }
}