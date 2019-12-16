using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models.SDM;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.API;
using Quantis.WorkFlow.Services.DTOs.BusinessLogic;
using Quantis.WorkFlow.Services.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace Quantis.WorkFlow.APIBase.API
{
    public class ServiceDeskManagerService : IServiceDeskManagerService
    {
        private static readonly object _object = new object();
        private readonly SDM.USD_WebServiceSoapClient _sdmClient = null;
        private readonly SDMExt.USD_R11_ExtSoapClient _sdmExtClient = null;
        private int _sid { get; set; }
        private readonly string _username;
        private readonly string _password;
        private readonly List<SDM_TicketGroup> _groupMapping;
        private readonly List<SDM_TicketStatus> _statusMapping;
        private readonly IDataService _dataService;
        private readonly WorkFlowPostgreSqlContext _dbcontext;
        private readonly IInformationService _infomationAPI;
        private readonly IConfiguration _configuration;

        private void LogIn()
        {
            try
            {
                if (_sid == -1)
                {
                    var login_a = _sdmClient.loginAsync(_username, _password);
                    login_a.Wait();
                    _sid = login_a.Result.loginReturn;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void LogOut()
        {
            try
            {
                if (_sid != -1)
                {
                    try
                    {
                        _sdmClient.logoutAsync(_sid).Wait();
                        _sid = -1;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ServiceDeskManagerService(WorkFlowPostgreSqlContext context, IDataService dataService, IInformationService infomationAPI, IConfiguration configuration)
        {
            _dbcontext = context;
            _infomationAPI = infomationAPI;
            _configuration = configuration;
            _groupMapping = _dbcontext.SDMTicketGroup.ToList();
            _statusMapping = _dbcontext.SDMTicketStatus.OrderBy(o => o.step).ToList();

            if (_sdmClient == null)
            {
                _sdmClient = new SDM.USD_WebServiceSoapClient(SDM.USD_WebServiceSoapClient.EndpointConfiguration.USD_WebServiceSoap, _configuration["SDMWebServices"]);
            }
            if (_sdmExtClient == null)
            {
                _sdmExtClient = new SDMExt.USD_R11_ExtSoapClient(SDMExt.USD_R11_ExtSoapClient.EndpointConfiguration.USD_R11_ExtSoap, _configuration["SDMExtWebServices"]);
            }
            _dataService = dataService;
            var usernameObj = _infomationAPI.GetConfiguration("be_sdm", "username");
            var passObj = _infomationAPI.GetConfiguration("be_sdm", "password");
            if (usernameObj == null || passObj == null)
            {
                var exp = new Exception("Cannot get SDM login Properties");
                throw exp;
            }
            else
            {
                _sid = -1;
                _username = usernameObj.Value;
                _password = passObj.Value;
            }
        }

        public List<SDMTicketLVDTO> GetAllTickets()
        {
            List<SDMTicketLVDTO> ret = null;
            LogIn();
            try
            {
                var select_a = _sdmClient.doSelectAsync(_sid, "cr", "", 99999, new string[] { "ref_num", "description", "group", "summary", "status", "zz_mgnote", "zz_cned_string1", "zz_cned_string2", "zz_cned_string3", "zz_cned_string4", "zz_string1", "zz_string2", "zz_string3", "last_mod_dt" });
                select_a.Wait();
                var select_result = select_a.Result.doSelectReturn;
                ret = parseTickets(select_result);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                LogOut();
            }
            return ret;
        }

        public List<SDMAttachmentDTO> GetAttachmentsByTicket(int ticketId)
        {
            List<SDMAttachmentDTO> ret = null;
            LogIn();
            try
            {
                var selecta = _sdmClient.doSelectAsync(_sid, "lrel_attachments_requests", "cr='cr:" + ticketId + "'", 99999, new string[] { "attmnt", "attmnt.attmnt_name", "last_mod_dt" });
                selecta.Wait();
                var sel = selecta.Result.doSelectReturn;
                ret = parseAttachments(sel);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                LogOut();
            }
            return ret.OrderByDescending(o => o.LastModifiedDate).ToList();
        }

        public byte[] DownloadAttachment(string attachmentHandle)
        {
            byte[] ret = null;
            LogIn();
            try
            {
                var select_a = _sdmExtClient.downloadAttachmentAsync(_sid, "attmnt:" + attachmentHandle);

                select_a.Wait();
                var select_result = select_a.Result.Body.downloadAttachmentResult;
                ret = select_result;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                LogOut();
            }
            return ret;
        }

        public SDMTicketLVDTO GetTicketByID(int Id)
        {
            LogIn();
            try
            {
                var select_a = _sdmClient.doSelectAsync(_sid, "cr", "id=" + Id + "", 1, new string[] { "ref_num", "description", "group", "summary", "status", "zz_mgnote", "zz_cned_string1", "zz_cned_string2", "zz_cned_string3", "zz_cned_string4", "zz_string1", "zz_string2", "zz_string3", "zz_string1", "zz_string2", "zz_string3", "last_mod_dt" });
                select_a.Wait();
                var select_result = select_a.Result.doSelectReturn;
                return parseTickets(select_result).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                LogOut();
            }
        }

        public SDMTicketLVDTO CreateTicket(CreateTicketDTO dto)
        {
            SDMTicketLVDTO ret = null;
            LogIn();
            try
            {
                if (string.IsNullOrEmpty(dto.Status))
                {
                    dto.Status = _statusMapping.FirstOrDefault().handle;
                }
                dto.Group = _groupMapping.Where(o => o.category_id == dto.GroupCategoryId).OrderBy(o => o.step).First().handle;
                string newRequestHandle = "";
                string newRequestNumber = "";
                var ticket = _sdmClient.createRequestAsync(new SDM.createRequestRequest(_sid, "",
                    new string[34]
                    {"type",
                      "crt:180",
                      "customer",
                      "cnt:9FF6A914066D09479BACC3736FBFFD21",
                      "zz_svc",
                      "zsvc:401021",
                      "category",
                      "pcat:148400475",
                      "summary",
                      dto.Summary,
                      "description",
                      dto.Description,
                      "status",
                      dto.Status,
                      "priority",
                      "pri:500",
                      "urgency",
                      "urg:1100",
                      "severity",
                      "sev:800",
                      "impact",
                      "imp:1603",
                      "group",
                      dto.Group,
                      "zz_mgnote",
                      dto.ID_KPI,
                      "zz_cned_string1",
                      dto.Reference1,
                      "zz_cned_string2",
                      dto.Reference2,
                      "zz_cned_string3",
                      dto.Reference3,
                      "zz_cned_string4",
                      dto.Period,
                    }, new string[0], "", new string[0], newRequestHandle, newRequestNumber)).Result.createRequestReturn;

                ret = parseNewTicket(ticket);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                LogOut();
            }
            return ret;
        }

        public SDMTicketLVDTO CreateTicketByKPIID(int Id)
        {
            bool ticketCreated = false;
            SDMTicketLVDTO ret = null;
            LogIn();
            try
            {
                var dto = _dataService.GetKPICredentialToCreateTicket(Id);
                if (string.IsNullOrEmpty(dto.Status))
                {
                    dto.Status = _statusMapping.FirstOrDefault().handle;
                }
                dto.Group = _groupMapping.Where(o => o.category_id == dto.GroupCategoryId).OrderBy(o => o.step).First().handle;
                string newRequestHandle = "";
                string newRequestNumber = "";
                var ticket = _sdmClient.createRequestAsync(new SDM.createRequestRequest(_sid, "",
                    new string[40]
                    {"type",
                      "crt:180",
                      "customer",
                      dto.Customer,
                      "zz_svc",
                      "zsvc:401021",
                      "category",
                      "pcat:148400475",
                      "summary",
                      dto.Summary,
                      "description",
                      dto.Description,
                      "status",
                      dto.Status,
                      "priority",
                      "pri:500",
                      "urgency",
                      "urg:1100",
                      "severity",
                      "sev:800",
                      "impact",
                      "imp:1603",
                      "group",
                      dto.Group,
                      "zz_mgnote",
                      dto.ID_KPI,
                      "zz_cned_string1",
                      dto.Reference1,
                      "zz_cned_string2",
                      dto.Reference2,
                      "zz_cned_string3",
                      dto.Reference3,
                      "zz_cned_string4",
                      dto.Period,
                      "zz_string1",
                      dto.zz1_contractParties,
                      "zz_string2",
                      dto.zz2_calcValue,
                      "zz_string3",
                      dto.zz3_KpiIds
                    }, new string[0], "", new string[0], newRequestHandle, newRequestNumber)).Result.createRequestReturn;
                ticketCreated = true;
                ret = parseNewTicket(ticket);
                var sdm_fact = new SDM_TicketFact()
                {
                    complaint = (dto.Description.IndexOf("[Compliant]") != -1),
                    created_on = DateTime.Now,
                    global_rule_id = int.Parse(dto.zz3_KpiIds.Split('|')[1]),
                    notcalculated = (dto.Description.IndexOf("[NE]") != -1),
                    notcomplaint = (dto.Description.IndexOf("[Non Compliant]") != -1),
                    period_month = DateTime.Now.AddMonths(-1).Month,
                    period_year = DateTime.Now.AddMonths(-1).Year,
                    primary_contract_party_id = dto.GroupCategoryId,
                    refused = false,
                    result_value = dto.zz2_calcValue,
                    secondary_contract_party_id = dto.SecondaryContractParty,
                    ticket_id = int.Parse(ret.Id),
                    ticket_refnum = int.Parse(ret.ref_num),
                    customer_id = _infomationAPI.GetContractIdByGlobalRuleId(int.Parse(dto.zz3_KpiIds.Split('|')[1]))
                };
                var deleteRecord = _dbcontext.SDMTicketExceptions.FirstOrDefault(o =>
                    o.global_rule_id == sdm_fact.global_rule_id && o.period_month == sdm_fact.period_month &&
                    o.period_year == sdm_fact.period_year);
                if (deleteRecord != null)
                {
                    _dbcontext.SDMTicketExceptions.Remove(deleteRecord);
                    _dbcontext.SaveChanges();
                }
                _dbcontext.SDMTicketFact.Add(sdm_fact);
                _dbcontext.SaveChanges();
                var attachments = _dataService.GetAttachmentsByKPIID(Id);
                foreach (var att in attachments)
                {
                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("sid", _sid + "");
                    param.Add("repositoryHandle", "doc_rep:1002");
                    param.Add("objectHandle", "cr:" + ret.Id);
                    param.Add("description", att.doc_name);
                    param.Add("fileName", att.doc_name);
                    SendSOAPRequest(_sdmClient.InnerChannel.RemoteAddress.ToString(), "createAttachment", param, att.content);
                }
                _dbcontext.SaveChanges();
            }
            catch (Exception e)
            {
                if (!ticketCreated)
                {
                    var entity = new SDM_TicketException()
                    {

                        period_month = DateTime.Now.AddMonths(-1).Month,
                        period_year = DateTime.Now.AddMonths(-1).Year
                    };
                    var kpi = _dbcontext.CatalogKpi.FirstOrDefault(o => o.id == Id);
                    if (kpi != null)
                    {
                        entity.global_rule_id = kpi.global_rule_id_bsi;
                        entity.exception_text = e.Message + "||||";
                        var inner_exception = e.InnerException;
                        while (inner_exception != null)
                        {
                            entity.exception_text += ">>>>>>" + inner_exception.Message;
                            inner_exception = inner_exception.InnerException;
                        }
                        _dbcontext.SDMTicketExceptions.Add(entity);
                        _dbcontext.SaveChanges();

                    }
                }

                throw e;

            }
            finally
            {
                LogOut();
            }
            return ret;
        }

        public string UploadAttachmentToTicket(SDMUploadAttachmentDTO dto, string userName)
        {
            string ret = null;
            LogIn();
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("sid", _sid + "");
            param.Add("repositoryHandle", "doc_rep:1002");
            param.Add("objectHandle", "cr:" + dto.TicketId);
            param.Add("description", dto.AttachmentName);
            param.Add("fileName", dto.AttachmentName);
            SendSOAPRequest(_sdmClient.InnerChannel.RemoteAddress.ToString(), "createAttachment", param, dto.AttachmentContent);
            string note = string.Format("Utente {0} ha aggiunto il seguente documento: {1}", userName, dto.AttachmentName);
            _sdmClient.createActivityLogAsync(_sid, "", "cr:" + dto.TicketId, note, "LOG", 0, false).Wait();
            LogOut();
            return ret;
        }

        public List<SDMTicketLVDTO> GetTicketsVerificationByUser(HttpContext context, string period)
        {
            List<SDMTicketLVDTO> ret = null;
            LogIn();
            try
            {
                var user = context.User as AuthUser;
                if (user == null)
                {
                    throw new Exception("No user Login to Get Tickets by user");
                }
                var userid = _dataService.GetUserIdByUserName(user.UserName);
                if (userid != null)
                {
                    List<SDMTicketLVDTO> tickets = new List<SDMTicketLVDTO>();
                    userid = userid.Split('\\')[1];

                    var select_a = _sdmClient.doSelectAsync(_sid, "cr", "status='" + _statusMapping.ElementAt(0).code + "' and zz_cned_string1 LIKE '%" + userid + "%' and zz_cned_string4='" + period + "'", 99999, new string[] { "ref_num", "description", "group", "summary", "status", "zz_mgnote", "zz_cned_string1", "zz_cned_string2", "zz_cned_string3", "zz_cned_string4", "zz_string1", "zz_string2", "zz_string3", "last_mod_dt" });
                    select_a.Wait();
                    var select_result = select_a.Result.doSelectReturn;
                    tickets.AddRange(parseTickets(select_result));

                    select_a = _sdmClient.doSelectAsync(_sid, "cr", "status='" + _statusMapping.ElementAt(1).code + "' and zz_cned_string2 LIKE '%" + userid + "%' and zz_cned_string4='" + period + "'", 99999, new string[] { "ref_num", "description", "group", "summary", "status", "zz_mgnote", "zz_cned_string1", "zz_cned_string2", "zz_cned_string3", "zz_cned_string4", "zz_string1", "zz_string2", "zz_string3", "last_mod_dt" });
                    select_a.Wait();
                    select_result = select_a.Result.doSelectReturn;
                    tickets.AddRange(parseTickets(select_result));

                    select_a = _sdmClient.doSelectAsync(_sid, "cr", "status='" + _statusMapping.ElementAt(2).code + "' and zz_cned_string3 LIKE '%" + userid + "%' and zz_cned_string4='" + period + "'", 99999, new string[] { "ref_num", "description", "group", "summary", "status", "zz_mgnote", "zz_cned_string1", "zz_cned_string2", "zz_cned_string3", "zz_cned_string4", "zz_string1", "zz_string2", "zz_string3", "last_mod_dt" });
                    select_a.Wait();
                    select_result = select_a.Result.doSelectReturn;
                    tickets.AddRange(parseTickets(select_result));
                    ret = tickets.ToList();
                    var ids = ret.Select(o => o.kpiIdPK).ToList();
                    var titolos = _dataService.GetKPISDMExtraInformation(ids);
                    return (from tks in ret
                            join tito in titolos on tks.kpiIdPK equals tito.id
                            into gj
                            from subset in gj.DefaultIfEmpty()
                            select new SDMTicketLVDTO()
                            {
                                Id = tks.Id,
                                ref_num = tks.ref_num,
                                Summary = tks.Summary,
                                Description = tks.Description,
                                Status = tks.Status,
                                Group = tks.Group,
                                ID_KPI = tks.ID_KPI,
                                Reference1 = tks.Reference1,
                                Reference2 = tks.Reference2,
                                Reference3 = tks.Reference3,
                                Period = tks.Period,
                                primary_contract_party = tks.primary_contract_party,
                                secondary_contract_party = tks.secondary_contract_party,
                                IsClosed = tks.IsClosed,
                                calcValue = tks.calcValue,
                                KpiIds = tks.KpiIds,
                                LastModifiedDate = tks.LastModifiedDate,
                                Titolo = subset?.titolo ?? string.Empty,
                                reference_input = subset?.referent ?? string.Empty,
                                tipologia = subset?.tipologia ?? string.Empty
                            }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                LogOut();
            }
            return ret;
        }

        public List<SDMTicketLVDTO> GetTicketsAdministratorByPeriod(string period)
        {
            List<SDMTicketLVDTO> ret = null;
            LogIn();
            try
            {
                List<SDMTicketLVDTO> tickets = new List<SDMTicketLVDTO>();

                var select_a = _sdmClient.doSelectAsync(_sid, "cr", "zz_cned_string4='" + period + "'", 99999, new string[] { "ref_num", "description", "group", "summary", "status", "zz_mgnote", "zz_cned_string1", "zz_cned_string2", "zz_cned_string3", "zz_cned_string4", "zz_string1", "zz_string2", "zz_string3", "last_mod_dt" });
                select_a.Wait();
                var select_result = select_a.Result.doSelectReturn;
                tickets.AddRange(parseTickets(select_result));
                ret = tickets.ToList();
                var ids = ret.Select(o => o.kpiIdPK).ToList();
                var titolos = _dataService.GetKPISDMExtraInformation(ids);
                return (from tks in ret
                        join tito in titolos on tks.kpiIdPK equals tito.id
                        into gj
                        from subset in gj.DefaultIfEmpty()
                        select new SDMTicketLVDTO()
                        {
                            Id = tks.Id,
                            ref_num = tks.ref_num,
                            Summary = tks.Summary,
                            Description = tks.Description,
                            Status = tks.Status,
                            Group = tks.Group,
                            ID_KPI = tks.ID_KPI,
                            Reference1 = tks.Reference1,
                            Reference2 = tks.Reference2,
                            Reference3 = tks.Reference3,
                            Period = tks.Period,
                            primary_contract_party = tks.primary_contract_party,
                            secondary_contract_party = tks.secondary_contract_party,
                            IsClosed = tks.IsClosed,
                            calcValue = tks.calcValue,
                            KpiIds = tks.KpiIds,
                            LastModifiedDate = tks.LastModifiedDate,
                            Titolo = subset?.titolo ?? string.Empty,
                            reference_input = subset?.referent ?? string.Empty,
                            tipologia = subset?.tipologia ?? string.Empty
                        }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                LogOut();
            }
        }

        public List<SDMTicketLVDTO> GetTicketsRicercaByUser(HttpContext context, string period)
        {
            List<SDMTicketLVDTO> ret = null;

            try
            {
                var user = context.User as AuthUser;
                if (user == null)
                {
                    throw new Exception("No user Login to Get Tickets by user");
                }
                var userid = _dataService.GetUserIdByUserName(user.UserName);
                if (userid != null)
                {
                    List<SDMTicketLVDTO> tickets = new List<SDMTicketLVDTO>();
                    userid = userid.Split('\\')[1];
                    var kpiDetials = _infomationAPI.GetContractPartyByUser(user.UserId);
                    var kpiIds = kpiDetials.Select(o => o.KPIId).ToList();
                    var contractparties = kpiDetials.Select(o => o.ContractPartyId).Distinct();
                    var globalrules = kpiDetials.Select(o => o.GlobalRuleId).ToList();
                    string filterstring = "";
                    var groups = _dbcontext.SDMTicketGroup.Where(o => contractparties.Contains(o.category_id)).Select(p => p.handle.Substring(4)).ToList();
                    if (!groups.Any())
                    {
                        return tickets;
                    }
                    var filters = groups.Select(o => string.Format(" group.id=U'{0}' ", o));
                    filterstring = string.Join("OR", filters);
                    if (!string.IsNullOrEmpty(period) && period != "all/all")
                    {
                        if (period.IndexOf("all/") != -1)
                        {
                            filterstring = string.Format("({0}) AND zz_cned_string4 LIKE '%/{1}'", filterstring, period.Split('/').LastOrDefault());
                        }
                        else if (period.IndexOf("/all") != -1)
                        {
                            filterstring = string.Format("({0}) AND zz_cned_string4 LIKE '{1}/%'", filterstring, period.Split('/').FirstOrDefault());
                        }
                        else
                        {
                            filterstring = string.Format("({0}) AND zz_cned_string4='{1}'", filterstring, period);
                        }
                    }
                    LogIn();
                    var select_a = _sdmClient.doSelectAsync(_sid, "cr", filterstring, 99999, new string[] { "ref_num", "description", "group", "summary", "status", "zz_mgnote", "zz_cned_string1", "zz_cned_string2", "zz_cned_string3", "zz_cned_string4", "zz_string1", "zz_string2", "zz_string3", "last_mod_dt" });
                    select_a.Wait();
                    var select_result = select_a.Result.doSelectReturn;
                    var tckts = parseTickets(select_result).Where(o => globalrules.Contains(o.global_rule_id)).ToList();
                    var ids = tckts.Select(o => o.kpiIdPK).ToList();
                    var titolos = _dataService.GetKPISDMExtraInformation(ids);
                    return (from tks in tckts
                            join tito in titolos on tks.kpiIdPK equals tito.id
                            into gj
                            from subset in gj.DefaultIfEmpty()
                            select new SDMTicketLVDTO()
                            {
                                Id = tks.Id,
                                ref_num = tks.ref_num,
                                Summary = tks.Summary,
                                Description = tks.Description,
                                Status = tks.Status,
                                Group = tks.Group,
                                ID_KPI = tks.ID_KPI,
                                Reference1 = tks.Reference1,
                                Reference2 = tks.Reference2,
                                Reference3 = tks.Reference3,
                                Period = tks.Period,
                                primary_contract_party = tks.primary_contract_party,
                                secondary_contract_party = tks.secondary_contract_party,
                                IsClosed = tks.IsClosed,
                                calcValue = tks.calcValue,
                                KpiIds = tks.KpiIds,
                                LastModifiedDate = tks.LastModifiedDate,
                                Titolo = subset?.titolo ?? string.Empty,
                                reference_input = subset?.referent ?? string.Empty,
                                tipologia = subset?.tipologia ?? string.Empty
                            }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                LogOut();
            }
            return ret;
        }

        public ChangeStatusDTO TransferTicketByID(int id, string status, string description, HttpContext context)
        {
            var user = context.User as AuthUser;
            if (user == null)
            {
                throw new Exception("No user Login to Get Tickets by user");
            }
            var userid = _dataService.GetUserIdByUserName(user.UserName);
            var dto = new ChangeStatusDTO();
            var ticket = GetTicketByID(id);
            if (ticket.Status != status)
            {
                _dbcontext.LogInformation("Status is equation to previous Status");
                return dto;
            }
            LogIn();
            try
            {
                if (ticket.Status == _statusMapping.OrderBy(o => o.step).First().name || ticket.Status == _statusMapping.OrderBy(o => o.step).Last().name || !_statusMapping.Any(o => o.name == ticket.Status))
                {
                    _dbcontext.LogInformation("Ticket Status not in configuration");
                    return dto;
                }
                int step = _statusMapping.FirstOrDefault(o => o.name == ticket.Status).step;
                step--;
                var newstatus = _statusMapping.FirstOrDefault(o => o.step == step).handle;
                string newgroup = "";
                var newgroupEnt = _groupMapping.FirstOrDefault(o => o.step == step && o.category_id == int.Parse(ticket.primary_contract_party));
                if (newgroupEnt != null)
                {
                    newgroup = newgroupEnt.handle;
                }
                else
                {
                    throw new Exception("No group configuration in settings: Name: " + ticket.Group + " and category id: " + ticket.primary_contract_party);
                }
                string primarycp = string.IsNullOrEmpty(ticket.primary_contract_party) ? "" : _dbcontext.Customers.Single(o => o.customer_id == int.Parse(ticket.primary_contract_party)).customer_name;
                string secondarycp = string.IsNullOrEmpty(ticket.secondary_contract_party) ? "" : _dbcontext.Customers.Single(o => o.customer_id == int.Parse(ticket.secondary_contract_party)).customer_name;
                var bsiticketdto = new BSIKPIUploadDTO()
                {
                    kpi_name = ticket.Summary.Split('|')[1],
                    contract_name = ticket.Summary.Split('|')[2],
                    id_ticket = ticket.ref_num,
                    period = ticket.Period,
                    primary_contract_party = primarycp,
                    secondary_contract_party = secondarycp,
                    ticket_status = _statusMapping.FirstOrDefault(o => o.step == step).name
                };
                string tickethandle = "cr:" + id;
                var esca = _sdmClient.updateObjectAsync(_sid, tickethandle, new string[2] { "group", newgroup }, new string[0]);
                esca.Wait();

                description = string.Format("Rifiutato da {0} Nota inserita dall'utente {1}", userid = userid.Split('\\')[1], description);

                var statusa = _sdmClient.changeStatusAsync(_sid, "", tickethandle, description, newstatus);
                statusa.Wait();
                LogOut();
                dto.IsSDMStatusChanged = true;
                if (CallUploadKPI(bsiticketdto))
                {
                    dto.IsBSIStatusChanged = true;
                }
                if (_dbcontext.SDMTicketFact.Any(o => o.ticket_id == id))
                {
                    var fact = _dbcontext.SDMTicketFact.Single(o => o.ticket_id == id);
                    var log = new SDM_TicketLog()
                    {
                        created_on = DateTime.Now,
                        log_type = "D",
                        note = description,
                        TicketFact = fact,
                        ticket_fact_id = fact.id
                    };
                    _dbcontext.SDMTicketLogs.Add(log);
                    _dbcontext.SaveChanges();
                }
                return dto;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                LogOut();
            }
        }

        public void UpdateTicketValue(HttpContext context, TicketValueDTO dto)
        {
            try
            {
                var user = context.User as AuthUser;
                if (user == null)
                {
                    throw new Exception("No user Login to Get Tickets by user");
                }
                var userid = _dataService.GetUserIdByUserName(user.UserName);
                string desc = GetTicketByID(dto.TicketId).Description;
                int startindex=desc.IndexOf("VALORE:");
                int endIndex=desc.IndexOf("\n",startindex);
                string replaceableString = desc.Substring(startindex, endIndex- startindex);
                var newViloreString = $"{dto.Value} {dto.Sign} {(dto.Type == 1 ? "[Compliant]":"[Non Compliant]")}";
                var newstring = string.Format("VALORE: {0} {1} {2}", dto.Value, dto.Sign, dto.Type == 1 ? "[Compliant]" : "[Non Compliant]");
                var newdesc = desc.Replace(replaceableString, newstring).ToString();

                var tickethandle = "cr:" + dto.TicketId;
                LogIn();
                var changeojb = _sdmClient.updateObjectAsync(_sid, tickethandle, new string[4] { "description", newdesc, "zz_string2", newViloreString }, new string[0]);
                changeojb.Wait();
                var note = string.Format("Aggiornato il valore del ticket da parte dell’utente: {0}  Questa è la nota inserita: {1}", userid.Split('\\')[1], dto.Note);
                _sdmClient.createActivityLogAsync(_sid, "", "cr:" + dto.TicketId, note, "LOG", 0, false).Wait();
                var sdmFact = _dbcontext.SDMTicketFact.FirstOrDefault(o => o.ticket_id == dto.TicketId);
                if (sdmFact != null)
                {
                    sdmFact.complaint = (newdesc.IndexOf("[Compliant]") != -1);
                    sdmFact.notcalculated = (newdesc.IndexOf("[NE]") != -1);
                    sdmFact.notcomplaint = (newdesc.IndexOf("[Non Compliant]") != -1);
                    sdmFact.result_value = newViloreString;
                    _dbcontext.SaveChanges();
                }
                
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                LogOut();
            }
        }

        public ChangeStatusDTO EscalateTicketbyID(int id, string status, string description, HttpContext context)
        {
            var user = context.User as AuthUser;
            if (user == null)
            {
                throw new Exception("No user Login to Get Tickets by user");
            }
            var userid = _dataService.GetUserIdByUserName(user.UserName);

            var dto = new ChangeStatusDTO();
            var ticket = GetTicketByID(id);
            if (ticket.Status != status)
            {
                _dbcontext.LogInformation("Status is equation to previous Status");
                return dto;
            }
            LogIn();
            try
            {
                if (ticket.Status == _statusMapping.OrderBy(o => o.step).Last().name || !_statusMapping.Any(o => o.name == ticket.Status))
                {
                    _dbcontext.LogInformation("Ticket Status not in configuration");
                    return dto;
                }
                int step = _statusMapping.FirstOrDefault(o => o.name == ticket.Status).step;
                step++;
                var newstatus = _statusMapping.FirstOrDefault(o => o.step == step).handle;
                string newgroup = "";
                var newgroupEnt = _groupMapping.FirstOrDefault(o => o.step == step && o.category_id == int.Parse(ticket.primary_contract_party));
                if (newgroupEnt != null)
                {
                    newgroup = newgroupEnt.handle;
                }
                else
                {
                    throw new Exception("No group configuration in settings: Name: " + ticket.Group + " and category id: " + ticket.primary_contract_party);
                }
                string primarycp = string.IsNullOrEmpty(ticket.primary_contract_party) ? "" : _dbcontext.Customers.Single(o => o.customer_id == int.Parse(ticket.primary_contract_party)).customer_name;
                string secondarycp = string.IsNullOrEmpty(ticket.secondary_contract_party) ? "" : _dbcontext.Customers.Single(o => o.customer_id == int.Parse(ticket.secondary_contract_party)).customer_name;
                var bsiticketdto = new BSIKPIUploadDTO()
                {
                    kpi_name = ticket.Summary.Split('|')[1],
                    contract_name = ticket.Summary.Split('|')[2],
                    id_ticket = ticket.ref_num,
                    period = ticket.Period,
                    primary_contract_party = primarycp,
                    secondary_contract_party = secondarycp,
                    ticket_status = _statusMapping.FirstOrDefault(o => o.step == step).name
                };

                string tickethandle = "cr:" + id;
                var esca = _sdmClient.updateObjectAsync(_sid, tickethandle, new string[2] { "group", newgroup }, new string[0]);
                esca.Wait();

                description = string.Format("Approvato da {0} Nota inserita dall'utente: {1}", userid = userid.Split('\\')[1], description);

                var statusa = _sdmClient.changeStatusAsync(_sid, "", tickethandle, description, newstatus);
                statusa.Wait();
                LogOut();
                dto.IsSDMStatusChanged = true;
                if (CallUploadKPI(bsiticketdto))
                {
                    dto.IsBSIStatusChanged = true;
                }
                if (step == _statusMapping.Max(o => o.step))
                {
                    dto.ShowArchivedMsg = true;
                    try
                    {
                        if (ticket.Summary.Split('|').Length <= 4)
                        {
                            var kpiid = int.Parse(ticket.KpiIds.Split('|').Last());
                            var kpi = _dbcontext.CatalogKpi.FirstOrDefault(o => o.global_rule_id_bsi == kpiid);
                            var contract_name_test = kpi.contract;
                            var    customer_name_test = _dbcontext.Customers.Single(o => o.customer_id == kpi.primary_contract_party).customer_name;
                            var global_rule_id_test = kpi.global_rule_id_bsi;
                            var id_kpi_test = kpi.id_kpi;
                            var    interval_kpi_test = new DateTime(2000 + int.Parse(ticket.Period.Split('/').Last()), int.Parse(ticket.Period.Split('/').First()), 1);
                            var tracking_period_test = kpi.tracking_period;
                            var    name_kpi_test = kpi.short_name;
                            var kpi_name_bsi_test = kpi.kpi_name_bsi;
                            var    kpi_description_bsi_test = _dbcontext.Rules.Where(o => o.global_rule_id == kpi.global_rule_id_bsi && o.is_effective == "Y").OrderByDescending(o => o.sla_version_id).FirstOrDefault().rule_description;
                            var rule_id_bsi_test = kpi.global_rule_id_bsi;
                            var    close_timestamp_ticket_test = DateTime.Now;
                            var ticket_id_test = int.Parse(ticket.ref_num);
                            var    value_kpi_test = (ticket.calcValue == "[NE]") ? "[NE]" : ticket.calcValue.Split(' ').FirstOrDefault().Trim();
                            var symbol_test = (ticket.calcValue == "[NE]") ? "N/A" : ticket.calcValue.Split(' ').ElementAt(1).Trim();
                            ARulesDTO ardto = new ARulesDTO()
                            {
                                contract_name = kpi.contract,
                                customer_name = _dbcontext.Customers.Single(o => o.customer_id == kpi.primary_contract_party).customer_name,
                                global_rule_id = kpi.global_rule_id_bsi,
                                archived = true,
                                id_kpi = kpi.id_kpi,
                                interval_kpi = new DateTime(2000 + int.Parse(ticket.Period.Split('/').Last()), int.Parse(ticket.Period.Split('/').First()), 1),
                                tracking_period = kpi.tracking_period,
                                name_kpi = kpi.short_name,
                                kpi_name_bsi = kpi.kpi_name_bsi,
                                kpi_description_bsi = kpi_description_bsi_test == null ? "" : kpi_description_bsi_test,
                                rule_id_bsi = kpi.global_rule_id_bsi,
                                close_timestamp_ticket = DateTime.Now,
                                ticket_id = int.Parse(ticket.ref_num),
                                value_kpi = (ticket.calcValue == "[NE]") ? "[NE]" : ticket.calcValue.Split(' ').FirstOrDefault().Trim(),
                                symbol = (ticket.calcValue == "[NE]") ? "N/A" : ticket.calcValue.Split(' ').ElementAt(1).Trim(),
                            };
                            _dataService.AddArchiveKPI(ardto);
                            _dataService.AddArchiveRawData(kpi.global_rule_id_bsi, ticket.Period, kpi.tracking_period);

                            dto.IsArchived = true;
                        }
                    }
                    catch (Exception e)
                    {
                        _dbcontext.LogInformation("Error: " + e.Message);
                    }
                }
                if (_dbcontext.SDMTicketFact.Any(o => o.ticket_id == id))
                {
                    var fact = _dbcontext.SDMTicketFact.Single(o => o.ticket_id == id);
                    var log = new SDM_TicketLog()
                    {
                        created_on = DateTime.Now,
                        log_type = "A",
                        note = description,
                        TicketFact = fact,
                        ticket_fact_id = fact.id
                    };
                    _dbcontext.SDMTicketLogs.Add(log);
                    _dbcontext.SaveChanges();
                }
                return dto;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                LogOut();
            }
        }

        public List<SDMTicketLogDTO> GetTicketHistory(int ticketId)
        {
            List<SDMTicketLogDTO> ret = null;
            LogIn();
            try
            {
                var selecta = _sdmClient.doSelectAsync(_sid, "alg", "call_req_id='cr:" + ticketId + "'", 99999, new string[0]);
                selecta.Wait();
                var sel = selecta.Result.doSelectReturn;
                ret = parseLogs(sel).Where(o => o.Type != "LOG").OrderByDescending(o => int.Parse(o.TimeStamp)).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                LogOut();
            }
            return ret;
        }

        private bool CallUploadKPI(BSIKPIUploadDTO dto)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    UploadKPIDTO data = new UploadKPIDTO() { arguments = new List<string>() { dto.primary_contract_party + "", dto.secondary_contract_party + "", dto.contract_name, dto.kpi_name, dto.id_ticket, dto.period, dto.ticket_status } };
                    var output = QuantisUtilities.FixHttpURLForCall(_dataService.GetBSIServerURL(), "/api/UploadKPI/UploadKPI");
                    client.BaseAddress = new Uri(output.Item1);
                    var dataAsString = JsonConvert.SerializeObject(data);
                    _dbcontext.LogInformation("Parameters for Upload KPI: " + dataAsString);
                    var content = new StringContent(dataAsString);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = client.PostAsync(output.Item2, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        if (res == "\"TRUE\"")
                        {
                            return true;
                        }
                        else
                        {
                            _dbcontext.LogInformation("Message from Upload KPI: " + res);
                            return false;
                        }
                    }
                    else
                    {
                        _dbcontext.LogInformation(string.Format("Call to Upload KPI has failed. BaseURL: {0} APIPath: {1} Data:{2}", output.Item1, output.Item2, dataAsString));
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private SDMTicketLVDTO parseNewTicket(string ticket)
        {
            var dtos = new List<SDMTicketLVDTO>();
            XDocument xdoc = XDocument.Parse(ticket);
            var attributes = xdoc.Element("UDSObject").Element("Attributes").Elements("Attribute");
            SDMTicketLVDTO dto = new SDMTicketLVDTO();
            dto.Id = xdoc.Element("UDSObject").Element("Handle").Value.Substring(3);
            dto.ref_num = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "ref_num").Element("AttrValue").Value;
            dto.Description = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "description").Element("AttrValue").Value;
            dto.Group = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "group").Element("AttrValue").Value;
            dto.Summary = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "summary").Element("AttrValue").Value;
            dto.Status = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "status").Element("AttrValue").Value;
            dto.ID_KPI = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_mgnote").Element("AttrValue").Value;
            dto.Reference1 = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_cned_string1").Element("AttrValue").Value;
            dto.Reference2 = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_cned_string2").Element("AttrValue").Value;
            dto.Reference3 = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_cned_string3").Element("AttrValue").Value;
            dto.Period = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_cned_string4").Element("AttrValue").Value;
            //dto.primary_contract_party = (attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_primary_contract_party")==null)?"":attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_primary_contract_party").Element("AttrValue").Value;
            //dto.secondary_contract_party = (attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_secondary_contract_party")==null)?"":attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_secondary_contract_party").Element("AttrValue").Value;

            if (_groupMapping.Any(o => o.handle.Substring(4) == dto.Group))
            {
                dto.Group = _groupMapping.FirstOrDefault(o => o.handle.Substring(4) == dto.Group).name;
            }
            return dto;
        }

        private List<SDMTicketLVDTO> parseTickets(string tickets)
        {
            var dtos = new List<SDMTicketLVDTO>();
            XDocument xdoc = XDocument.Parse(tickets);
            var lists = from uoslist in xdoc.Element("UDSObjectList").Elements("UDSObject") select uoslist;
            foreach (var l in lists)
            {
                var attributes = l.Element("Attributes").Elements("Attribute");
                SDMTicketLVDTO dto = new SDMTicketLVDTO();
                dto.Id = l.Element("Handle").Value.Substring(3);
                dto.ref_num = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "ref_num").Element("AttrValue").Value;
                dto.Description = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "description").Element("AttrValue").Value;
                dto.Group = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "group").Element("AttrValue").Value;
                dto.Summary = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "summary").Element("AttrValue").Value;
                dto.Status = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "status").Element("AttrValue").Value;
                dto.ID_KPI = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_mgnote").Element("AttrValue").Value;
                dto.Reference1 = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_cned_string1").Element("AttrValue").Value;
                dto.Reference2 = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_cned_string2").Element("AttrValue").Value;
                dto.Reference3 = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_cned_string3").Element("AttrValue").Value;
                dto.Period = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_cned_string4").Element("AttrValue").Value;
                dto.LastModifiedDate = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "last_mod_dt").Element("AttrValue").Value;
                var zz1 = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_string1");
                var zz2 = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_string2");
                var zz3 = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "zz_string3");
                bool isIncluded = true;
                if (zz1 == null)
                {
                    dto.primary_contract_party = "";
                    dto.secondary_contract_party = "";
                    isIncluded = false;
                }
                else
                {
                    var val = zz1.Element("AttrValue").Value.Split("|");
                    dto.primary_contract_party = val[0];
                    if (val.Count() == 2)
                    {
                        dto.secondary_contract_party = val[1];
                    }
                    else
                    {
                        dto.secondary_contract_party = "";
                    }
                }
                if (zz2 != null)
                {
                    dto.calcValue = zz2.Element("AttrValue").Value;
                }
                else
                {
                    isIncluded = false;
                }
                if (zz3 != null)
                {
                    dto.KpiIds = zz3.Element("AttrValue").Value;
                    int kpiid = 0;
                    int globalruleid = 0;
                    int.TryParse(dto.KpiIds.Split('|').FirstOrDefault(), out kpiid);
                    if (dto.KpiIds.Split('|').Count() == 2)
                    {
                        int.TryParse(dto.KpiIds.Split('|').ElementAt(1), out globalruleid);
                    }

                    dto.kpiIdPK = kpiid;
                    dto.global_rule_id = globalruleid;
                }
                else
                {
                    isIncluded = false;
                }
                if (_groupMapping.Any(o => o.handle.Substring(4) == dto.Group) && !string.IsNullOrEmpty(dto.primary_contract_party))
                {
                    var groupscene = _groupMapping.FirstOrDefault(o => o.handle.Substring(4) == dto.Group && o.category_id == int.Parse(dto.primary_contract_party));
                    if (groupscene != null)
                    {
                        dto.Group = groupscene.name;
                    }
                }
                else
                {
                    isIncluded = false;
                }
                if (_statusMapping.Any(o => o.code == dto.Status))
                {
                    var st = dto.Status;
                    dto.Status = _statusMapping.FirstOrDefault(o => o.code == dto.Status).name;
                    if (_statusMapping.First(o => o.code == st).step == _statusMapping.Max(p => p.step))
                    {
                        dto.IsClosed = true;
                    }
                    else
                    {
                        dto.IsClosed = false;
                    }
                }
                else
                {
                    isIncluded = false;
                }
                if (isIncluded)
                {
                    dtos.Add(dto);
                }
            }
            return dtos;
        }

        private List<SDMTicketLogDTO> parseLogs(string logs)
        {
            var dtos = new List<SDMTicketLogDTO>();
            XDocument xdoc = XDocument.Parse(logs);
            var lists = from uoslist in xdoc.Element("UDSObjectList").Elements("UDSObject") select uoslist;
            foreach (var l in lists)
            {
                var attributes = l.Element("Attributes").Elements("Attribute");
                SDMTicketLogDTO dto = new SDMTicketLogDTO();
                dto.LogId = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "id").Element("AttrValue").Value;
                dto.MsgBody = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "msg_body").Element("AttrValue").Value;
                dto.TicketHandler = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "call_req_id").Element("AttrValue").Value;
                dto.TicketStatus = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "cr_status").Element("AttrValue").Value;
                dto.TimeStamp = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "time_stamp").Element("AttrValue").Value;
                dto.ActionDescription = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "action_desc").Element("AttrValue").Value;
                dto.Description = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "description").Element("AttrValue").Value;
                dto.Type = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "type").Element("AttrValue").Value;
                dtos.Add(dto);
            }
            return dtos;
        }

        private List<SDMAttachmentDTO> parseAttachments(string logs)
        {
            var dtos = new List<SDMAttachmentDTO>();
            XDocument xdoc = XDocument.Parse(logs);
            var lists = from uoslist in xdoc.Element("UDSObjectList").Elements("UDSObject") select uoslist;
            foreach (var l in lists)
            {
                var attributes = l.Element("Attributes").Elements("Attribute");
                SDMAttachmentDTO dto = new SDMAttachmentDTO();
                dto.AttachmentHandle = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "attmnt").Element("AttrValue").Value;
                dto.AttachmentName = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "attmnt.attmnt_name").Element("AttrValue").Value;
                dto.LastModifiedDate = attributes.FirstOrDefault(o => o.Element("AttrName").Value == "last_mod_dt").Element("AttrValue").Value;
                dtos.Add(dto);
            }
            return dtos;
        }

        private string SendSOAPRequest(string url, string action, Dictionary<string, string> parameters, byte[] fileData)
        {
            lock (_object)
            {
                var dto = new UploadTicketDTO();
                dto.url = url;
                dto.action = action;
                dto.sid = parameters["sid"];
                dto.repositoryHandle = parameters["repositoryHandle"];
                dto.objectHandle = parameters["objectHandle"];
                dto.description = parameters["description"];
                dto.fileName = parameters["fileName"].Replace(":", "");
                dto.fileData = fileData;
                using (var client = new HttpClient())
                {
                    var bsiconf = _infomationAPI.GetConfiguration("be_bsi", "bsi_api_url");
                    var output = QuantisUtilities.FixHttpURLForCall(bsiconf.Value, "/home/SendSOAPRequest");
                    client.BaseAddress = new Uri(output.Item1);
                    var dataAsString = JsonConvert.SerializeObject(dto);
                    var content = new StringContent(dataAsString);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = client.PostAsync(output.Item2, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string xml = response.Content.ReadAsStringAsync().Result;
                        if (xml == "TRUE")
                        {
                            return "TRUE";
                        }
                        else
                        {
                            _dbcontext.LogInformation(xml);
                            throw new Exception(xml);
                        }
                    }
                    else
                    {
                        _dbcontext.LogInformation("Call to BSI not sucessfull");
                        throw new Exception("Call to BSI not sucessfull");
                    }
                }
            }
        }

        ~ServiceDeskManagerService()
        {
            LogOut();
        }
    }
}