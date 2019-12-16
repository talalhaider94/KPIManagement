using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.Monitoring;

namespace Quantis.WorkFlow.APIBase.API
{
    public class MonitoringService:IMonitoringService
    {
        private readonly WorkFlowPostgreSqlContext _dbcontext;
        private readonly IConfiguration _configuration;
        public MonitoringService(WorkFlowPostgreSqlContext dbcontext,
            IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        public List<MonitoringDTO> GetTicketsMonitoringByPeriod(string period)
        {
            var periodDate=new DateTime(int.Parse(period.Split('/')[1]), int.Parse(period.Split('/')[0]), 1);
            string query = @"select r.rule_name,ck.global_rule_id_bsi,m.sla_id,m.sla_name,c.customer_id,c.customer_name,CASE WHEN ck.organization_unit='' THEN null ELSE CAST (ck.organization_unit AS INTEGER) END as organization_unit_id, ou.organization_unit,ck.day_workflow,ck.month
                            from t_catalog_kpis ck
                            left join t_rules r on r.global_rule_id=ck.global_rule_id_bsi
                            left join t_sla_versions s on r.sla_version_id = s.sla_version_id 
                            left join t_slas m on m.sla_id = s.sla_id
                            left join t_customers c on m.customer_id=c.customer_id
                            left join t_organization_units ou on CASE WHEN ck.organization_unit='' THEN null ELSE CAST (ck.organization_unit AS INTEGER) END=ou.id
                            where s.sla_status = 'EFFECTIVE' AND m.sla_status = 'EFFECTIVE'
                            and ck.month is not null
                            and ck.enable=true
                            and ck.day_workflow!=0";
            using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
            {
                var baseResult = new List<BaseMonitoringDTO>();
                con.Open();
                var command = new NpgsqlCommand(query, con);
                command.CommandType = CommandType.Text;
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        var monitorDTO = new BaseMonitoringDTO();
                        monitorDTO.GlobalRuleName = result.GetString(result.GetOrdinal("rule_name"));
                        monitorDTO.GlobalRuleId = result.GetInt32(result.GetOrdinal("global_rule_id_bsi"));
                        monitorDTO.ContractId= result.GetInt32(result.GetOrdinal("sla_id"));
                        monitorDTO.ContractName = result.GetString(result.GetOrdinal("sla_name"));
                        monitorDTO.ContractPartyId = result.GetInt32(result.GetOrdinal("customer_id"));
                        monitorDTO.ContractPartyName = result.GetString(result.GetOrdinal("customer_name"));
                        monitorDTO.OrganizationUnitId = (result[result.GetOrdinal("organization_unit_id")]==DBNull.Value)?null: (int?)result.GetInt32(result.GetOrdinal("organization_unit_id"));
                        monitorDTO.OrganizationUnitName = (result[result.GetOrdinal("organization_unit")] == DBNull.Value) ? null : result.GetString(result.GetOrdinal("organization_unit"));
                        monitorDTO.WorkflowDay = result.GetInt32(result.GetOrdinal("day_workflow"));
                        monitorDTO.Months = result.GetString(result.GetOrdinal("month"));
                        monitorDTO.TicketCreated = false;
                        baseResult.Add(monitorDTO);
                    }

                    baseResult= baseResult.Where(o => o.Months.Split(',').Contains(DateTime.Now.Month + "")).ToList();
                    var globalRuleIds = baseResult.Select(o => o.GlobalRuleId).ToList();
                    var ticketsCreated = _dbcontext.SDMTicketFact.Where(o => o.period_year == periodDate.Year && o.period_month == periodDate.Month && globalRuleIds.Contains(o.global_rule_id)).ToList();
                    foreach (var b in baseResult)
                    {
                        var ticket=ticketsCreated.FirstOrDefault(o => o.global_rule_id == b.GlobalRuleId);
                        if (ticket != null)
                        {
                            b.TicketCreated = true;
                        }
                    }
                    var returnResult = baseResult
                        .GroupBy(o => new
                        {
                            o.ContractPartyId, o.ContractPartyName, o.ContractId, o.ContractName, o.OrganizationUnitId,
                            o.OrganizationUnitName
                        }).Select(o => new MonitoringDTO()
                        {
                            ContractId = o.Key.ContractId,
                            ContractName = o.Key.ContractName,
                            OrganizationUnitName = o.Key.OrganizationUnitName,
                            OrganizationUnitId = o.Key.OrganizationUnitId,
                            ContractPartyName = o.Key.ContractPartyName,
                            ContractPartyId = o.Key.ContractPartyId,
                            NoOfTicketsToBeOpenedForCompletePeriod = o.Count(),
                            NoOfTicketsToBeOpenedTillToday = o.Count(p=>p.WorkflowDay<=DateTime.Now.Day),
                            NoOfTicketsOpenedTillToday = o.Count(p=>p.TicketCreated),
                            TicketsToBeOpenedForCompletePeriod = o.Select(p=>new MonitoringKPIDTO()
                            {
                                GlobalRuleId = p.GlobalRuleId,
                                ContractId = o.Key.ContractId,
                                WorkflowDay = p.WorkflowDay,
                                ContractName = o.Key.ContractName,
                                OrganizationUnitId = o.Key.OrganizationUnitId,
                                OrganizationUnitName = o.Key.OrganizationUnitName,
                                ContractPartyName = o.Key.ContractPartyName,
                                ContractPartyId = o.Key.ContractPartyId,
                                GlobalRuleName = p.GlobalRuleName
                            }).ToList(),
                            TicketsToBeOpenedTillToday = o.Where(q => q.WorkflowDay <= DateTime.Now.Day).Select(p => new MonitoringKPIDTO()
                            {
                                GlobalRuleId = p.GlobalRuleId,
                                ContractId = o.Key.ContractId,
                                WorkflowDay = p.WorkflowDay,
                                ContractName = o.Key.ContractName,
                                OrganizationUnitId = o.Key.OrganizationUnitId,
                                OrganizationUnitName = o.Key.OrganizationUnitName,
                                ContractPartyName = o.Key.ContractPartyName,
                                ContractPartyId = o.Key.ContractPartyId,
                                GlobalRuleName = p.GlobalRuleName
                            }).ToList(),
                            TicketsOpenedTillToday = ticketsCreated.Where(p=>o.Select(q=>q.GlobalRuleId).Contains(p.global_rule_id)).Select(r=>new MonitoringTicketDTO()
                            {
                                GlobalRuleId = r.global_rule_id,
                                ContractName = o.Key.ContractName,
                                OrganizationUnitId = o.Key.OrganizationUnitId,
                                OrganizationUnitName = o.Key.OrganizationUnitName,
                                ContractPartyName = o.Key.ContractPartyName,
                                ContractPartyId = o.Key.ContractPartyId,
                                ContractId = o.Key.ContractId,
                                CreatedOn = r.created_on,
                                ResultValue = r.result_value,
                                TicketId = r.ticket_id,
                                TicketRefNumber = r.ticket_refnum
                                
                            }).ToList()
                        }).ToList();
                    return returnResult;
                }
            }
        }
        public List<MonitoringDayLevelDTO> GetDayLevelTicketsMonitoring(string period)
        {
            var periodDate = new DateTime(int.Parse(period.Split('/')[1]), int.Parse(period.Split('/')[0]), 1);
            var Finalresult = new List<MonitoringDayLevelDTO>();
            string query = @"select r.rule_name,ck.global_rule_id_bsi,m.sla_id,m.sla_name,c.customer_id,c.customer_name,CASE WHEN ck.organization_unit='' THEN null ELSE CAST (ck.organization_unit AS INTEGER) END as organization_unit_id, ou.organization_unit,ck.day_workflow,ck.month
                            from t_catalog_kpis ck
                            left join t_rules r on r.global_rule_id=ck.global_rule_id_bsi
                            left join t_sla_versions s on r.sla_version_id = s.sla_version_id 
                            left join t_slas m on m.sla_id = s.sla_id
                            left join t_customers c on m.customer_id=c.customer_id
                            left join t_organization_units ou on CASE WHEN ck.organization_unit='' THEN null ELSE CAST (ck.organization_unit AS INTEGER) END=ou.id
                            where s.sla_status = 'EFFECTIVE' AND m.sla_status = 'EFFECTIVE'
                            and ck.month is not null
                            and ck.enable=true
                            and ck.day_workflow!=0";
            using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
            {
                var baseResult = new List<BaseMonitoringDTO>();
                con.Open();
                var command = new NpgsqlCommand(query, con);
                command.CommandType = CommandType.Text;
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        var monitorDTO = new BaseMonitoringDTO();
                        monitorDTO.GlobalRuleName = result.GetString(result.GetOrdinal("rule_name"));
                        monitorDTO.GlobalRuleId = result.GetInt32(result.GetOrdinal("global_rule_id_bsi"));
                        monitorDTO.ContractId = result.GetInt32(result.GetOrdinal("sla_id"));
                        monitorDTO.ContractName = result.GetString(result.GetOrdinal("sla_name"));
                        monitorDTO.ContractPartyId = result.GetInt32(result.GetOrdinal("customer_id"));
                        monitorDTO.ContractPartyName = result.GetString(result.GetOrdinal("customer_name"));
                        monitorDTO.OrganizationUnitId = (result[result.GetOrdinal("organization_unit_id")] == DBNull.Value) ? null : (int?)result.GetInt32(result.GetOrdinal("organization_unit_id"));
                        monitorDTO.OrganizationUnitName = (result[result.GetOrdinal("organization_unit")] == DBNull.Value) ? null : result.GetString(result.GetOrdinal("organization_unit"));
                        monitorDTO.WorkflowDay = result.GetInt32(result.GetOrdinal("day_workflow"));
                        monitorDTO.Months = result.GetString(result.GetOrdinal("month"));
                        monitorDTO.TicketCreated = false;
                        baseResult.Add(monitorDTO);
                    }

                    baseResult = baseResult.Where(o => o.Months.Split(',').Contains(DateTime.Now.Month + "")).ToList();
                    var globalRuleIds = baseResult.Select(o => o.GlobalRuleId).ToList();
                    var ticketsCreated = _dbcontext.SDMTicketFact.Where(o => o.period_year == periodDate.Year && o.period_month == periodDate.Month && globalRuleIds.Contains(o.global_rule_id)).ToList();

                    int daysInMonth=DateTime.DaysInMonth(periodDate.Year, periodDate.Month);
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        var res = new MonitoringDayLevelDTO()
                        {
                            DayNumber = day,
                            NoOfTicketsToBeOpenedToday = baseResult.Count(o => o.WorkflowDay == day),
                            NoOfTicketsOpenedToday = ticketsCreated.Count(o => o.created_on.Day == day),
                            TicketsToBeOpenedToday = baseResult.Where(o => o.WorkflowDay == day).Select(p =>
                                new MonitoringKPIDTO()
                                {
                                    ContractName = p.ContractName,
                                    OrganizationUnitId = p.OrganizationUnitId,
                                    OrganizationUnitName = p.OrganizationUnitName,
                                    ContractPartyName = p.ContractPartyName,
                                    ContractPartyId = p.ContractPartyId,
                                    ContractId = p.ContractId,
                                    GlobalRuleId = p.GlobalRuleId,
                                    WorkflowDay = p.WorkflowDay,
                                    GlobalRuleName = p.GlobalRuleName
                                }).ToList(),
                            TicketsOpenedToday = ticketsCreated.Where(o => o.created_on.Day == day).Select(p =>
                                new MonitoringTicketDTO()
                                {
                                    OrganizationUnitId = baseResult.FirstOrDefault(o=>o.GlobalRuleId==p.global_rule_id).OrganizationUnitId,
                                    GlobalRuleId = p.global_rule_id,
                                    OrganizationUnitName = baseResult.FirstOrDefault(o => o.GlobalRuleId == p.global_rule_id).OrganizationUnitName,
                                    ContractPartyName = baseResult.FirstOrDefault(o => o.GlobalRuleId == p.global_rule_id).ContractPartyName,
                                    ContractPartyId = baseResult.FirstOrDefault(o => o.GlobalRuleId == p.global_rule_id).ContractPartyId,
                                    ContractId = baseResult.FirstOrDefault(o => o.GlobalRuleId == p.global_rule_id).ContractId,
                                    ContractName = baseResult.FirstOrDefault(o => o.GlobalRuleId == p.global_rule_id).ContractName,
                                    CreatedOn = p.created_on,
                                    TicketId = p.ticket_id,
                                    ResultValue = p.result_value,
                                    TicketRefNumber = p.ticket_refnum
                                }).ToList()
                        };
                        Finalresult.Add(res);
                    }
                    return Finalresult;
                }
            }
        }

        public DataTable ExecuteLocalDatabase(ExecuteLocalDatabaseDTO dto)
        {
            using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
            {
                con.Open();
                var command = new NpgsqlCommand(dto.Query, con);
                command.CommandType = CommandType.Text;
                _dbcontext.Database.OpenConnection();
                var result = command.ExecuteReader();
                DataTable myTable = new DataTable();
                myTable.Load(result);
                return myTable;
            }
        }


    }
}
