using Oracle.ManagedDataAccess.Client;
using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.OracleAPI;
using Quantis.WorkFlow.Services.DTOs.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantis.WorkFlow.APIBase.API
{
    public class WidgetService : IWidgetService
    {
        private readonly WorkFlowPostgreSqlContext _dbcontext;
        private IDataService _dataService;
        private static string _connectionstring = null;

        public WidgetService(WorkFlowPostgreSqlContext dbcontext, IDataService dataService)
        {
            _dataService = dataService;
            _dbcontext = dbcontext;
            if (_connectionstring == null)
            {
                _connectionstring = QuantisUtilities.GetOracleConnectionString(_dbcontext);
            }
        }

        public List<XYDTO> GetKPICountTrend(WidgetwithAggOptionDTO dto)
        {
            var result = new List<XYDTO>();
            if (dto.Measures.FirstOrDefault() == Measures.Number_of_ticket_in_KPI_in_Verifica)
            {
                var facts = _dbcontext.SDMTicketFact.Where(o => o.period_month >= dto.DateRange.Item1.Month && o.period_year >= dto.DateRange.Item1.Year && o.period_month <= dto.DateRange.Item2.Month && o.period_year <= dto.DateRange.Item2.Year && dto.KPIs.Contains(o.global_rule_id));
                if (dto.AggregationOption == AggregationOption.ANNAUL.Key)
                {
                    result = facts.GroupBy(o => o.period_year).Select(p => new XYDTO()
                    {
                        XValue = p.Key + "",
                        YValue = p.Count(r => (p.Key == r.period_year ? true : false))  //was p.Count() @SHAHZAD pls check this, it was not working "exception near AS"
                    }).OrderBy(o => o.XValue).ToList();
                }
                else
                {
                    result = facts.GroupBy(o => new { o.period_year, o.period_month }).Select(p => new XYDTO()
                    {
                        XValue = p.Key.period_month + "/" + p.Key.period_year,
                        YValue = p.Count()
                    }).ToList();
                }
            }
            else if (dto.Measures.FirstOrDefault() == Measures.Number_of_ticket_of_KPI_Compliant)
            {
                var facts = _dbcontext.SDMTicketFact.Where(o => o.period_month >= dto.DateRange.Item1.Month && o.period_year >= dto.DateRange.Item1.Year && o.period_month <= dto.DateRange.Item2.Month && o.period_year <= dto.DateRange.Item2.Year && dto.KPIs.Contains(o.global_rule_id));
                if (dto.AggregationOption == AggregationOption.ANNAUL.Key)
                {
                    result = facts.GroupBy(o => o.period_year).Select(p => new XYDTO()
                    {
                        XValue = p.Key + "",
                        YValue = p.Count(r => r.complaint)
                    }).OrderBy(o => o.XValue).ToList();
                }
                else
                {
                    result = facts.GroupBy(o => new { o.period_year, o.period_month }).Select(p => new XYDTO()
                    {
                        XValue = p.Key.period_month + "/" + p.Key.period_year,
                        YValue = p.Count(r => r.complaint)
                    }).ToList();
                }
            }
            else if (dto.Measures.FirstOrDefault() == Measures.Number_of_ticket_of_KPI_Non_Calcolato)
            {
                var facts = _dbcontext.SDMTicketFact.Where(o => o.period_month >= dto.DateRange.Item1.Month && o.period_year >= dto.DateRange.Item1.Year && o.period_month <= dto.DateRange.Item2.Month && o.period_year <= dto.DateRange.Item2.Year && dto.KPIs.Contains(o.global_rule_id));
                if (dto.AggregationOption == AggregationOption.ANNAUL.Key)
                {
                    result = facts.GroupBy(o => o.period_year).Select(p => new XYDTO()
                    {
                        XValue = p.Key + "",
                        YValue = p.Count(r => r.notcalculated)
                    }).OrderBy(o => o.XValue).ToList();
                }
                else
                {
                    result = facts.GroupBy(o => new { o.period_year, o.period_month }).Select(p => new XYDTO()
                    {
                        XValue = p.Key.period_month + "/" + p.Key.period_year,
                        YValue = p.Count(r => r.notcalculated)
                    }).ToList();
                }
            }
            else if (dto.Measures.FirstOrDefault() == Measures.Number_of_ticket_of_KPI_Non_Compliant)
            {
                var facts = _dbcontext.SDMTicketFact.Where(o => o.period_month >= dto.DateRange.Item1.Month && o.period_year >= dto.DateRange.Item1.Year && o.period_month <= dto.DateRange.Item2.Month && o.period_year <= dto.DateRange.Item2.Year && dto.KPIs.Contains(o.global_rule_id));
                if (dto.AggregationOption == AggregationOption.ANNAUL.Key)
                {
                    result = facts.GroupBy(o => o.period_year).Select(p => new XYDTO()
                    {
                        XValue = p.Key + "",
                        YValue = p.Count(r => r.notcomplaint)
                    }).OrderBy(o => o.XValue).ToList();
                }
                else
                {
                    result = facts.GroupBy(o => new { o.period_year, o.period_month }).Select(p => new XYDTO()
                    {
                        XValue = p.Key.period_month + "/" + p.Key.period_year,
                        YValue = p.Count(r => r.notcomplaint)
                    }).ToList();
                }
            }
            else if (dto.Measures.FirstOrDefault() == Measures.Number_of_Total_KPI_compliant || dto.Measures.FirstOrDefault() == Measures.Number_of_Total_KPI_not_compliant)
            {
                string signcomplaint = "<";
                if (dto.Measures.FirstOrDefault() == Measures.Number_of_Total_KPI_not_compliant)
                {
                    signcomplaint = ">";
                }
                string query = @"select
                                psl.end_period,
                                count(1)
                                from
                                (
                                  select
                                  temp.global_rule_id,
                                  temp.time_stamp as timestamp,
                                  temp.time_stamp_utc as end_period,
                                  temp.begin_time_stamp_utc as start_period,
                                  temp.sla_id,
                                  temp.rule_id,
                                  temp.time_unit,
                                  temp.interval_length,
                                  temp.is_period,
                                  temp.service_level_target,
                                  temp.service_level_target_ce,
                                  temp.provided_ce,
                                  temp.deviation_ce,
                                  temp.complete_record,
                                  temp.sla_version_id,
                                  temp.metric_type_id,
                                  temp.domain_category_id,
                                  temp.service_domain_id,
                                  case
                                    when deviation_ce > 0 then 'non compliant'
                                    when deviation_ce < 0 then 'compliant'
                                    else 'nc'
                                  end as resultPsl
                                  from
                                  (
                                    select *
                                    from t_psl_0_month pm
                                    union all
                                    select *
                                    from t_psl_0_quarter pq
                                    union all
                                    select *
                                    from t_psl_0_year py
                                    union all
                                    select *
                                    from t_psl_1_all pa
                                  ) temp
                                  where provided is not null
                                  and service_level_target is not null
                                ) psl
                                left join t_rules r on  psl.rule_id = r.rule_id
                                left join t_sla_versions sv on r.sla_version_id = sv.sla_version_id
                                left join t_slas s on sv.sla_id = s.sla_id
                                where r.is_effective = 'Y' AND s.sla_status = 'EFFECTIVE'
                                and psl.time_unit='MONTH'
                                and psl.complete_record=1
                                and TRUNC(psl.start_period) >= TO_DATE(:start_period,'yyyy-mm-dd')
                                and TRUNC(psl.end_period) <= TO_DATE(:end_period,'yyyy-mm-dd')
                                and {0}
                                and psl.deviation_ce {1} 0
                                group by psl.end_period";
                var rules = QuantisUtilities.GetOracleGlobalRuleInQuery("psl.global_rule_id", dto.KPIs);
                query = string.Format(query, rules, signcomplaint);
                using (OracleConnection con = new OracleConnection(_connectionstring))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = query;
                        OracleParameter param1 = new OracleParameter("start_period", dto.DateRange.Item1.AddDays(-1).ToString("yyyy-MM-dd"));
                        OracleParameter param2 = new OracleParameter("end_period", dto.DateRange.Item2.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            result.Add(new XYDTO()
                            {
                                XValue = ((DateTime)reader[0]).ToString("MM/yy"),
                                YValue = Decimal.ToDouble((Decimal)reader[1])
                            });
                        }
                    }
                }
                if (dto.AggregationOption == AggregationOption.ANNAUL.Key)
                {
                    result = result.GroupBy(o => o.XValue.Split('/')[1]).Select(p => new XYDTO() { XValue = p.Key, YValue = p.Sum(q => q.YValue) }).OrderBy(o=>o.XValue).ToList();
                }
                if (dto.AggregationOption == AggregationOption.PERIOD.Key)
                {
                    result = result.GroupBy(o => o.XValue).Select(p => new XYDTO() { XValue = p.Key, YValue = p.Sum(q => q.YValue) }).OrderBy(o => o.XValue).ToList();
                }
            }
            return result;
        }

        public XYDTO GetCatalogPendingCount(BaseWidgetDTO dto)
        {
            if (dto.Measures.FirstOrDefault() == Measures.Pending_KPIs)
            {
                return new XYDTO() { XValue = "", YValue = _dataService.GetAllTRules().Count() };
            }
            if (dto.Measures.FirstOrDefault() == Measures.Pending_Users)
            {
                return new XYDTO() { XValue = "", YValue = _dataService.GetAllTUsers().Count() };
            }
            return null;
        }

        public List<XYDTO> GetDistributionByVerifica(BaseWidgetDTO dto)
        {
            var facts = _dbcontext.SDMTicketFact.Where(o => o.period_month == dto.Date.Month && o.period_year >= dto.Date.Year);
            if (dto.KPIs.Any())
            {
                facts = facts.Where(o => dto.KPIs.Contains(o.global_rule_id));
            }

            var res = new List<XYDTO>();

            res.Add(new XYDTO() { XValue = "Compliant", YValue = facts.Where(o => o.complaint).Count() });
            res.Add(new XYDTO() { XValue = "Non Compliant", YValue = facts.Where(o => o.notcomplaint).Count() });
            res.Add(new XYDTO() { XValue = "NE", YValue = facts.Where(o => o.notcalculated).Count() });
            return res;
        }

        public List<XYZDTO> GetKPICountByOrganization(WidgetwithAggOptionDTO dto)
        {
            try
            {
                var result = new List<XYZDTO>();
                var basedtos = new List<LandingPageBaseDTO>();
                if (!dto.KPIs.Any())
                {
                    return null;
                }
                string query = @"select
                                c.customer_id,
                                c.customer_name,
                                s.sla_id,
                                s.sla_name,
                                gr.global_rule_name,
                                gr.global_rule_id,
                                psl.service_level_target_ce,
                                psl.provided_ce,
                                psl.resultpsl,
                                psl.deviation_ce
                                from
                                (
                                  select
                                  temp.global_rule_id,
                                  temp.time_stamp as timestamp,
                                  temp.time_stamp_utc as end_period,
                                  temp.begin_time_stamp_utc as start_period,
                                  temp.sla_id,
                                  temp.rule_id,
                                  temp.time_unit,
                                  temp.interval_length,
                                  temp.is_period,
                                  temp.service_level_target,
                                  temp.service_level_target_ce,
                                  temp.provided_ce,
                                  temp.deviation_ce,
                                  temp.complete_record,
                                  temp.sla_version_id,
                                  temp.metric_type_id,
                                  temp.domain_category_id,
                                  temp.service_domain_id,
                                  case
                                    when deviation_ce > 0 then 'non compliant'
                                    when deviation_ce < 0 then 'compliant'
                                    else 'nc'
                                  end as resultPsl
                                  from
                                  (
                                    select *
                                    from t_psl_0_month pm
                                    union all
                                    select *
                                    from t_psl_0_quarter pq
                                    union all
                                    select *
                                    from t_psl_0_year py
                                    union all
                                    select *
                                    from t_psl_1_all pa
                                  ) temp
                                  where provided is not null
                                  and service_level_target is not null
                                ) psl
                                left join t_rules r on  psl.rule_id = r.rule_id
                                left join T_GLOBAL_RULES gr on psl.global_rule_id = gr.global_rule_id
                                left join t_sla_versions sv on r.sla_version_id = sv.sla_version_id
                                left join t_slas s on sv.sla_id = s.sla_id
                                left join t_customers c on s.customer_id = c.customer_id
                                where r.is_effective = 'Y' AND s.sla_status = 'EFFECTIVE'
                                and psl.time_unit='MONTH'
                                and psl.complete_record=1
                                and TRUNC(psl.start_period) >= TO_DATE(:start_period,'yyyy-mm-dd')
                                and TRUNC(psl.end_period) <= TO_DATE(:end_period,'yyyy-mm-dd')
                                and {0}";
                var rules = QuantisUtilities.GetOracleGlobalRuleInQuery("psl.global_rule_id", dto.KPIs);
                query = string.Format(query, rules);
                using (OracleConnection con = new OracleConnection(_connectionstring))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = query;
                        OracleParameter param1 = new OracleParameter("start_period", dto.DateRange.Item1.AddDays(-1).ToString("yyyy-MM-dd"));
                        OracleParameter param2 = new OracleParameter("end_period", dto.DateRange.Item2.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            basedtos.Add(new LandingPageBaseDTO()
                            {
                                ContractPartyId = Decimal.ToInt32((Decimal)reader[0]),
                                ContractPartyName = (string)reader[1],
                                ContractId = Decimal.ToInt32((Decimal)reader[2]),
                                ContractName = (string)reader[3],
                                GlobalRuleName = (string)reader[4],
                                GlobalRuleId = Decimal.ToInt32((Decimal)reader[5]),
                                Target = (reader[6] == DBNull.Value) ? 0 : (double)reader[6],
                                Actual = (reader[7] == DBNull.Value) ? 0 : (double)reader[7],
                                Result = (string)reader[8],
                                Deviation = (reader[9] == DBNull.Value) ? 0 : (double)reader[9],
                            });
                        }
                    }
                }
                if (dto.AggregationOption == AggregationOption.KPI.Key)
                {
                    result.AddRange(basedtos.Where(o => o.Result == "compliant").GroupBy(o => new { o.GlobalRuleId, o.GlobalRuleName }).Select(o => new XYZDTO()
                    {
                        XValue = o.Key.GlobalRuleName,
                        YValue = o.Count(),
                        ZValue = "Compliant"
                    }));
                    result.AddRange(basedtos.Where(o => o.Result != "compliant").GroupBy(o => new { o.GlobalRuleId, o.GlobalRuleName }).Select(o => new XYZDTO()
                    {
                        XValue = o.Key.GlobalRuleName,
                        YValue = o.Count(),
                        ZValue = "Non Compliant"
                    }));
                }
                if (dto.AggregationOption == AggregationOption.CONTRACT.Key)
                {
                    result.AddRange(basedtos.Where(o => o.Result == "compliant").GroupBy(o => new { o.ContractId, o.ContractName }).Select(o => new XYZDTO()
                    {
                        XValue = o.Key.ContractName,
                        YValue = o.Count(),
                        ZValue = "Compliant"
                    }));
                    result.AddRange(basedtos.Where(o => o.Result != "compliant").GroupBy(o => new { o.ContractId, o.ContractName }).Select(o => new XYZDTO()
                    {
                        XValue = o.Key.ContractName,
                        YValue = o.Count(),
                        ZValue = "Non Compliant"
                    }));
                }
                if (dto.AggregationOption == AggregationOption.CONTRACTPARTY.Key)
                {
                    result.AddRange(basedtos.Where(o => o.Result == "compliant").GroupBy(o => new { o.ContractPartyId, o.ContractPartyName }).Select(o => new XYZDTO()
                    {
                        XValue = o.Key.ContractPartyName,
                        YValue = o.Count(),
                        ZValue = "Compliant"
                    }));
                    result.AddRange(basedtos.Where(o => o.Result != "compliant").GroupBy(o => new { o.ContractPartyId, o.ContractPartyName }).Select(o => new XYZDTO()
                    {
                        XValue = o.Key.ContractPartyName,
                        YValue = o.Count(),
                        ZValue = "Non Compliant"
                    }));
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public XYDTO GetKPICountSummary(BaseWidgetDTO dto)
        {
            var result = new XYDTO();

            string signcomplaint = "";
            if (dto.Measures.FirstOrDefault() == Measures.Number_of_Total_KPI_compliant)
            {
                signcomplaint = "and psl.deviation_ce < 0";
            }
            if (dto.Measures.FirstOrDefault() == Measures.Number_of_Total_KPI_not_compliant)
            {
                signcomplaint = "and psl.deviation_ce > 0";
            }
            if (dto.Measures.FirstOrDefault() == Measures.Number_of_Total_KPI_non_calcolato)
            {
                signcomplaint = "and psl.deviation_ce = 0";
            }
            string query = @"select
                                psl.end_period,
                                count(1)
                                from
                                (
                                  select
                                  temp.global_rule_id,
                                  temp.time_stamp as timestamp,
                                  temp.time_stamp_utc as end_period,
                                  temp.begin_time_stamp_utc as start_period,
                                  temp.sla_id,
                                  temp.rule_id,
                                  temp.time_unit,
                                  temp.interval_length,
                                  temp.is_period,
                                  temp.service_level_target,
                                  temp.service_level_target_ce,
                                  temp.provided_ce,
                                  temp.deviation_ce,
                                  temp.complete_record,
                                  temp.sla_version_id,
                                  temp.metric_type_id,
                                  temp.domain_category_id,
                                  temp.service_domain_id,
                                  case
                                    when deviation_ce > 0 then 'non compliant'
                                    when deviation_ce < 0 then 'compliant'
                                    else 'nc'
                                  end as resultPsl
                                  from
                                  (
                                    select *
                                    from t_psl_0_month pm
                                    union all
                                    select *
                                    from t_psl_0_quarter pq
                                    union all
                                    select *
                                    from t_psl_0_year py
                                    union all
                                    select *
                                    from t_psl_1_all pa
                                  ) temp
                                  where provided is not null
                                  and service_level_target is not null
                                ) psl
                                left join t_rules r on  psl.rule_id = r.rule_id
                                left join t_sla_versions sv on r.sla_version_id = sv.sla_version_id
                                left join t_slas s on sv.sla_id = s.sla_id
                                where r.is_effective = 'Y' AND s.sla_status = 'EFFECTIVE'
                                and psl.time_unit='MONTH'
                                and psl.complete_record=1
                                and TRUNC(psl.start_period) >= TO_DATE(:start_period,'yyyy-mm-dd')
                                and TRUNC(psl.end_period) <= TO_DATE(:end_period,'yyyy-mm-dd')
                                and {0}
                                {1}
                                group by psl.end_period";
            var rules = QuantisUtilities.GetOracleGlobalRuleInQuery("psl.global_rule_id", dto.KPIs);
            query = string.Format(query, rules, signcomplaint);
            using (OracleConnection con = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = query;
                    OracleParameter param1 = new OracleParameter("start_period", dto.Date.AddDays(-1).ToString("yyyy-MM-dd"));
                    OracleParameter param2 = new OracleParameter("end_period", dto.Date.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result = new XYDTO()
                        {
                            XValue = ((DateTime)reader[0]).ToString("MM/yy"),
                            YValue = Decimal.ToDouble((Decimal)reader[1])
                        };
                    }
                    return result;
                }
            }
        }

        public List<XYDTO> GetNotificationTrend(WidgetwithAggOptionDTO dto)
        {
            var res = new List<XYDTO>();
            res.Add(new XYDTO() { XValue = "04/2019", YValue = 26 });
            res.Add(new XYDTO() { XValue = "05/2019", YValue = 20 });
            res.Add(new XYDTO() { XValue = "06/2019", YValue = 25 });
            res.Add(new XYDTO() { XValue = "07/2019", YValue = 22 });
            return res;
        }

        public List<XYZDTO> GetKPIReportTrend(WidgetwithAggOptionDTO dto,int completeRecord=1)
        {
            var result = new List<XYZDTO>();
            string periodstring = $"(psl.time_unit='MONTH' and psl.complete_record={completeRecord})";
            if (dto.AggregationOption == AggregationOption.TRACKINGPERIOD.Key)
            {
                periodstring = $"((psl.time_unit='MONTH' and psl.complete_record={completeRecord}) or (psl.time_unit='QUARTER' and psl.complete_record={completeRecord}) or (psl.time_unit='YEAR' and psl.complete_record={completeRecord}))";
            }
                string query = @"select
                            psl.end_period,
                            psl.service_level_target_ce,
                            psl.provided_ce,
                            psl.resultpsl,
                            u.unit_symbol
                            from
                            (
                              select
                              temp.global_rule_id,
                              temp.time_stamp as timestamp,
                              temp.time_stamp_utc as end_period,
                              temp.begin_time_stamp_utc as start_period,
                              temp.sla_id,
                              temp.rule_id,
                              temp.time_unit,
                              temp.interval_length,
                              temp.is_period,
                              temp.service_level_target,
                              temp.service_level_target_ce,
                              temp.provided_ce,
                              temp.deviation_ce,
                              temp.complete_record,
                              temp.sla_version_id,
                              temp.metric_type_id,
                              temp.domain_category_id,
                              temp.service_domain_id,
                              case
                                when deviation_ce > 0 then 'non compliant'
                                when deviation_ce < 0 then 'compliant'
                                else 'nc'
                              end as resultPsl
                              from
                              (
                                select *
                                from t_psl_0_month pm
                                union all
                                select *
                                from t_psl_0_quarter pq
                                union all
                                select *
                                from t_psl_0_year py
                                union all
                                select *
                                from t_psl_1_all pa
                              ) temp
                              where provided is not null
                              and service_level_target is not null
                            ) psl
                            left join t_rules r on  psl.rule_id = r.rule_id
                            left join t_domain_categories d on r.domain_category_id = d.domain_category_id
                            left join t_units u on d.unit_id = u.unit_id
                            left join T_GLOBAL_RULES gr on psl.global_rule_id = gr.global_rule_id
                            left join t_sla_versions sv on r.sla_version_id = sv.sla_version_id
                            left join t_slas s on sv.sla_id = s.sla_id
                            where r.is_effective = 'Y' AND s.sla_status = 'EFFECTIVE'
                            and {0}
                            and TRUNC(psl.start_period) >= TO_DATE(:start_period,'yyyy-mm-dd')
                            and TRUNC(psl.end_period) <= TO_DATE(:end_period,'yyyy-mm-dd')
                            and psl.global_rule_id =:global_rule_id";
            query = string.Format(query, periodstring);
            using (OracleConnection con = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = query;
                    OracleParameter param1 = new OracleParameter("start_period", dto.DateRange.Item1.AddDays(-1).ToString("yyyy-MM-dd"));
                    OracleParameter param2 = new OracleParameter("end_period", dto.DateRange.Item2.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
                    OracleParameter param3 = new OracleParameter("global_rule_id", dto.KPI);
                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(new XYZDTO()
                        {
                            XValue = ((DateTime)reader[0]).ToString("MM/yy"),
                            YValue = (double)reader[1],
                            Description = (string)reader[3] + "|" + (string)reader[4],
                            ZValue = "Target"
                        });
                        result.Add(new XYZDTO()
                        {
                            XValue = ((DateTime)reader[0]).ToString("MM/yy"),
                            YValue = (double)reader[2],
                            Description = (string)reader[3] + "|" + (string)reader[4],
                            ZValue = "Value"
                        });
                    }
                }
            }
            return result;
        }

        public List<KPIStatusSummaryDTO> GetKPIStatusSummary(BaseWidgetDTO dto)
        {
            if (!dto.KPIs.Any())
            {
                return new List<KPIStatusSummaryDTO>();
            }
            var result = new List<KPIStatusSummaryDTO>();
            string query = @"select contract_party, contract, global_rule_id, ""ID KPI"",replace(replace(replace(replace(""DESCRIZIONE KPI"",'(Non Cumulato)'),'(Cumulato)'),'(Non cumulato)'),'(Progressivo)') as ""DESCRIZIONE KPI"",tipologia,
                            frequenza,calcolo,
                            fornitura,escalation,""VALORE LIMITE ATTESO"",
                            case when fornitura = 'C' then mas.valore
                            when fornitura = 'NC' then med.media end as TREND,
                            CASE WHEN Gen = 'ND' THEN '<font color=""red"">'
                            WHEN Gen = 'NA' THEN '<font color=""black"">'
                            WHEN Gen = 'NE' THEN '<font color=""black"">'
                            else (case
                            WHEN relation = 'NLT' and Gen<target THEN '<font color=""red"">'
                                 WHEN relation = 'NMT' and Gen > target THEN '<font color=""red"">'
                                 ELSE '<font color=""green"">' END)END || CASE WHEN SUBSTR(Gen,1,1) = '.' THEN '0' ELSE '' END || Gen || '</font>' as Gen,
                            CASE WHEN Feb = 'ND' THEN '<font color=""red"">'
                            WHEN Feb = 'NA' THEN '<font color=""black"">'
                            WHEN Feb = 'NE' THEN '<font color=""black"">'
                            else (case
                            WHEN relation = 'NLT' and Feb<target THEN '<font color=""red"">'
                                 WHEN relation = 'NMT' and Feb > target THEN '<font color=""red"">'
                                 ELSE '<font color=""green"">' END)END || CASE WHEN SUBSTR(Feb,1,1) = '.' THEN '0' ELSE '' END || Feb || '</font>' as Feb,
                            CASE WHEN Mar = 'ND' THEN '<font color=""red"">'
                            WHEN Mar = 'NA' THEN '<font color=""black"">'
                            WHEN Mar = 'NE' THEN '<font color=""black"">'
                            else (case
                            WHEN relation = 'NLT' and Mar<target THEN '<font color=""red"">'
                                 WHEN relation = 'NMT' and Mar > target THEN '<font color=""red"">'
                                 ELSE '<font color=""green"">' END)END || CASE WHEN SUBSTR(Mar,1,1) = '.' THEN '0' ELSE '' END || Mar || '</font>' as Mar,
                            CASE WHEN Apr = 'ND' THEN '<font color=""red"">'
                            WHEN Apr = 'NA' THEN '<font color=""black"">'
                            WHEN Apr = 'NE' THEN '<font color=""black"">'
                            else (case
                            WHEN relation = 'NLT' and Apr<target THEN '<font color=""red"">'
                                 WHEN relation = 'NMT' and Apr > target THEN '<font color=""red"">'
                                 ELSE '<font color=""green"">' END)END || CASE WHEN SUBSTR(Apr,1,1) = '.' THEN '0' ELSE '' END || Apr || '</font>' as Apr,
                            CASE WHEN Mag = 'ND' THEN '<font color=""red"">'
                            WHEN Mag = 'NA' THEN '<font color=""black"">'
                            WHEN Mag = 'NE' THEN '<font color=""black"">'
                            else (case
                            WHEN relation = 'NLT' and Mag<target THEN '<font color=""red"">'
                                 WHEN relation = 'NMT' and Mag > target THEN '<font color=""red"">'
                                 ELSE '<font color=""green"">' END)END || CASE WHEN SUBSTR(Mag,1,1) = '.' THEN '0' ELSE '' END || Mag || '</font>' as Mag,
                            CASE WHEN Giu = 'ND' THEN '<font color=""red"">'
                            WHEN Giu = 'NA' THEN '<font color=""black"">'
                            WHEN Giu = 'NE' THEN '<font color=""black"">'
                            else (case
                            WHEN relation = 'NLT' and Giu<target THEN '<font color=""red"">'
                                 WHEN relation = 'NMT' and Giu > target THEN '<font color=""red"">'
                                 ELSE '<font color=""green"">' END)END || CASE WHEN SUBSTR(Giu,1,1) = '.' THEN '0' ELSE '' END || Giu || '</font>' as Giu,
                            CASE WHEN Lug = 'ND' THEN '<font color=""red"">'
                            WHEN Lug = 'NA' THEN '<font color=""black"">'
                            WHEN Lug = 'NE' THEN '<font color=""black"">'
                            else (case
                            WHEN relation = 'NLT' and Lug<target THEN '<font color=""red"">'
                                 WHEN relation = 'NMT' and Lug > target THEN '<font color=""red"">'
                                 ELSE '<font color=""green"">' END)END || CASE WHEN SUBSTR(Lug,1,1) = '.' THEN '0' ELSE '' END || Lug || '</font>' as Lug,
                            CASE WHEN Ago = 'ND' THEN '<font color=""red"">'
                            WHEN Ago = 'NA' THEN '<font color=""black"">'
                            WHEN Ago = 'NE' THEN '<font color=""black"">'
                            else (case
                            WHEN relation = 'NLT' and Ago<target THEN '<font color=""red"">'
                                 WHEN relation = 'NMT' and Ago > target THEN '<font color=""red"">'
                                 ELSE '<font color=""green"">' END)END || CASE WHEN SUBSTR(Ago,1,1) = '.' THEN '0' ELSE '' END || Ago || '</font>' as Ago,
                            CASE WHEN Sep = 'ND' THEN '<font color=""red"">'
                            WHEN Sep = 'NA' THEN '<font color=""black"">'
                            WHEN Sep = 'NE' THEN '<font color=""black"">'
                            else (case
                            WHEN relation = 'NLT' and Sep<target THEN '<font color=""red"">'
                                 WHEN relation = 'NMT' and Sep > target THEN '<font color=""red"">'
                                 ELSE '<font color=""green"">' END)END || CASE WHEN SUBSTR(Sep,1,1) = '.' THEN '0' ELSE '' END || Sep || '</font>' as Sep,
                            CASE WHEN Ott = 'ND' THEN '<font color=""red"">'
                            WHEN Ott = 'NA' THEN '<font color=""black"">'
                            WHEN Ott = 'NE' THEN '<font color=""black"">'
                            else (case
                            WHEN relation = 'NLT' and Ott<target THEN '<font color=""red"">'
                                 WHEN relation = 'NMT' and Ott > target THEN '<font color=""red"">'
                                 ELSE '<font color=""green"">' END)END || CASE WHEN SUBSTR(Ott,1,1) = '.' THEN '0' ELSE '' END || Ott || '</font>' as Ott,
                            CASE WHEN Nov = 'ND' THEN '<font color=""red"">'
                            WHEN Nov = 'NA' THEN '<font color=""black"">'
                            WHEN Nov = 'NE' THEN '<font color=""black"">'
                            else (case
                            WHEN relation = 'NLT' and Nov<target THEN '<font color=""red"">'
                                 WHEN relation = 'NMT' and Nov > target THEN '<font color=""red"">'
                                 ELSE '<font color=""green"">' END)END || CASE WHEN SUBSTR(Nov,1,1) = '.' THEN '0' ELSE '' END || Nov || '</font>' as Nov,
                            CASE WHEN Dic = 'ND' THEN '<font color=""red"">'
                            WHEN Dic = 'NA' THEN '<font color=""black"">'
                            WHEN Dic = 'NE' THEN '<font color=""black"">'
                            else (case
                            WHEN relation = 'NLT' and Dic<target THEN '<font color=""red"">'
                                 WHEN relation = 'NMT' and Dic > target THEN '<font color=""red"">'
                                 ELSE '<font color=""green"">' END)END || CASE WHEN SUBSTR(Dic,1,1) = '.' THEN '0' ELSE '' END || Dic || '</font>' as Dic
                            from(
                            select distinct replace(substr(note, 1, 4), '*', '') as ""ID KPI"", contract_party, contract, global_rule_id, descrizione_kpi as ""DESCRIZIONE KPI"",
                            case when substr(note, 14, 1) = 'M' then 'M' else 'A' end calcolo,
                            substr(note, 45, 1) as tipologia,
                            case when replace(substr(note, 1,4),'*','') in (0015, 0010, 009) then '6M' else frequenza end frequenza,
                            case when(descrizione_kpi like '%BPSIN083%'
                            or descrizione_kpi like '%BPSIN082%'
                            or descrizione_kpi like '%BPSIN081%'
                            or descrizione_kpi like '%BPSIN080%'
                            or descrizione_kpi like '%BPSIN079%'
                            or descrizione_kpi like '%BPSIN078%'
                            or descrizione_kpi like '%BPSIN077%'
                            or descrizione_kpi like '%BPSIN076%'
                            or descrizione_kpi like '%BPSIN075%'
                            or descrizione_kpi like '%BPSIN074%'
                            or descrizione_kpi like '%BPSIN073%'
                            or descrizione_kpi like '%BPSIN072%'
                            or descrizione_kpi like '%BPSIN071%'
                            or descrizione_kpi like '%Report Riep%') then 'NC' else 'C' end fornitura,
                            replace(replace(replace(replace(substr(note, 46, 12), '*', ''), 'M', '>'), 'm', '<'), '<in', 'min') as escalation,
                            replace(replace(replace(replace(substr(note, 58, 12), '*', ''), 'M', '>'), 'm', '<'), '<in', 'min') as ""VALORE LIMITE ATTESO"",
                            relation, target,
                            case when Gen = -999 then 'NE'
                            when frequenza not in '1M' then 'NA'
                            when frequenza = '1M' and Gen is null then 'ND'
                            else TO_CHAR(Gen) end Gen,
                            case
                            when Feb = -999 then 'NE'
                            when(frequenza = '1M' and Feb is null
                            ) then 'ND'
                            when(to_char(sysdate, 'MM') < 3 and to_char(sysdate, 'YYYY') = anno)  then null
                            when frequenza not in '1M' then 'NA'
                            else TO_CHAR(Feb)
                            end Feb,
                            case when
                            Mar = -999 then 'NE'
                            when to_char(sysdate, 'MM') < 4 and to_char(sysdate, 'YYYY') = anno then null
                            when frequenza not in ('1M', '3M') then 'NA'
                            when frequenza in ('1M', '3M') and Mar is null then 'ND'
                            else TO_CHAR(Mar) end Mar,
                            case when Apr = -999 then 'NE'
                            when to_char(sysdate, 'MM') < 5 and to_char(sysdate, 'YYYY') = anno then null
                            when frequenza not in '1M' then 'NA'
                            when frequenza = '1M' and Apr is null then 'ND'
                            else TO_CHAR(Apr) end Apr,
                            case when Mag = -999 then 'NE'
                            when to_char(sysdate, 'MM') < 6 and to_char(sysdate, 'YYYY') = anno then null
                            when frequenza not in '1M' then 'NA'
                            when frequenza = '1M' and Mag is null then 'ND'
                            else TO_CHAR(Mag) end Mag,
                            case when Giu = -999 then 'NE'
                            when to_char(sysdate, 'MM') < 7 and to_char(sysdate, 'YYYY') = anno then null
                            when frequenza not in ('1M', '3M', '6M') then 'NA'
                            when frequenza in ('1M', '3M', '6M') and Giu is null then 'ND'
                            else TO_CHAR(Giu) end Giu,
                            case when Lug = -999 then 'NE'
                            when to_char(sysdate, 'MM') < 8 and to_char(sysdate, 'YYYY') = anno then null
                            when frequenza not in '1M' then 'NA'
                            when frequenza = '1M' and Lug is null then 'ND'
                            else TO_CHAR(Lug) end Lug,
                            case when Ago = -999 then 'NE'
                            when to_char(sysdate, 'MM') < 9 and to_char(sysdate, 'YYYY') = anno then null
                            when frequenza not in '1M' then 'NA'
                            when frequenza = '1M' and Ago is null then 'ND'
                            else TO_CHAR(Ago) end Ago,
                            case when Sep = -999 then 'NE'
                            when to_char(sysdate, 'MM') < 10 and to_char(sysdate, 'YYYY') = anno then null
                            when frequenza not in ('1M', '3M') then 'NA'
                            when frequenza in ('1M', '3M') and Sep is null then 'ND'
                            else TO_CHAR(Sep) end Sep,
                            case when
                            Ott = -999 then 'NE'
                            when to_char(sysdate, 'MM') < 11 and to_char(sysdate, 'YYYY') = anno then null
                            when frequenza not in '1M' then 'NA'
                            when frequenza = '1M' and Ott is null then 'ND'
                            else TO_CHAR(Ott) end Ott,
                            case when Nov = -999 then 'NE'
                            when to_char(sysdate, 'MM') < 12 and to_char(sysdate, 'YYYY') = anno then null
                            when frequenza not in '1M' then 'NA'
                            when frequenza = '1M' and Nov is null then 'ND'
                            else TO_CHAR(Nov) end Nov,
                            case when Dic = -999 then 'NE'
                            when to_char(sysdate, 'MM') < 13 and to_char(sysdate, 'YYYY') = anno then null
                            when Dic is null then 'ND'
                            else TO_CHAR(Dic) end Dic, anno
                            from(
                            select r.rule_description as description,
                            s.sla_name as contract,
                            cu.customer_name as contract_party,
                            r.global_rule_id,
                            replace(replace(CAST(n.note AS VARCHAR(255)), '<font size=3 face=Calibri><font size=3 face=Calibri>', ''), '<p>', '') as note,
                            r.rule_id,
                            r.rule_name as descrizione_kpi,
                            d.domain_category_relation as relation,
                            r.service_level_target as target,
                            case when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'OFF' then
                            CAST(to_char(p3.time_stamp_utc, 'MM') AS int)
                            when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'ON' then CAST(to_char(p2.time_stamp_utc, 'MM') AS int)
                            else CAST(to_char(p.time_stamp_utc, 'MM') AS int) end time_stamp_utc,
                            case when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'OFF' then p3.provided_ce
                            when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'ON' then p2.provided_ce
                            when  MONTH_TU_CALC_STATUS = 'ON' then p.provided_ce end provided_ce,
                            case when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'OFF' then '12M'
                            when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'ON' then '3M'
                            when MONTH_TU_CALC_STATUS = 'ON' then '1M' end frequenza,
                            case when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'OFF' then
                            CAST(to_char(p3.time_stamp_utc, 'YYYY') AS int)
                            when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'ON' then CAST(to_char(p2.time_stamp_utc, 'YYYY') AS int)
                            else CAST(to_char(p.time_stamp_utc, 'YYYY') AS int) end
                            as anno, MAX(r.rule_id)
                            from t_rules r
                            left
                            join t_sla_versions v on r.SLA_VERSION_ID = v.SLA_VERSION_ID
                            left
                            join t_slas s on v.sla_id = s.SLA_ID
                            left
                            join t_customers cu on s.customer_id = cu.customer_id
                            left
                            join t_psl_0_month p on p.rule_id = r.rule_id and r.is_effective = 'Y'
                                  and to_char(p.time_stamp_utc, 'YYYY') = to_char(sysdate, 'YYYY') and time_unit<> 'DAY' and p.time_stamp_utc is not null
                            left join t_psl_0_quarter p2 on p2.rule_id = r.rule_id and r.is_effective = 'Y'
                              and to_char(p2.time_stamp_utc, 'YYYY') = to_char(sysdate, 'YYYY') and p2.time_unit <> 'DAY' and p2.time_stamp_utc is not null
                            left join t_psl_0_year p3 on p3.rule_id = r.rule_id and r.is_effective = 'Y'
                              and to_char(p3.time_stamp_utc, 'YYYY') = to_char(sysdate, 'YYYY') and p3.time_unit <> 'DAY' and p3.time_stamp_utc is not null
                            left join t_domain_categories d on r.domain_category_id = d.domain_category_id
                            left join t_rule_notes n on n.rule_id = (select MAX(rule_id) from t_rules f where f.global_rule_id = r.global_rule_id )
                            where s.sla_id = 1405 and r.global_rule_id NOT IN('65350', '65352', '37643', '37645', '37641', '39033', '39035', '39040', '39042',
                             '37731', '37663', '37956', '37954', '37940', '37937', '37960', '37958', '37942', '37944', '37948', '37946', '38173', '38171', '37964', '37962', '37952', '37950', '37665', '37763', '37633', '37811', '37697', '37669', '37685', '79804', '79812', '59334')--and to_char(p.time_stamp_utc, 'YYYY') = to_char(sysdate, 'YYYY')
                                                  --and r.global_rule_id not in (77686)
                            and(p3.time_stamp_utc is not null or p2.time_stamp_utc is not null or p.time_stamp_utc is not null)
                            group by
                            s.sla_name,
                            cu.customer_name,
                            r.global_rule_id,
                            r.rule_description,
                            replace(replace(CAST(n.note AS VARCHAR(255)), '<font size=3 face=Calibri><font size=3 face=Calibri>', ''), '<p>', ''),
                            r.rule_id,r.rule_name ,
                            d.domain_category_relation,
                            r.service_level_target,
                            case when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'OFF' then
                            CAST(to_char(p3.time_stamp_utc, 'MM') AS int)
                            when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'ON' then CAST(to_char(p2.time_stamp_utc, 'MM') AS int)
                            else CAST(to_char(p.time_stamp_utc, 'MM') AS int) end,
                            case when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'OFF' then p3.provided_ce
                            when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'ON' then p2.provided_ce
                            when  MONTH_TU_CALC_STATUS = 'ON' then p.provided_ce end,
                            case when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'OFF' then '12M'
                            when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'ON' then '3M'
                            when MONTH_TU_CALC_STATUS = 'ON' then '1M' end,
                            case when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'OFF' then
                            CAST(to_char(p3.time_stamp_utc, 'YYYY') AS int)
                            when MONTH_TU_CALC_STATUS = 'OFF' and QUARTER_TU_CALC_STATUS = 'ON' then CAST(to_char(p2.time_stamp_utc, 'YYYY') AS int)
                            else CAST(to_char(p.time_stamp_utc, 'YYYY') AS int) end
                            )
                            PIVOT(max(ROUND(provided_ce, 2)) for time_stamp_utc
                            in (1 as Gen, 2 as Feb, 3 as Mar, 4 as Apr, 5 as Mag, 6 as Giu, 7 as Lug, 8 as Ago, 9 as Sep, 10 as Ott, 11 as Nov, 12 as Dic
                             ))
                                            where
                                            descrizione_kpi not like '%(OLD)%' AND descrizione_kpi not like '%SLM%' and descrizione_kpi not like '%(Progressivo)%'
                            OR descrizione_kpi like '%(Progressivo)%' and descrizione_kpi like '%(IC)%'
                            OR descrizione_kpi like '%(Progressivo)%' and descrizione_kpi like '%BPSIN001%'
                            OR descrizione_kpi like '%(Progressivo)%' and descrizione_kpi like '%BPSIN002%'
                            OR descrizione_kpi like '%(Progressivo)%' and descrizione_kpi like '%(IS)%'
                            OR descrizione_kpi like '%(Progressivo)%' and descrizione_kpi like '%BPSIN007%'
                            OR descrizione_kpi like '%(Progressivo)%' and descrizione_kpi like '%BPSIN017%'
                            OR descrizione_kpi like '%(Progressivo)%' and descrizione_kpi like '%BPSIN042%'
                            OR descrizione_kpi like '%(Progressivo)%' and descrizione_kpi like '%BPSIN043%'
                            OR descrizione_kpi like '%(Progressivo)%' and descrizione_kpi like '%BPSIN047%'
                            OR descrizione_kpi like '%(Progressivo)%' and descrizione_kpi like '%BPSIN048%'
                            OR descrizione_kpi like '%(Progressivo)%' and descrizione_kpi like '%BPSIN070%'

                            ) f
                            left join V_TOT_MEDIA med on(f.""DESCRIZIONE KPI"" = med.DESCRIZIONE_KPI and med.ANNO = to_char(sysdate, 'YYYY'))
                            left join V_TOT_MAX mas on(f.""DESCRIZIONE KPI"" = mas.DESCRIZIONE_KPI and mas.ANNO = to_char(sysdate, 'YYYY'))
                            where {0}
                            order by ""ID KPI""";
            var rules = QuantisUtilities.GetOracleGlobalRuleInQuery("global_rule_id", dto.KPIs);
            query = string.Format(query, rules);
            using (OracleConnection con = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = query;
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(new KPIStatusSummaryDTO()
                        {
                            ContractParty = (reader[0] == DBNull.Value) ? "" : (string)reader[0],
                            Contract = (reader[1] == DBNull.Value) ? "" : (string)reader[1],
                            GlobalRuleId = (reader[2] == DBNull.Value) ? 0 : Decimal.ToInt32((Decimal)reader[2]),
                            IdKPI = (reader[3] == DBNull.Value) ? "" : (string)reader[3],
                            DescrizioneKPI = (reader[4] == DBNull.Value) ? "" : (string)reader[4],
                            Tipologia = (reader[5] == DBNull.Value) ? "" : (string)reader[5],
                            Frequenza = (reader[6] == DBNull.Value) ? "" : (string)reader[6],
                            Calcolo = (reader[7] == DBNull.Value) ? "" : (string)reader[7],
                            Fornitura = (reader[8] == DBNull.Value) ? "" : (string)reader[8],
                            Escalation = (reader[9] == DBNull.Value) ? "" : (string)reader[9],
                            ViloreLimiteAtteso = (reader[10] == DBNull.Value) ? "" : (string)reader[10],
                            Trend = (reader[11] == DBNull.Value) ? 0.0 : Decimal.ToDouble((Decimal)reader[11]),
                            GEN = (reader[12] == DBNull.Value) ? "" : (string)reader[12],
                            FEB = (reader[13] == DBNull.Value) ? "" : (string)reader[13],
                            MAR = (reader[14] == DBNull.Value) ? "" : (string)reader[14],
                            APR = (reader[15] == DBNull.Value) ? "" : (string)reader[15],
                            MAG = (reader[16] == DBNull.Value) ? "" : (string)reader[16],
                            GIU = (reader[17] == DBNull.Value) ? "" : (string)reader[17],
                            LUG = (reader[18] == DBNull.Value) ? "" : (string)reader[18],
                            AGO = (reader[19] == DBNull.Value) ? "" : (string)reader[19],
                            SEP = (reader[20] == DBNull.Value) ? "" : (string)reader[20],
                            OTT = (reader[21] == DBNull.Value) ? "" : (string)reader[21],
                            NOV = (reader[22] == DBNull.Value) ? "" : (string)reader[22],
                            DIC = (reader[23] == DBNull.Value) ? "" : (string)reader[23]
                        });
                    }
                }
            }
            return result;
        }
    }
}