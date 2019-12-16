using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.API;
using Quantis.WorkFlow.Services.DTOs.OracleAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Xml.Linq;

namespace Quantis.WorkFlow.APIBase.API
{
    public class OracleDataService : IOracleDataService
    {
        private static string _connectionstring = null;
        private readonly WorkFlowPostgreSqlContext _dbcontext;
        private readonly IInformationService _informationService;

        public OracleDataService(WorkFlowPostgreSqlContext context, IInformationService informationService)
        {
            _dbcontext = context;
            _informationService = informationService;
            if (_connectionstring == null)
            {
                _connectionstring = QuantisUtilities.GetOracleConnectionString(_dbcontext);
            }
        }
        public List<BookletLVDTO> GetBookletsLV(int userId)
        {
            var result = new List<BookletLVDTO>();
            string query = @"select fo.folder_id, rg.report_id, fo.folder_name,  rg.report_name, rg.report_description, rg.report_xml, fo.folder_owner, fus.user_name, rg.report_owner, rus.user_name
                              from t_report_galleries rg , t_users rus, t_report_folders fo,  t_users fus
                              where rg.report_type ='BOOKLET'
                              and rg.is_executable=1
                              and rg.report_owner=rus.user_id
                              and fo.folder_owner=fus.user_id
                              and rg.folder_id=fo.folder_id
                              and (rg.report_owner=:user_id or fo.folder_owner=:user_id)
                              order by fo.folder_name, rg.report_name";
            using (OracleConnection con = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = query;
                    OracleParameter param1 = new OracleParameter("user_id", userId);
                    cmd.Parameters.Add(param1);
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var res = new BookletLVDTO()
                        {
                            FolderId = Decimal.ToInt32((Decimal)reader[0]),
                            ReportId = Decimal.ToInt32((Decimal)reader[1]),
                            FolderName = (string)reader[2],
                            ReportName = (string)reader[3]
                        };
                        var xml = (string)reader[5];
                        XDocument xdoc = XDocument.Parse(xml);
                        res.TemplateName= xdoc.Element("REPORT_GROUP").Element("TEMPLATE_NAME").Value;
                        res.ContractContext = Int32.Parse(xdoc.Element("REPORT_GROUP").Element("CONTRACT_CONTEXT").Value);
                        result.Add(res);
                    }
                }
            }
            var contractIds=_informationService.GetContractIdsByUserId(userId);
            result = result.Where(o => contractIds.Contains(o.ContractContext)).ToList();
            var slaIds = result.Select(o => o.ContractContext).ToList();
            var slas = _dbcontext.Slas.Where(o => slaIds.Contains(o.sla_id)).Select(o => new { sla_id=o.sla_id, sla_name=o.sla_name }).ToList();
            result=(from res in result
            join slaobj in slas on res.ContractContext equals slaobj.sla_id
            select new BookletLVDTO()
            {
                ContractContext = res.ContractContext,
                FolderId = res.FolderId,
                ContractName = slaobj.sla_name,
                FolderName = res.FolderName,
                ReportId = res.ReportId,
                ReportName = res.ReportName,
                TemplateName = res.TemplateName
            }).ToList();
            return result;
        }
        public List<ReportPersonalDTO> GetPersonalReport(PersonalReportFilterDTO filter)
        {
            var result = new List<ReportPersonalDTO>();
            string periodstring = "(psl.time_unit='MONTH' and psl.complete_record=1)";
            if (filter.AggregationOption == "trackingperiod")
            {
                periodstring = "((psl.time_unit='MONTH' and psl.complete_record=1) or (psl.time_unit='QUARTER' and psl.complete_record=1) or (psl.time_unit='YEAR' and psl.complete_record=1))";
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
                    OracleParameter param1 = new OracleParameter("start_period", filter.StartDate.AddDays(-1).ToString("yyyy-MM-dd"));
                    OracleParameter param2 = new OracleParameter("end_period", filter.EndDate.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
                    OracleParameter param3 = new OracleParameter("global_rule_id",filter.GlobalRuleId);
                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(new ReportPersonalDTO()
                        {
                            XValue = ((DateTime)reader[0]).ToString("MM/yy"),
                            Target = (double)reader[1],
                            Actual= (double)reader[2],
                            Result = (string)reader[3],
                            Unit = (string)reader[4]
                        });                        
                    }
                }
            }
            return result;
        }
        public List<OracleBookletDTO> GetBooklets()
        {
            try
            {
                string query = @"select document_name, document_id from T_DOCUMENT_REPOSITORY where document_file_type='docx' and document_type_id='2' order by document_name asc";
                using (OracleConnection con = new OracleConnection(_connectionstring))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = query;
                        OracleDataReader reader = cmd.ExecuteReader();
                        List<OracleBookletDTO> res = new List<OracleBookletDTO>();
                        while (reader.Read())
                        {
                            res.Add(new OracleBookletDTO()
                            {
                                DocumentName = (string)reader[0],
                                DocumentId = (long)reader[1]
                            });
                        }
                        return res.ToList();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<LandingPageDTO> GetLandingPageByUser(int userId, string period)
        {
            try
            {
                var basedtos = new List<LandingPageBaseDTO>();
                var kpis = _dbcontext.UserKPIs.Where(o => o.user_id == userId).Select(o => o.global_rule_id).ToList();
                kpis=_dbcontext.CatalogKpi.Where(o => kpis.Contains(o.global_rule_id_bsi) && o.kpi_category == "C").Select(o => o.global_rule_id_bsi).ToList();
                if (!kpis.Any())
                {
                    return null;
                }
                string query = @"select temp.customer_id, temp.customer_name, temp.sla_id, temp.sla_name, temp.global_rule_name, temp.global_rule_id,
psl.service_level_target_ce, psl.provided_ce, resultpsl, psl.deviation_ce from (
select
c.customer_id,
c.customer_name,
r.rule_id,
r.rule_name,
                                r.global_rule_id,
                                m.sla_id,
                                m.sla_name,
                                gr.global_rule_name,
                                r.service_level_target,
                                trf.yellow_thr as ESCALATION,
                                h.DOMAIN_CATEGORY_RELATION AS RELATION,
                                r.RULE_PERIOD_TIME_UNIT,
                                r.RULE_PERIOD_INTERVAL_LENGTH,
                                h.DOMAIN_CATEGORY_ID,
                                h.DOMAIN_CATEGORY_NAME,
                                r.HOUR_TU_CALC_STATUS,
                                r.DAY_TU_CALC_STATUS,
                                r.WEEK_TU_CALC_STATUS,
                                r.MONTH_TU_CALC_STATUS,
                                r.QUARTER_TU_CALC_STATUS,
                                r.YEAR_TU_CALC_STATUS
                                from t_rules r
                                left join t_sla_versions s on r.sla_version_id = s.sla_version_id
                                left join t_slas m on m.sla_id = s.sla_id
                                left join T_GLOBAL_RULES gr on r.global_rule_id = gr.global_rule_id
                                left join T_DOMAIN_CATEGORIES h on r.DOMAIN_CATEGORY_ID = h.DOMAIN_CATEGORY_ID
                                left join t_customers c on c.customer_id = m.customer_id
                                left join t_report_threshold_rules_flat trf on r.global_rule_id = trf.global_rule_id
                                where s.status ='EFFECTIVE' AND m.sla_status ='EFFECTIVE'
                                and r.is_effective = 'Y'
                                and {0}
                               ) temp

                                left join
                                 ( select
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
                                  and time_unit='MONTH'
                                    and complete_record=1
                                    and TRUNC(begin_time_stamp_utc) >= TO_DATE(:start_period,'yyyy-mm-dd')
                                    and TRUNC(time_stamp_utc) <= TO_DATE(:end_period,'yyyy-mm-dd')
                                    and {1}
                                ) psl
                                on psl.global_rule_id = temp.global_rule_id";
                string filter1 = QuantisUtilities.GetOracleGlobalRuleInQuery("gr.global_rule_id", kpis);
                string filter2 = QuantisUtilities.GetOracleGlobalRuleInQuery("global_rule_id", kpis);

                query = string.Format(query, filter1, filter2);
                var startDate = new DateTime(int.Parse(period.Split('/')[1]), int.Parse(period.Split('/')[0]), 1);
                using (OracleConnection con = new OracleConnection(_connectionstring))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = query;
                        OracleParameter param1 = new OracleParameter("start_period", startDate.AddDays(-1).ToString("yyyy-MM-dd"));
                        OracleParameter param2 = new OracleParameter("end_period", startDate.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
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
                                Result = (reader[8] == DBNull.Value) ? "" : (string)reader[8],
                                Deviation = (reader[9] == DBNull.Value) ? 0 : (double)reader[9],
                            }); ; ;
                        }
                        var result = basedtos.GroupBy(o => new { o.ContractPartyId, o.ContractPartyName }).Select(p => new LandingPageDTO()
                        {
                            ContractPartyId = p.Key.ContractPartyId,
                            ContractPartyName = p.Key.ContractPartyName,
                            TotalContracts = p.GroupBy(q => q.ContractId).Count(),
                            TotalKPIs = p.Count(),
                            ComplaintContracts = p.GroupBy(q => q.ContractId).Where(r => r.All(s => s.Result == "compliant")).Count(),
                            NonComplaintContracts = p.GroupBy(q => q.ContractId).Where(r => r.Any(s => s.Result != "compliant")).Count(),
                            ComplaintKPIs = p.Where(q => q.Result == "compliant").Count(),
                            NonComplaintKPIs = p.Where(q => q.Result == "non compliant").Count(),
                            BestContracts = p.GroupBy(q => new { q.ContractName, q.ContractId }).OrderByDescending(r => r.Count(s => s.Result == "compliant") / r.Count()).ThenByDescending(r=> r.Count(s => s.Result == "compliant")).Select(t => new LandingPageContractDTO()
                            {
                                ContractId = t.Key.ContractId,
                                ContractName = t.Key.ContractName,
                                TotalKPIs = t.Count(),
                                ComplaintKPIs = t.Count(u => u.Result == "compliant"),
                                NonComplaintKPIs = t.Count(u => u.Result == "non compliant"),
                                ComplaintPercentage = (t.Count(u => u.Result == "compliant") * 100) / t.Count(),
                                AverageDeviation = t.Average(u => u.Deviation)
                            }).ToList(),
                            WorstContracts = p.GroupBy(q => new { q.ContractName, q.ContractId }).OrderByDescending(r => r.Count(s => s.Result == "non compliant") / r.Count()).ThenByDescending(r => r.Count(s => s.Result != "compliant")).Select(t => new LandingPageContractDTO()
                            {
                                ContractId = t.Key.ContractId,
                                ContractName = t.Key.ContractName,
                                TotalKPIs = t.Count(),
                                ComplaintKPIs = t.Count(u => u.Result == "compliant"),
                                NonComplaintKPIs = t.Count(u => u.Result == "non compliant"),
                                ComplaintPercentage = (t.Count(u => u.Result == "compliant") * 100) / t.Count(),
                                AverageDeviation = t.Average(u => u.Deviation)
                            }).ToList()
                        }).ToList();
                        return result.OrderBy(o=>o.ContractPartyName).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<LandingPageLevel1DTO> GetLandingPageLevel1(int userId, int contractPartyId, string period)
        {
            var basedtos = new List<LandingPageBaseDTO>();
            var kpis = _dbcontext.UserKPIs.Where(o => o.user_id == userId).Select(o => o.global_rule_id).ToList();
            kpis = _dbcontext.CatalogKpi.Where(o => kpis.Contains(o.global_rule_id_bsi) && o.kpi_category == "C").Select(o => o.global_rule_id_bsi).ToList();
            if (!kpis.Any())
            {
                return null;
            }
            string query = @"select temp.customer_id, temp.customer_name, temp.sla_id, temp.sla_name, temp.global_rule_name, temp.global_rule_id,
                                psl.service_level_target_ce, psl.provided_ce, resultpsl, psl.deviation_ce,temp.RELATION,temp.unit_symbol from (
                                select
                                c.customer_id,
                                c.customer_name,
                                r.rule_id,
                                r.rule_name,
                                r.global_rule_id,
                                m.sla_id,
                                m.sla_name,
                                gr.global_rule_name,
                                r.service_level_target,
                                trf.yellow_thr as ESCALATION,
                                h.DOMAIN_CATEGORY_RELATION AS RELATION,
                                u.unit_symbol,
                                r.RULE_PERIOD_TIME_UNIT,
                                r.RULE_PERIOD_INTERVAL_LENGTH,
                                h.DOMAIN_CATEGORY_ID,
                                h.DOMAIN_CATEGORY_NAME,
                                r.HOUR_TU_CALC_STATUS,
                                r.DAY_TU_CALC_STATUS,
                                r.WEEK_TU_CALC_STATUS,
                                r.MONTH_TU_CALC_STATUS,
                                r.QUARTER_TU_CALC_STATUS,
                                r.YEAR_TU_CALC_STATUS
                                from t_rules r
                                left join t_sla_versions s on r.sla_version_id = s.sla_version_id
                                left join t_slas m on m.sla_id = s.sla_id
                                left join T_GLOBAL_RULES gr on r.global_rule_id = gr.global_rule_id
                                left join T_DOMAIN_CATEGORIES h on r.DOMAIN_CATEGORY_ID = h.DOMAIN_CATEGORY_ID
                                left join t_units u on h.unit_id = u.unit_id
                                left join t_customers c on c.customer_id = m.customer_id
                                left join t_report_threshold_rules_flat trf on r.global_rule_id = trf.global_rule_id
                                where s.status ='EFFECTIVE' AND m.sla_status ='EFFECTIVE'
                                and r.is_effective = 'Y'
                                and c.customer_id = :customer_id
                                and {0}
                               ) temp

                                left join
                                 ( select
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
                                  and time_unit='MONTH'
                                    and complete_record=1
                                    and TRUNC(begin_time_stamp_utc) >= TO_DATE(:start_period,'yyyy-mm-dd')
                                    and TRUNC(time_stamp_utc) <= TO_DATE(:end_period,'yyyy-mm-dd')
                                    and {1}
                                ) psl
                                on psl.global_rule_id = temp.global_rule_id";
            string filter1 = QuantisUtilities.GetOracleGlobalRuleInQuery("gr.global_rule_id", kpis);
            string filter2 = QuantisUtilities.GetOracleGlobalRuleInQuery("global_rule_id", kpis);

            query = string.Format(query, filter1, filter2);
            var startDate = new DateTime(int.Parse(period.Split('/')[1]), int.Parse(period.Split('/')[0]), 1);
            using (OracleConnection con = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = query;
                    OracleParameter param1 = new OracleParameter("start_period", startDate.AddDays(-1).ToString("yyyy-MM-dd"));
                    OracleParameter param2 = new OracleParameter("end_period", startDate.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
                    OracleParameter param3 = new OracleParameter("customer_id", contractPartyId);
                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
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
                            Result = (reader[8] == DBNull.Value) ? "" : (string)reader[8],
                            Deviation = (reader[9] == DBNull.Value) ? 0 : (double)reader[9],
                            Operator = (reader[10] == DBNull.Value) ? "" : ((string)reader[10] == "NLT" ? ">" : "<"),
                            Unit = (reader[11] == DBNull.Value) ? "" : (string)reader[11]
                        });
                    }
                    var result = basedtos.GroupBy(o => new { o.ContractId, o.ContractName }).Select(p => new LandingPageLevel1DTO()
                    {
                        ContractId = p.Key.ContractId,
                        ContractName = p.Key.ContractName,
                        AverageDeviation = p.Average(o => o.Deviation),
                        ComplaintPercentage = (p.Count(o => o.Result == "compliant") * 100) / p.Count(),
                        TotalKPIs = p.Count(),
                        ComplaintKPIs = p.Where(q => q.Result == "compliant").Count(),
                        NonComplaintKPIs = p.Where(q => q.Result == "non compliant").Count(),
                        BestKPIs = p.Where(q => q.Result == "compliant").OrderByDescending(o => o.Deviation).Select(o => new LandingPageKPIDTO()
                        {
                            KPIID = o.GlobalRuleId,
                            KPIName = o.GlobalRuleName,
                            Target = o.Target,
                            Value = o.Actual,
                            Unit = o.Unit,
                            Operator = o.Operator
                            
                        }).ToList(),
                        WorstKPIs = p.Where(q => q.Result == "non compliant").OrderBy(o => o.Deviation).Select(o => new LandingPageKPIDTO()
                        {
                            KPIID = o.GlobalRuleId,
                            KPIName = o.GlobalRuleName,
                            Target = o.Target,
                            Value = o.Actual,
                            Unit = o.Unit,
                            Operator = o.Operator
                        }).ToList(),
                    }).ToList();
                    return result.OrderBy(o => o.ContractName).ToList();
                }
            }
        }

        public LandingPageLevel1DTO GetLandingPageLevel1ByContract(int userId, int contractId, string period)
        {
            var basedtos = new List<LandingPageBaseDTO>();
            var kpis = _dbcontext.UserKPIs.Where(o => o.user_id == userId).Select(o => o.global_rule_id).ToList();
            kpis = _dbcontext.CatalogKpi.Where(o => kpis.Contains(o.global_rule_id_bsi) && o.kpi_category == "C").Select(o => o.global_rule_id_bsi).ToList();
            if (!kpis.Any())
            {
                return null;
            }
            string query = @"select temp.customer_id, temp.customer_name, temp.sla_id, temp.sla_name, temp.global_rule_name, temp.global_rule_id,
                                psl.service_level_target_ce, psl.provided_ce, resultpsl, psl.deviation_ce,temp.RELATION,temp.unit_symbol from (
                                select
                                c.customer_id,
                                c.customer_name,
                                r.rule_id,
                                r.rule_name,
                                r.global_rule_id,
                                m.sla_id,
                                m.sla_name,
                                gr.global_rule_name,
                                r.service_level_target,
                                trf.yellow_thr as ESCALATION,
                                h.DOMAIN_CATEGORY_RELATION AS RELATION,
                                u.unit_symbol,
                                r.RULE_PERIOD_TIME_UNIT,
                                r.RULE_PERIOD_INTERVAL_LENGTH,
                                h.DOMAIN_CATEGORY_ID,
                                h.DOMAIN_CATEGORY_NAME,
                                r.HOUR_TU_CALC_STATUS,
                                r.DAY_TU_CALC_STATUS,
                                r.WEEK_TU_CALC_STATUS,
                                r.MONTH_TU_CALC_STATUS,
                                r.QUARTER_TU_CALC_STATUS,
                                r.YEAR_TU_CALC_STATUS
                                from t_rules r
                                left join t_sla_versions s on r.sla_version_id = s.sla_version_id
                                left join t_slas m on m.sla_id = s.sla_id
                                left join T_GLOBAL_RULES gr on r.global_rule_id = gr.global_rule_id
                                left join T_DOMAIN_CATEGORIES h on r.DOMAIN_CATEGORY_ID = h.DOMAIN_CATEGORY_ID
                                left join t_units u on h.unit_id = u.unit_id
                                left join t_customers c on c.customer_id = m.customer_id
                                left join t_report_threshold_rules_flat trf on r.global_rule_id = trf.global_rule_id
                                where s.status ='EFFECTIVE' AND m.sla_status ='EFFECTIVE'
                                and r.is_effective = 'Y'
                                and m.sla_id = :sla_id
                                and {0}
                               ) temp

                                left join
                                 ( select
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
                                  and time_unit='MONTH'
                                    and complete_record=1
                                    and TRUNC(begin_time_stamp_utc) >= TO_DATE(:start_period,'yyyy-mm-dd')
                                    and TRUNC(time_stamp_utc) <= TO_DATE(:end_period,'yyyy-mm-dd')
                                    and {1}
                                ) psl
                                on psl.global_rule_id = temp.global_rule_id";
            string filter1 = QuantisUtilities.GetOracleGlobalRuleInQuery("gr.global_rule_id", kpis);
            string filter2 = QuantisUtilities.GetOracleGlobalRuleInQuery("global_rule_id", kpis);

            query = string.Format(query, filter1, filter2);
            var startDate = new DateTime(int.Parse(period.Split('/')[1]), int.Parse(period.Split('/')[0]), 1);
            using (OracleConnection con = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = query;
                    OracleParameter param1 = new OracleParameter("start_period", startDate.AddDays(-1).ToString("yyyy-MM-dd"));
                    OracleParameter param2 = new OracleParameter("end_period", startDate.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
                    OracleParameter param3 = new OracleParameter("sla_id", contractId);
                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
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
                            Result = (reader[8] == DBNull.Value) ? "" : (string)reader[8],
                            Deviation = (reader[9] == DBNull.Value) ? 0 : (double)reader[9],
                            Operator = (reader[10] == DBNull.Value) ? "" : ((string)reader[10]=="NLT"?">":"<"),
                            Unit = (reader[11] == DBNull.Value) ? "" : (string)reader[11]
                        });
                    }
                    var result = basedtos.GroupBy(o => new { o.ContractId, o.ContractName }).Select(p => new LandingPageLevel1DTO()
                    {
                        ContractId = p.Key.ContractId,
                        ContractName = p.Key.ContractName,
                        AverageDeviation = p.Average(o => o.Deviation),
                        ComplaintPercentage = (p.Count(o => o.Result == "compliant") * 100) / p.Count(),
                        TotalKPIs = p.Count(),
                        ComplaintKPIs = p.Where(q => q.Result == "compliant").Count(),
                        NonComplaintKPIs = p.Where(q => q.Result == "non compliant").Count(),
                        BestKPIs = p.Where(q => q.Result == "compliant").OrderByDescending(o => o.Deviation).Select(o => new LandingPageKPIDTO()
                        {
                            KPIID = o.GlobalRuleId,
                            KPIName = o.GlobalRuleName,
                            Target = o.Target,
                            Value = o.Actual,
                            Unit = o.Unit,
                            Operator = o.Operator
                        }).ToList(),
                        WorstKPIs = p.Where(q => q.Result == "non compliant").OrderBy(o => o.Deviation).Select(o => new LandingPageKPIDTO()
                        {
                            KPIID = o.GlobalRuleId,
                            KPIName = o.GlobalRuleName,
                            Target = o.Target,
                            Value = o.Actual,
                            Unit = o.Unit,
                            Operator = o.Operator
                        }).ToList(),
                    }).FirstOrDefault();
                    return result;
                }
            }
        }

        public List<LandingPageBaseDTO> GetLandingPageKPIDetails(int userId, int contractPartyId, string period)
        {
            var basedtos = new List<LandingPageBaseDTO>();
            var kpis = _dbcontext.UserKPIs.Where(o => o.user_id == userId).Select(o => o.global_rule_id).ToList();
            kpis = _dbcontext.CatalogKpi.Where(o => kpis.Contains(o.global_rule_id_bsi) && o.kpi_category == "C").Select(o => o.global_rule_id_bsi).ToList();
            if (!kpis.Any())
            {
                return null;
            }
            string query = @"select temp.customer_id, temp.customer_name, temp.sla_id, temp.sla_name, temp.global_rule_name, temp.global_rule_id,
                                psl.service_level_target_ce, psl.provided_ce, resultpsl, psl.deviation_ce from (
                                select
                                c.customer_id,
                                c.customer_name,
                                r.rule_id,
                                r.rule_name,
                                r.global_rule_id,
                                m.sla_id,
                                m.sla_name,
                                gr.global_rule_name,
                                r.service_level_target,
                                trf.yellow_thr as ESCALATION,
                                h.DOMAIN_CATEGORY_RELATION AS RELATION,
                                r.RULE_PERIOD_TIME_UNIT,
                                r.RULE_PERIOD_INTERVAL_LENGTH,
                                h.DOMAIN_CATEGORY_ID,
                                h.DOMAIN_CATEGORY_NAME,
                                r.HOUR_TU_CALC_STATUS,
                                r.DAY_TU_CALC_STATUS,
                                r.WEEK_TU_CALC_STATUS,
                                r.MONTH_TU_CALC_STATUS,
                                r.QUARTER_TU_CALC_STATUS,
                                r.YEAR_TU_CALC_STATUS
                                from t_rules r
                                left join t_sla_versions s on r.sla_version_id = s.sla_version_id
                                left join t_slas m on m.sla_id = s.sla_id
                                left join T_GLOBAL_RULES gr on r.global_rule_id = gr.global_rule_id
                                left join T_DOMAIN_CATEGORIES h on r.DOMAIN_CATEGORY_ID = h.DOMAIN_CATEGORY_ID
                                left join t_customers c on c.customer_id = m.customer_id
                                left join t_report_threshold_rules_flat trf on r.global_rule_id = trf.global_rule_id
                                where s.status ='EFFECTIVE' AND m.sla_status ='EFFECTIVE'
                                and r.is_effective = 'Y'
                                and c.customer_id = :customer_id
                                and {0}
                               ) temp

                                left join
                                 ( select
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
                                  and time_unit='MONTH'
                                    and complete_record=1
                                    and TRUNC(begin_time_stamp_utc) >= TO_DATE(:start_period,'yyyy-mm-dd')
                                    and TRUNC(time_stamp_utc) <= TO_DATE(:end_period,'yyyy-mm-dd')
                                    and {1}
                                ) psl
                                on psl.global_rule_id = temp.global_rule_id";
            string filter1 = QuantisUtilities.GetOracleGlobalRuleInQuery("gr.global_rule_id", kpis);
            string filter2 = QuantisUtilities.GetOracleGlobalRuleInQuery("global_rule_id", kpis);

            query = string.Format(query, filter1, filter2);
            var startDate = new DateTime(int.Parse(period.Split('/')[1]), int.Parse(period.Split('/')[0]), 1);
            using (OracleConnection con = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = query;
                    OracleParameter param1 = new OracleParameter("start_period", startDate.AddDays(-1).ToString("yyyy-MM-dd"));
                    OracleParameter param2 = new OracleParameter("end_period", startDate.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
                    OracleParameter param3 = new OracleParameter("customer_id", contractPartyId);
                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
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
                            Result = (reader[8] == DBNull.Value) ? "" : (string)reader[8],
                            Deviation = (reader[9] == DBNull.Value) ? 0 : (double)reader[9],
                        });
                    }
                    
                    return basedtos;
                }
            }
        }

        public List<OracleCustomerDTO> GetCustomer(int id, string name)
        {
            try
            {
                string query = @"select c.customer_id,c.customer_name,s.sla_id,s.sla_name from t_customers c left join t_slas s on s.customer_id = c.customer_id left join t_sla_versions v on s.sla_id = v.sla_id where s.sla_status = 'EFFECTIVE' and v.status = 'EFFECTIVE'";
                if (!string.IsNullOrEmpty(name))
                {
                    query += " and LOWER(c.customer_name) LIKE LOWER('%' || :customer_name || '%')";
                }
                if (id != 0)
                {
                    query += " and c.customer_id = :customer_id";
                }

                using (OracleConnection con = new OracleConnection(_connectionstring))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = query;
                        OracleParameter param1 = new OracleParameter("customer_id", id);
                        OracleParameter param2 = new OracleParameter("customer_name", name);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        OracleDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        List<DataRow> list = dt.AsEnumerable().ToList();
                        var values = list.GroupBy(o => new { customer_id = o[0], customer_name = o[1] }).Select(p => new OracleCustomerDTO()
                        {
                            customer_id = Decimal.ToInt32((Decimal)p.Key.customer_id),
                            customer_name = (string)p.Key.customer_name,
                            slas = p.Select(q => new CustomerSLA()
                            {
                                sla_id = Decimal.ToInt32((Decimal)q[2]),
                                sla_name = (string)q[3]
                            }).ToList()
                        });
                        return values.ToList();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //public List<PslDTO> GetPsl(string period, string sla_name, string rule_name, string tracking_period)
        public List<PslDTO> GetPsl(string period, int global_rule_id, string tracking_period)
        {
            try
            {
                var period_table = "t_psl_0_month";
                switch (tracking_period)
                {
                    case "MENSILE":
                        period_table = "t_psl_0_month";
                        break;

                    case "TRIMESTRALE":
                        period_table = "t_psl_0_quarter";
                        break;

                    case "QUADRIMESTRALE":
                        period_table = "t_psl_0_month";
                        break;

                    case "SEMESTRALE":
                        period_table = "t_psl_0_month";
                        break;

                    case "ANNUALE":
                        period_table = "t_psl_0_year";
                        break;
                }

                string query = @"select s.sla_id, r.rule_id, ROUND(p.provided, 2), ROUND(p.provided_c, 2), ROUND(p.provided_e, 2), ROUND(p.provided_ce, 2), time_stamp_utc, d.domain_category_relation, r.service_level_target, u.unit_symbol from t_rules r left join t_sla_versions v on r.SLA_VERSION_ID = v.SLA_VERSION_ID left join t_global_rules gr on gr.global_rule_id = :global_rule_id left join t_slas s on v.sla_id = s.SLA_ID left join t_domain_categories d on r.domain_category_id = d.domain_category_id left join t_units u on d.unit_id = u.unit_id left join ";
                query += period_table;
                query += " p on p.rule_id = r.rule_id and r.is_effective = 'Y' and CONCAT(CONCAT(to_char(time_stamp_utc, 'MM'), '/'), to_char(time_stamp_utc, 'YY')) = :period where r.rule_name = gr.global_rule_name and p.time_stamp_utc is not null";
                using (OracleConnection con = new OracleConnection(_connectionstring))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = query;
                        OracleParameter param1 = new OracleParameter("period", period);
                        OracleParameter param2 = new OracleParameter("global_rule_id", global_rule_id);
                        //OracleParameter param2 = new OracleParameter("sla_name", sla_name);
                        //OracleParameter param3 = new OracleParameter("rule_name", rule_name);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        //cmd.Parameters.Add(param3);
                        OracleDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        List<DataRow> list = dt.AsEnumerable().ToList();
                        var values = list.Select(o => new PslDTO()
                        {
                            sla_id = Decimal.ToInt32((Decimal)o[0]),
                            rule_id = Decimal.ToInt32((Decimal)o[1]),
                            provided = (o[2] == DBNull.Value) ? 0 : Decimal.ToInt32((Decimal)o[2]),
                            provided_c = (o[3] == DBNull.Value) ? 0 : Decimal.ToInt32((Decimal)o[3]),
                            provided_e = (o[4] == DBNull.Value) ? 0 : Decimal.ToInt32((Decimal)o[4]),
                            provided_ce = (o[5] == DBNull.Value) ? 0 : Decimal.ToInt32((Decimal)o[5]),
                            time_stamp_utc = (DateTime)o[6],
                            result = (o[5] == DBNull.Value) ? "[NE]" :
                                (Decimal)o[5] == -999 ? "[Nessun Evento]" :
                                (o[7].ToString() == "NLT") ?
                                (Decimal.ToInt32((Decimal)o[5]) < (Decimal)o[8] ? "[Non Compliant]" : "[Compliant]")
                                :
                                (Decimal.ToInt32((Decimal)o[5]) < (Decimal)o[8] ? "[Compliant]" : "[Non Compliant]"),
                            target = (o[8] == DBNull.Value) ? 0 : Decimal.ToInt32((Decimal)o[8]),
                            relation = o[7].ToString(),
                            symbol = o[9].ToString()
                        });
                        return values.ToList();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /*
                public List<OracleFormDTO> GetForm(int id, int userid)
                {
                    try
                    {
                        bool securityMember = false;
                        if (userid != 0)
                        {
                            using (OracleConnection con1 = new OracleConnection(_connectionstring))
                            {
                                using (OracleCommand cmd = con1.CreateCommand())
                                {
                                    string query1 = "select user_group_id, security_group_id from t_security_group_members where security_group_id in (select security_group_id from t_security_groups sg where sg.security_group_name = 'Insight Super Administrators') and user_group_id = :userid";
                                    con1.Open();
                                    cmd.BindByName = true;
                                    cmd.CommandText = query1;
                                    OracleParameter param1 = new OracleParameter("userid", userid);
                                    cmd.Parameters.Add(param1);
                                    OracleDataReader reader = cmd.ExecuteReader();
                                    DataTable dt = new DataTable();
                                    dt.Load(reader);
                                    List<DataRow> userSecurity = dt.AsEnumerable().ToList();
                                    if(userSecurity.Count() >= 1)
                                    {
                                        securityMember = true;
                                    }
                                }
                            }
                        }
                        string query = "select f.form_id, f.form_name,f.form_description, f.reader_id,f.form_owner_id,f.create_date, f.modify_date,	ug.user_group_id,ug.user_group_name from t_forms f left join t_forms_permitted_users fpu on fpu.form_id = f.form_id left join t_user_groups ug on fpu.user_group_id = ug.user_group_id where 1 = 1 ";
                        bool getConfigurations = false;
                        if (userid != 0)
                        {
                            if (!securityMember)
                            {
                                query += " and ug.user_group_id = :userid";
                            }
                            else
                            {
                                query = "select f.form_id, f.form_name,f.form_description, f.reader_id,f.form_owner_id,f.create_date, f.modify_date from t_forms f where 1 = 1 ";
                            }
                        }
                        if (id != 0)
                        {
                            //query = "select f.form_id, f.form_name,f.form_description, f.reader_id,f.form_owner_id,f.create_date, f.modify_date, r.reader_configuration,	ug.user_group_id,ug.user_group_name from t_forms f left join t_readers r on f.reader_id = r.reader_id left join t_forms_permitted_users fpu on fpu.form_id = f.form_id left join t_user_groups ug on fpu.user_group_id = ug.user_group_id where 1 = 1 ";
                            query = "select f.form_id, f.form_name,f.form_description, f.reader_id,f.form_owner_id,f.create_date, f.modify_date, r.reader_configuration, f.form_schema from t_forms f left join t_readers r on f.reader_id = r.reader_id where 1 = 1 ";

                            query += " and f.form_id = :form_id";
                            getConfigurations = true;
                        }
                        var day_cutoffValue = _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_restserver" && o.key == "day_cutoff");
                        //per comodit prendo il cutoff dalla t_configurations e non dalla t_catalog_kpi
                        string todayDayValue = DateTime.Now.ToString("dd");
                        int todayDay = Int32.Parse(todayDayValue);
                        int day_cutoff = Int32.Parse(day_cutoffValue.value);
                        bool cutoff_result;
                        if(day_cutoff == 0) {
                            cutoff_result = false;
                        } else {
                            if (todayDay < day_cutoff) { cutoff_result = false; } else { cutoff_result = true; }
                        }

                        using (OracleConnection con = new OracleConnection(_connectionstring))
                        {
                            using (OracleCommand cmd = con.CreateCommand())
                            {
                                con.Open();
                                cmd.BindByName = true;
                                cmd.CommandText = query;
                                OracleParameter param1 = new OracleParameter("form_id", id);
                                OracleParameter param2 = new OracleParameter("userid", userid);
                                cmd.Parameters.Add(param1);
                                cmd.Parameters.Add(param2);
                                OracleDataReader reader = cmd.ExecuteReader();
                                DataTable dt = new DataTable();
                                dt.Load(reader);
                                List<DataRow> list = dt.AsEnumerable().ToList();

                                if (getConfigurations)
                                {
                                    var result = list.Select(o => new OracleFormDTO()
                                    {
                                        form_id = Decimal.ToInt32((Decimal)o[0]),
                                        form_name = (string)o[1],
                                        form_description = (o[2] == DBNull.Value) ? string.Empty : (string)o[2],
                                        reader_id = Decimal.ToInt32((Decimal)o[3]),
                                        form_owner_id = Decimal.ToInt32((Decimal)o[4]),
                                        create_date = (DateTime)o[5],
                                        modify_date = (DateTime)o[6],
                                        reader_configuration = GetFormAdapterConfiguration((string)o[7], GetFormConfiguration((string)o[8])),
                                        user_group_id = 0,//(o[8] == DBNull.Value) ? 0 : Decimal.ToInt32((Decimal)o[8]),
                                        user_group_name = string.Empty, //(o[9] == DBNull.Value) ? string.Empty : (string)o[9],
                                        day_cutoff = day_cutoff,
                                        cutoff = (bool)cutoff_result,
                                        latest_input_date = _dbcontext.FormLogs.Any(p => p.id_form == id) ? _dbcontext.FormLogs.Where(q => q.id_form == id).Max(r => r.time_stamp) : new DateTime(0)
                                    });
                                    return result.ToList();
                                }
                                else
                                {
                                    var result = list.Select(o => new OracleFormDTO()
                                    {
                                        form_id = Decimal.ToInt32((Decimal)o[0]),
                                        form_name = (string)o[1],
                                        form_description = (o[2] == DBNull.Value) ? string.Empty : (string)o[2],
                                        reader_id = Decimal.ToInt32((Decimal)o[3]),
                                        form_owner_id = Decimal.ToInt32((Decimal)o[4]),
                                        create_date = (DateTime)o[5],
                                        modify_date = (DateTime)o[6],
                                        reader_configuration = null,
                                        user_group_id = securityMember ? 0 :
                                            (o[7] == DBNull.Value) ? 0 : Decimal.ToInt32((Decimal)o[7]),
                                        user_group_name = securityMember ? string.Empty :
                                            (o[8] == DBNull.Value) ? string.Empty : (string)o[8],
                                        day_cutoff = day_cutoff,
                                        cutoff = (bool)cutoff_result,
                                        latest_input_date = securityMember ? new DateTime(0) :
                                            _dbcontext.FormLogs.Any(p => p.id_form == id) ? _dbcontext.FormLogs.Where(q => q.id_form == id).Max(r => r.time_stamp) : new DateTime(0)
                                    });
                                    return result.ToList();
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                private ReaderConfiguration GetFormAdapterConfiguration(string xml, List<FormConfigurationDTO> conf)
                {
                    XDocument xdoc = XDocument.Parse(xml);
                    var lists = from uoslist in xdoc.Element("AdapterConfiguration").Element("InputFormatCollection").Element("InputFormat").Element("InputFormatFields").Elements("InputFormatField") select uoslist;
                    var formfields = new List<FormField>();

                    foreach (var l in lists)
                    {
                        formfields.Add(new FormField()
                        {
                            name = l.Attribute("Name").Value,
                            type = l.Attribute("Type").Value,
                            source = l.Attribute("Source").Value,
                        });
                    }
                    foreach (var s in conf)
                    {
                        if (s.a_type == "DLFLabel" && s.text != "Label" && s.text != null && s.text.Length > 0)
                        {
                            formfields.Add(new FormField()
                            {
                                name = "Label",
                                type = "Label",
                                source = s.text
                            });
                        }
                    }
                    return new ReaderConfiguration()
                    {
                        inputformatfield = (from f in formfields
                                            join c in conf on f.name equals c.name
                                            into gj from subset in gj.DefaultIfEmpty()
                                            select new FormField()
                                            {
                                                name = f.name,
                                                label = subset?.text??String.Empty,
                                                mandatory = subset?.a_isMandatory??String.Empty,
                                                defaultValue = subset?.defaultValue??String.Empty,
                                                source = f.source,
                                                type = f.type,
                                                a_type = subset?.a_type??String.Empty,
                                            }).ToList()
                    };
                }
                */

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<OracleGroupDTO> GetGroup(int id, string name)
        {
            try
            {
                string query = @"select ug.user_group_id,ug.user_group_name,u.user_id, u.user_name,u.user_email from t_user_groups ug left join t_user_group_members ugm on ug.user_group_id = ugm.user_group_id left join t_users u on ugm.user_id = u.user_id where 1=1";
                if (!string.IsNullOrEmpty(name))
                {
                    query += "and LOWER(ug.user_group_name) LIKE LOWER('%' || :user_group_name || '%')";
                }
                if (id != 0)
                {
                    query += "and ug.user_group_id = :user_group_id";
                }

                using (OracleConnection con = new OracleConnection(_connectionstring))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = query;
                        OracleParameter param1 = new OracleParameter("user_group_id", id);
                        OracleParameter param2 = new OracleParameter("user_group_name", name);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        OracleDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        List<DataRow> list = dt.AsEnumerable().ToList();
                        var values = list.GroupBy(o => new { group_id = o[0], group_name = o[1] }).Select(p => new OracleGroupDTO()
                        {
                            user_group_id = Decimal.ToInt32((Decimal)p.Key.group_id),
                            user_group_name = (string)p.Key.group_name,
                            users = p.Select(q => new OracleGroupUserDTO()
                            {
                                user_id = Decimal.ToInt32((Decimal)q[2]),
                                user_name = (string)q[3],
                                user_email = (q[4] == DBNull.Value) ? string.Empty : (string)q[4]
                            }).ToList()
                        });
                        return values.ToList();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<OracleSlaDTO> GetSla(int id, string name)
        {
            try
            {
                string query = @"select f.sla_id, f.sla_name, r.sla_version_id, MAX (f.sla_versions) as last_version, f.sla_status, f.sla_valid_from, f.sla_valid_to, s.customer_id, s.customer_name from t_slas f left join t_customers s on f.customer_id = s.customer_id left join t_sla_versions r on f.sla_id = r.sla_id where f.sla_status = 'EFFECTIVE' AND r.status ='EFFECTIVE'";
                if (!string.IsNullOrEmpty(name))
                {
                    query += " and LOWER(r.sla_name) LIKE LOWER('%' || :sla_name || '%')";
                }
                if (id != 0)
                {
                    query += " and f.sla_id = :sla_id";
                }
                query += " group by f.sla_name,f.sla_id, f.sla_status, f.sla_valid_from, f.sla_valid_to, s.customer_name, s.customer_id, r.sla_version_id order by f.sla_id ASC";

                using (OracleConnection con = new OracleConnection(_connectionstring))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = query;
                        OracleParameter param1 = new OracleParameter("sla_id", id);
                        OracleParameter param2 = new OracleParameter("sla_name", name);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        OracleDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        List<DataRow> list = dt.AsEnumerable().ToList();
                        var values = list.Select(o => new OracleSlaDTO()
                        {
                            sla_id = Decimal.ToInt32((Decimal)o[0]),
                            sla_name = (String)o[1],
                            sla_version_id = Decimal.ToInt32((Decimal)o[2]),
                            last_version = Decimal.ToInt32((Decimal)o[3]),
                            sla_status = (String)o[4],
                            sla_valid_from = (DateTime)o[5],
                            sla_valid_to = (DateTime)o[6],
                            customer_id = Decimal.ToInt32((Decimal)o[7]),
                            customer_name = (String)o[8],
                        });
                        return values.ToList();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<OracleRuleDTO> GetRule(int id, string name)
        {
            try
            {
                string query = @"select r.rule_id,
                                r.rule_name,
                                r.global_rule_id,
                                m.sla_id,
                                m.sla_name,
                                r.service_level_target,
                                trf.yellow_thr as ESCALATION,
                                h.DOMAIN_CATEGORY_RELATION AS RELATION,
                                r.RULE_PERIOD_TIME_UNIT,
                                r.RULE_PERIOD_INTERVAL_LENGTH,
                                h.DOMAIN_CATEGORY_ID,
                                h.DOMAIN_CATEGORY_NAME,
                                r.HOUR_TU_CALC_STATUS,
                                r.DAY_TU_CALC_STATUS,
                                r.WEEK_TU_CALC_STATUS,
                                r.MONTH_TU_CALC_STATUS,
                                r.QUARTER_TU_CALC_STATUS,
                                r.YEAR_TU_CALC_STATUS
                                from t_rules r left join t_sla_versions s on r.sla_version_id = s.sla_version_id
                                left join t_slas m on m.sla_id = s.sla_id
                                left join T_DOMAIN_CATEGORIES h on r.DOMAIN_CATEGORY_ID = h.DOMAIN_CATEGORY_ID
                                left join t_report_threshold_rules_flat trf on r.global_rule_id = trf.global_rule_id
                                where s.status ='EFFECTIVE' AND m.sla_status ='EFFECTIVE'
                                ";
                if (!string.IsNullOrEmpty(name))
                {
                    query += "and LOWER(r.rule_name) LIKE LOWER('%' || :rule_name || '%')";
                }
                if (id != 0)
                {
                    query += "and r.rule_id = :rule_id";
                }

                using (OracleConnection con = new OracleConnection(_connectionstring))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = query;
                        OracleParameter param1 = new OracleParameter("rule_id", id);
                        OracleParameter param2 = new OracleParameter("rule_name", name);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        OracleDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        List<DataRow> list = dt.AsEnumerable().ToList();
                        var values = list.Select(p => new OracleRuleDTO()
                        {
                            rule_id = Decimal.ToInt32((Decimal)p[0]),
                            rule_name = (string)p[1],
                            global_rule_id = Decimal.ToInt32((Decimal)p[2]),
                            sla_id = Decimal.ToInt32((Decimal)p[3]),
                            sla_name = (string)p[4],
                            service_level_target = (p[5] == DBNull.Value) ? (int?)null : Decimal.ToInt32((Decimal)p[5]),
                            escalation = (p[6] == DBNull.Value) ? (int?)null : Decimal.ToInt32((Decimal)p[6]),
                            relation = (string)p[7],
                            rule_period_time_unit = (string)p[8],
                            rule_period_interval_length = (p[9] == DBNull.Value) ? (int?)null : (int)(long)p[9],
                            domain_category_id = (p[10] == DBNull.Value) ? 0 : Decimal.ToInt32((Decimal)p[10]),
                            domain_category_name = (string)p[11],
                            granularity = new OracleRuleGranularityDTO()
                            {
                                hour_tu_calc_status = (string)p[12],
                                day_tu_calc_status = (string)p[13],
                                week_tu_calc_status = (p[14] == DBNull.Value) ? string.Empty : (string)p[14],
                                month_tu_calc_status = (string)p[15],
                                quarter_tu_calc_status = (string)p[16],
                                year_tu_calc_status = (string)p[17]
                            }
                        });
                        return values.ToList();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<BSIFreeFormReportDTO> GetBSIFreeFormReports()
        {
            var results = new List<BSIFreeFormReportDTO>();
            string query = @"select fo.folder_id, rg.report_id, fo.folder_name,  rg.report_name, rg.report_description, rg.report_xml, fo.folder_owner, fus.user_name, rg.report_owner, rus.user_name
                              from t_report_galleries rg , t_users rus, t_report_folders fo,  t_users fus
                              where rg.report_type ='FREEFORM'
                              and rg.is_executable=1
                              and rg.report_owner=rus.user_id
                              and fo.folder_owner=fus.user_id
                              and rg.folder_id=fo.folder_id
                              and rg.report_name not like 'Admin%'
                              order by fo.folder_name, rg.report_name";
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
                        var result = new BSIFreeFormReportDTO();
                        result.FolderId= Decimal.ToInt32((Decimal)reader[0]);
                        result.ReportId = Decimal.ToInt32((Decimal)reader[1]);
                        result.FolderName = (string)reader[2];
                        result.ReportName = (string)reader[3];
                        result.ReportDescription = (reader[4] == DBNull.Value) ? null : (string)reader[4];
                        result.FolderOwnerId= Decimal.ToInt32((Decimal)reader[6]);
                        result.FolderOwnerName = (string) reader[7];
                        result.OwnerId = Decimal.ToInt32((Decimal)reader[8]);
                        result.OwnerName = (string)reader[9];
                        var xml = (string)reader[5];
                        XDocument xdoc = XDocument.Parse(xml);
                        var queryString = xdoc.Element("REPORT_GROUP").Element("REPORT_ITEM").Element("REPORT").Element("query_string").Value;
                        queryString = queryString.Replace("&gt;", ">");
                        queryString = queryString.Replace("&lt;", "<");
                        result.Query = queryString;
                        var custom = xdoc.Element("REPORT_GROUP").Element("REPORT_ITEM").Element("REPORT").Element("custom");
                        if (custom != null && custom.Element("query") != null)
                        {
                            var elements = custom.Element("query").Element("params").Elements();
                            foreach (var elem in elements)
                            {
                                var paramName = elem.Attribute("name").Value;
                                result.Parameters.Add(new KeyValuePairDTO()
                                {
                                    Key = paramName,
                                    Value = ""
                                });
                            }
                        }

                        results.Add(result);
                    }
                }
            }
            return results;
        }

        public List<OracleUserDTO> GetUser(int id, string name)
        {
            try
            {
                string query = @"select
                                user_id,
                                user_name,
                                user_email,
                                user_group_id,
                                user_group_name
                                from t_users
                                left join t_user_group_members using (user_id)
                                left join t_user_groups using (user_group_id)
                                where 1=1
                                ";
                if (!string.IsNullOrEmpty(name))
                {
                    query += " and LOWER(user_name) LIKE LOWER('%' || :user_name || '%')";
                }
                if (id != 0)
                {
                    query += " and user_id = :user_id";
                }

                using (OracleConnection con = new OracleConnection(_connectionstring))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = query;
                        OracleParameter param1 = new OracleParameter("user_id", id);
                        OracleParameter param2 = new OracleParameter("user_name", name);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        OracleDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        List<DataRow> list = dt.AsEnumerable().ToList();
                        var values = list.GroupBy(o => new { user_id = o[0], user_name = o[1], user_email = o[2] }).Select(p => new OracleUserDTO()
                        {
                            user_id = Decimal.ToInt32((Decimal)p.Key.user_id),
                            user_name = (string)p.Key.user_name,
                            user_email = (p.Key.user_email == DBNull.Value) ? string.Empty : (string)p.Key.user_email,
                            groups = p.Select(q =>
                            (q[3] == DBNull.Value) ? null :
                            new OracleUserGroupsDTO()
                            {
                                user_group_id = (q[3] == DBNull.Value) ? (int?)null : Decimal.ToInt32((Decimal)q[3]),
                                user_group_name = (q[4] == DBNull.Value) ? string.Empty : (string)q[4]
                            }).ToList()
                        });

                        return values.ToList();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Tuple<int, int> GetUserIdLocaleIdByUserName(string username)
        {
            try
            {
                string query = @"SELECT USER_ID, USER_LOCALE_ID FROM T_Users Where USER_NAME = :user_name AND USER_STATUS = 'ACTIVE'";

                using (OracleConnection con = new OracleConnection(_connectionstring))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = query;
                        OracleParameter param2 = new OracleParameter("user_name", username);
                        cmd.Parameters.Add(param2);
                        OracleDataReader reader = cmd.ExecuteReader();
                        Tuple<int, int> res = new Tuple<int, int>(0, 0);
                        if (reader.Read())
                        {
                            res = new Tuple<int, int>(Decimal.ToInt32((Decimal)reader["USER_ID"]), Decimal.ToInt32((Decimal)reader["USER_LOCALE_ID"]));
                        }
                        return res;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<FormConfigurationDTO> GetFormConfiguration(string schema)
        {
            //string str = "<DataLoadingForms><ControlsXml><Xml>*securestring*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</Xml></ControlsXml></DataLoadingForms>";
            var dto = new APIToWorkflowDTO() { input = schema };
            try
            {
                using (var client = new HttpClient())
                {
                    var bsiconf = _informationService.GetConfiguration("be_bsi", "bsi_api_url");
                    //List<string> data = new List<string>() { dto.primary_contract_party + "", dto.secondary_contract_party + "", dto.contract_name, dto.kpi_name, dto.id_ticket, dto.period, dto.ticket_status };
                    var output = QuantisUtilities.FixHttpURLForCall(bsiconf.Value, "/home/APIToWorkflow");
                    client.BaseAddress = new Uri(output.Item1);
                    var dataAsString = JsonConvert.SerializeObject(dto);
                    var content = new StringContent(dataAsString);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = client.PostAsync(output.Item2, content).Result;
                    //var response = client.PostAsync(output.Item2, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        //if (response.Content.ReadAsStringAsync().Result == "True")
                        //string x = response.Content.ReadAsStringAsync().Result;
                        string xml = response.Content.ReadAsStringAsync().Result;
                        string xmlDecoded = HttpUtility.HtmlDecode(xml);
                        XDocument xdoc = XDocument.Parse(xmlDecoded);
                        var lists = from uoslist in xdoc.Element("DataLoadingForms").Element("ControlsXml").Element("Xml").Element("Controls").Elements("Control") select uoslist;
                        //var labelList = new List<FormConfigurationDTO>();
                        var labelList = lists.Where(o => o.Attribute("type").Value == "DLFLabel").Select(l => new
                        {
                            a_id = l.Attribute("id").Value,
                            a_top = l.Attribute("top").Value,
                            a_left = l.Attribute("left").Value,
                            a_width = l.Attribute("width").Value,
                            a_height = l.Attribute("height").Value,
                            text = l.Element("text").Value,
                            a_isMandatoryLabel = l.Attribute("isMandatoryLabel").Value,
                            a_type = l.Attribute("type").Value
                        }).ToList();
                        var formfields = lists.Where(o => o.Attribute("type").Value != "DLFLabel").Select(l => new
                        {
                            a_id = l.Attribute("id").Value,
                            //useless a_name = l.Attribute("name").Value,
                            a_top = l.Attribute("top").Value,
                            a_left = l.Attribute("left").Value,
                            a_width = l.Attribute("width").Value,
                            a_height = l.Attribute("height").Value,
                            a_type = l.Attribute("type").Value,
                            a_dataType = l.Attribute("dataType").Value,
                            name = l.Element("name").Value,
                            text = (l.Attribute("type").Value == "DLFLabel") ? l.Element("text").Value
                                          : (l.Attribute("type").Value == "DLFCheckBox") ? l.Element("text").Value : null,

                            a_isMandatoryLabel = (l.Attribute("type").Value == "DLFLabel") ? l.Attribute("isMandatoryLabel").Value : null,

                            a_controllerDataType = (l.Attribute("type").Value == "DLFTextBox") ? l.Attribute("controllerDataType").Value
                                                         : (l.Attribute("type").Value == "DLFCheckBox") ? l.Attribute("controllerDataType").Value : null,

                            defaultValue = (l.Attribute("defaultValue") != null) ? l.Element("defaultValue").Value : null,

                            a_maxLength = (l.Attribute("type").Value == "DLFTextBox") ? l.Attribute("maxLength").Value : null,
                            a_isMandatory = (l.Attribute("type").Value == "DLFTextBox") ? l.Attribute("isMandatory").Value
                                                  : (l.Attribute("type").Value == "DLFDatePicker") ? l.Attribute("isMandatory").Value : null,

                            a_labelId = (l.Attribute("type").Value == "DLFTextBox") ? l.Attribute("labelId").Value
                                              : (l.Attribute("type").Value == "DLFDatePicker") ? l.Attribute("labelId").Value : null,

                            //a_checkedStatus = (l.Attribute("type").Value == "DLFCheckBox") ? l.Attribute("checkedStatus").Value : null,
                            a_checkedValue = (l.Attribute("checkedValue") != null) ? l.Element("checkedValue").Value : null,
                            a_unCheckedValue = (l.Attribute("unCheckedValue") != null) ? l.Element("unCheckedValue").Value : null,
                        });
                        //foreach (var l in lists)
                        //{
                        //    if (l.Attribute("type").Value == "DLFLabel")
                        //    {
                        //        labelList.Add(new FormConfigurationDTO()
                        //        {
                        //            a_id = l.Attribute("id").Value,
                        //            a_top = l.Attribute("top").Value,
                        //            a_left = l.Attribute("left").Value,
                        //            a_width = l.Attribute("width").Value,
                        //            a_height = l.Attribute("height").Value,
                        //            text = l.Element("text").Value,
                        //            a_isMandatoryLabel = l.Attribute("isMandatoryLabel").Value,
                        //            a_type = l.Attribute("type").Value
                        //        }
                        //        );
                        //    }
                        //    else
                        //    {
                        //        formfields.Add(new FormConfigurationDTO()
                        //        {
                        //            a_id = l.Attribute("id").Value,
                        //            //useless a_name = l.Attribute("name").Value,
                        //            a_top = l.Attribute("top").Value,
                        //            a_left = l.Attribute("left").Value,
                        //            a_width = l.Attribute("width").Value,
                        //            a_height = l.Attribute("height").Value,
                        //            a_type = l.Attribute("type").Value,
                        //            //useless a_fontColor = l.Attribute("fontColor").Value,
                        //            //useless a_fontFamily = l.Attribute("fontFamily").Value,
                        //            //useless a_fontWeight = l.Attribute("fontWeight").Value,
                        //            //useless a_fontItalic = l.Attribute("fontItalic").Value,
                        //            //useless a_textDecoration = l.Attribute("textDecoration").Value,
                        //            //useless a_fontSize = l.Attribute("fontSize").Value,
                        //            //useless a_backgrounColor = l.Attribute("backgroundColor").Value,
                        //            //useless a_isDefaultFontColor = l.Attribute("isDefaultFontColor").Value,
                        //            //useless a_isDefaultBGColor = l.Attribute("isDefaultBGColor").Value,
                        //            //useless a_text = (l.Attribute("type").Value == "DLFLabel") ? l.Attribute("text").Value : null,

                        //            a_dataType = l.Attribute("dataType").Value,
                        //            name = l.Element("name").Value,
                        //            text = (l.Attribute("type").Value == "DLFLabel") ? l.Element("text").Value
                        //                  : (l.Attribute("type").Value == "DLFCheckBox") ? l.Element("text").Value : null,

                        //            a_isMandatoryLabel = (l.Attribute("type").Value == "DLFLabel") ? l.Attribute("isMandatoryLabel").Value : null,

                        //            a_controllerDataType = (l.Attribute("type").Value == "DLFTextBox") ? l.Attribute("controllerDataType").Value
                        //                                 : (l.Attribute("type").Value == "DLFCheckBox") ? l.Attribute("controllerDataType").Value : null,

                        //            a_defaultValue = (l.Attribute("type").Value == "DLFTextBox") ? l.Attribute("defaultValue").Value
                        //                           : (l.Attribute("type").Value == "DLFDatePicker") ? l.Attribute("defaultValue").Value : null,

                        //            a_maxLength = (l.Attribute("type").Value == "DLFTextBox") ? l.Attribute("maxLength").Value : null,
                        //            a_isMandatory = (l.Attribute("type").Value == "DLFTextBox") ? l.Attribute("isMandatory").Value
                        //                          : (l.Attribute("type").Value == "DLFDatePicker") ? l.Attribute("isMandatory").Value : null,

                        //            a_labelId = (l.Attribute("type").Value == "DLFTextBox") ? l.Attribute("labelId").Value
                        //                      : (l.Attribute("type").Value == "DLFDatePicker") ? l.Attribute("labelId").Value : null,

                        //            a_checkedStatus = (l.Attribute("type").Value == "DLFCheckBox") ? l.Attribute("checkedStatus").Value : null,
                        //            a_checkedValue = (l.Attribute("type").Value == "DLFCheckBox") ? l.Attribute("checkedValue").Value : null,
                        //            a_unCheckedValue = (l.Attribute("type").Value == "DLFCheckBox") ? l.Attribute("unCheckedValue").Value : null,

                        //            /*if (a_type == "DLFLabel"){
                        //                  a_text = l.Attribute("text").Value,
                        //                  a_isMandatoryLabel = l.Attribute("isMandatoryLabel").Value,
                        //              }
                        //              if (a_type == "DLFTextBox") {
                        //                  a_controllerDataType = l.Attribute("controllerDataType").Value,
                        //                  a_defaultValue = l.Attribute("defaultValue").Value,
                        //                  a_maxLength = l.Attribute("maxLength").Value,
                        //                  a_isMandatory = l.Attribute("isMandatory").Value,
                        //                  a_labelId = l.Attribute("labelId").Value,
                        //               }
                        //               if (a_type == "DLFDatePicker"){
                        //                  a_defaultValue = l.Attribute("defaultValue").Value,
                        //                  a_showLegend = l.Attribute("showLegend").Value,
                        //                  a_isMandatory = l.Attribute("isMandatory").Value,
                        //                  a_labelId = l.Attribute("labelId").Value,
                        //                }
                        //                if (a_type == "DLFCheckBox"){
                        //                  a_text = l.Attribute("text").Value,
                        //                  a_controllerDataType = l.Attribute("controllerDataType").Value,
                        //                  a_checkedStatus = l.Attribute("checkedStatus").Value,
                        //                  a_checkedValue = l.Attribute("checkedValue").Value,
                        //                  a_unCheckedValue = l.Attribute("unCheckedValue").Value,
                        //                } */

                        //        });
                        //    }

                        //}

                        //name: "Trans_CICS_Apert_Libr_no_abend"
                        //source: "event"
                        //type: "real"

                        var outputs = new List<FormConfigurationDTO>();
                        formfields = formfields.OrderBy(o => Int32.Parse(o.a_top)).ToList();
                        foreach (var f in formfields)
                        {
                            var fields = new FormConfigurationDTO()
                            {
                                a_dataType = f.a_dataType,
                                a_isMandatory = f.a_isMandatory,
                                name = f.name,
                                a_type = f.a_type,
                                defaultValue = f.defaultValue,
                            };
                            if (fields.a_type == "DLFCheckBox")
                            {
                                fields.Extras.Add("a_checkedValue", f.a_checkedValue);
                                fields.Extras.Add("a_unCheckedValue", f.a_unCheckedValue);
                            }

                            var label = labelList.FirstOrDefault(o => o.a_id == f.a_labelId ||
                            (
                            (Int32.Parse(o.a_top) + Int32.Parse(o.a_height)) >= Int32.Parse(f.a_top) - 10 &&
                            Int32.Parse(o.a_top) <= (Int32.Parse(f.a_top) + Int32.Parse(f.a_height))
                            ));
                            if (label != null)
                            {
                                fields.text = label.text;
                                labelList.Remove(label);
                            }
                            outputs.Add(fields);
                        }

                        outputs.AddRange(labelList.Select(o => new FormConfigurationDTO()
                        {
                            a_type = o.a_type,
                            text = o.text
                        }));
                        return outputs;
                    }

                    _dbcontext.LogInformation(string.Format("Call to API has failed. BaseURL: {0} APIPath: {1} Data:{2}", output.Item1, output.Item2, dataAsString));
                    return new List<FormConfigurationDTO>();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}