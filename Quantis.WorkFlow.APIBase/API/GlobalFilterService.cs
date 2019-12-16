using Microsoft.Extensions.Configuration;
using Npgsql;
using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.Dashboard;
using Quantis.WorkFlow.Services.DTOs.Information;
using Quantis.WorkFlow.Services.DTOs.Widgets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Quantis.WorkFlow.APIBase.API
{
    public class GlobalFilterService : IGlobalFilterService
    {
        private readonly IInformationService _infoService;
        private readonly WorkFlowPostgreSqlContext _dbcontext;
        private readonly IConfiguration _configuration;
        private string defaultDateRange = "06/2019-08/2019";

        public GlobalFilterService(IInformationService infoService, WorkFlowPostgreSqlContext dbcontext, IConfiguration configuration)
        {
            _infoService = infoService;
            _dbcontext = dbcontext;
            _configuration = configuration;
            var val = _infoService.GetConfiguration("defaultdaterange", "dashboard");
            if (val != null)
            {
                defaultDateRange = val.Value;
            }
        }

        public string GetDefualtDateRange()
        {
            return defaultDateRange;
        }

        public BaseWidgetDTO MapBaseWidget(WidgetParametersDTO props)
        {
            var dto = new BaseWidgetDTO();
            if (props.Filters.ContainsKey("organizations"))
            {
                dto.KPIs = GetGlobalRuleIds(props.UserId, props.Filters["organizations"]);
            }
            else
            {
                dto.KPIs = new List<int>() { -1 };
            }
            if (props.Filters.ContainsKey("kpi"))
            {
                dto.KPI = int.Parse(props.Filters["kpi"]);
            }
            else
            {
                dto.KPI = -1;
            }
            if (props.Filters.ContainsKey("daterange"))
            {
                var daterange = props.Filters["daterange"];
                var range = daterange.Split('-');
                dto.DateRange = new Tuple<DateTime, DateTime>(DateTime.Parse(range[0]), DateTime.Parse(range[1]));
            }
            else
            {
                var daterange = defaultDateRange;
                var range = daterange.Split('-');
                dto.DateRange = new Tuple<DateTime, DateTime>(DateTime.Parse(range[0]), DateTime.Parse(range[1]));
            }
            if (props.Filters.ContainsKey("date"))
            {
                var range = props.Filters["date"];
                dto.Date = DateTime.ParseExact(range, "MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                dto.Date = DateTime.Now.AddMonths(-1);
            }
            if (props.Properties.ContainsKey("measure"))
            {
                dto.Measures = new List<Measures>() { (Measures)Int32.Parse(props.Properties["measure"]) };
            }
            else
            {
                dto.Measures = new List<Measures>();
            }

            if (props.Properties.ContainsKey("incompleteperiod"))
            {
                dto.IncompletePeriod = (props.Properties["incompleteperiod"] == "true");
            }
            else
            {
                dto.IncompletePeriod = false;
            }

            return dto;
        }

        public WidgetwithAggOptionDTO MapAggOptionWidget(WidgetParametersDTO props)
        {
            var map = MapBaseWidget(props);
            var dto = new WidgetwithAggOptionDTO()
            {
                DateRange = map.DateRange,
                KPIs = map.KPIs,
                Measures = map.Measures,
                Date = map.Date,
                KPI = map.KPI
            };
            if (props.Properties.ContainsKey("aggregationoption"))
            {
                dto.AggregationOption = props.Properties["aggregationoption"];
            }
            else
            {
                dto.AggregationOption = "";
            }
            return dto;
        }

        private List<int> GetGlobalRuleIds(int userId, string customerIds)
        {
            var res = new List<int>();
            string query = @"select r.global_rule_id
                            from t_rules r
                            left join t_sla_versions s on r.sla_version_id = s.sla_version_id
                            left join t_slas m on m.sla_id = s.sla_id
                            left join t_user_kpis uk on r.global_rule_id = uk.global_rule_id
                            left join t_catalog_kpis ck on ck.global_rule_id_bsi = r.global_rule_id
                            where s.sla_status = 'EFFECTIVE' AND m.sla_status = 'EFFECTIVE'
                            and ck.enable=true
                            and uk.user_id =  :user_id
                            and m.sla_id in ({0})";
            query = string.Format(query, customerIds);
            using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
            {
                con.Open();
                var command = new NpgsqlCommand(query, con);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue(":user_id", userId);
                command.CommandText = query;
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        res.Add(Decimal.ToInt32((Decimal)result[0]));
                    }
                }
            }
            return res;
        }

        public List<HierarchicalNameCodeDTO> GetOrganizationHierarcy(int globalFilterId, int userId)
        {
            try
            {
                var res = new List<UserKPIDTO>();
                string query = @"select m.sla_id,m.sla_name,c.customer_name,c.customer_id
                                from t_sla_versions s
                                left join t_slas m on m.sla_id = s.sla_id
                                left join t_customers c on m.customer_id = c.customer_id
                                left join t_rules r on r.sla_version_id = s.sla_version_id
                                left join t_user_kpis uk on r.global_rule_id = uk.global_rule_id
                                where s.sla_status = 'EFFECTIVE'
                                AND m.sla_status = 'EFFECTIVE'
                                and uk.user_id = :user_id
                                group by m.sla_id,m.sla_name,c.customer_name,c.customer_id ";
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                {
                    con.Open();
                    var command = new NpgsqlCommand(query, con);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue(":user_id", userId);
                    command.CommandText = query;
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            res.Add(new UserKPIDTO()
                            {
                                Sla_Id = Decimal.ToInt32((Decimal)result[0]),
                                Sla_Name = (string)result[1],
                                Customer_name = (string)result[2],
                                Customer_Id = (int)result[3],
                            });
                        }
                    }
                    var ret = res.GroupBy(o => new { o.Customer_name, o.Customer_Id }).Select(p => new HierarchicalNameCodeDTO(p.Key.Customer_Id, p.Key.Customer_name, p.Key.Customer_name)
                    {
                        Children = p.Select(q => new HierarchicalNameCodeDTO(q.Sla_Id, q.Sla_Name, q.Sla_Name)).ToList()
                    }).ToList();
                    return ret;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<KeyValuePair<int, string>> GetContractParties(int globalFilterId, int userId)
        {
            try
            {
                var res = new List<KeyValuePair<int, string>>();
                string query = @"select c.customer_name,c.customer_id
                                from t_rules r
                                left join t_sla_versions s on r.sla_version_id = s.sla_version_id
                                left join t_slas m on m.sla_id = s.sla_id
                                left join t_customers c on m.customer_id = c.customer_id
                                left join t_user_kpis uk on r.global_rule_id = uk.global_rule_id
                                where s.sla_status = 'EFFECTIVE' AND m.sla_status = 'EFFECTIVE'
                                and uk.user_id = :user_id
                                group by c.customer_name,c.customer_id";
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                {
                    con.Open();
                    var command = new NpgsqlCommand(query, con);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue(":user_id", userId);
                    command.CommandText = query;
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            res.Add(new KeyValuePair<int, string>((int)result[1], (string)result[0]));
                        }
                    }
                    return res;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<KeyValuePair<int, string>> GetContracts(int globalFilterId, int userId, int contractpartyId)
        {
            try
            {
                var res = new List<KeyValuePair<int, string>>();
                string query = @"select m.sla_id,m.sla_name
                                from t_rules r
                                left join t_sla_versions s on r.sla_version_id = s.sla_version_id
                                left join t_slas m on m.sla_id = s.sla_id
                                left join t_customers c on m.customer_id = c.customer_id
                                left join t_user_kpis uk on r.global_rule_id = uk.global_rule_id
                                where s.sla_status = 'EFFECTIVE' AND m.sla_status = 'EFFECTIVE'
                                and uk.user_id = :user_id
                                and c.customer_id= :customer_id
                                group by m.sla_id,m.sla_name";
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                {
                    con.Open();
                    var command = new NpgsqlCommand(query, con);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue(":user_id", userId);
                    command.Parameters.AddWithValue(":customer_id", contractpartyId);
                    command.CommandText = query;
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            res.Add(new KeyValuePair<int, string>(Decimal.ToInt32((Decimal)result[0]), (string)result[1]));
                        }
                    }
                    return res;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<KeyValuePair<int, string>> GetKPIs(int globalFilterId, int userId, int contractId)
        {
            try
            {
                var res = new List<KeyValuePair<int, string>>();
                string query = @"select r.global_rule_id,r.rule_name
                                from t_rules r
                                left join t_sla_versions s on r.sla_version_id = s.sla_version_id
                                left join t_slas m on m.sla_id = s.sla_id
                                left join t_customers c on m.customer_id = c.customer_id
                                left join t_user_kpis uk on r.global_rule_id = uk.global_rule_id
                                left join t_catalog_kpis ck on ck.global_rule_id_bsi = r.global_rule_id
                                where s.sla_status = 'EFFECTIVE' AND m.sla_status = 'EFFECTIVE'
                                and ck.enable=true
                                and uk.user_id = :user_id
                                and m.sla_id=:sla_id
                                group by r.rule_name, r.global_rule_id";
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                {
                    con.Open();
                    var command = new NpgsqlCommand(query, con);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue(":user_id", userId);
                    command.Parameters.AddWithValue(":sla_id", contractId);
                    command.CommandText = query;
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            res.Add(new KeyValuePair<int, string>(Decimal.ToInt32((Decimal)result[0]), (string)result[1]));
                        }
                    }
                    return res;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}