using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models;
using Quantis.WorkFlow.Services;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.API;
using Quantis.WorkFlow.Services.DTOs.BusinessLogic;
using Quantis.WorkFlow.Services.DTOs.Information;
using Quantis.WorkFlow.Services.DTOs.Widgets;
using Quantis.WorkFlow.Services.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Quantis.WorkFlow.APIBase.API
{
    public class DataService : IDataService
    {
        private readonly IMappingService<GroupDTO, T_Group> _groupMapper;
        private readonly IMappingService<PageDTO, T_Page> _pageMapper;
        private readonly IMappingService<WidgetDTO, T_Widget> _widgetMapper;
        private readonly IMappingService<UserDTO, T_CatalogUser> _userMapper;
        private readonly IMappingService<FormRuleDTO, T_FormRule> _formRuleMapper;
        private readonly IMappingService<FormUsersDTO, T_FormUsers> _formUsersMapper;
        private readonly IMappingService<CatalogKpiDTO, T_CatalogKPI> _catalogKpiMapper;
        private readonly IMappingService<ApiDetailsDTO, T_APIDetail> _apiMapper;
        private readonly IMappingService<FormAttachmentDTO, T_FormAttachment> _fromAttachmentMapper;
        private readonly IMappingService<PersonalReportDTO, T_PersonalReport> _personalReportMapper;
        private readonly IOracleDataService _oracleAPI;
        private readonly IConfiguration _configuration;
        private readonly ISMTPService _smtpService;
        private readonly IInformationService _infomationAPI;
        private readonly WorkFlowPostgreSqlContext _dbcontext;
        private static string _connectionstring = null;
        private IMemoryCache _cache;

        public DataService(WorkFlowPostgreSqlContext context,
            IMappingService<GroupDTO, T_Group> groupMapper,
            IMappingService<PageDTO, T_Page> pageMapper,
            IMappingService<FormUsersDTO, T_FormUsers> formUsersMapper,
            IMappingService<WidgetDTO, T_Widget> widgetMapper,
            IMappingService<UserDTO, T_CatalogUser> userMapper,
            IMappingService<FormRuleDTO, T_FormRule> formRuleMapper,
            IMappingService<CatalogKpiDTO, T_CatalogKPI> catalogKpiMapper,
            IMappingService<ApiDetailsDTO, T_APIDetail> apiMapper,
            IMappingService<FormAttachmentDTO, T_FormAttachment> fromAttachmentMapper,
            IMappingService<PersonalReportDTO, T_PersonalReport> personalReportMapper,
            IConfiguration configuration,
            ISMTPService smtpService,
            IOracleDataService oracleAPI,
            IInformationService infomationAPI,
            IMemoryCache memoryCache)
        {
            _groupMapper = groupMapper;
            _pageMapper = pageMapper;
            _widgetMapper = widgetMapper;
            _userMapper = userMapper;
            _formRuleMapper = formRuleMapper;
            _formUsersMapper = formUsersMapper;
            _catalogKpiMapper = catalogKpiMapper;
            _apiMapper = apiMapper;
            _oracleAPI = oracleAPI;
            _fromAttachmentMapper = fromAttachmentMapper;
            _personalReportMapper = personalReportMapper;
            _configuration = configuration;
            _smtpService = smtpService;
            _dbcontext = context;
            _infomationAPI = infomationAPI;
            _cache = memoryCache;
            if (_connectionstring == null)
            {
                _connectionstring = QuantisUtilities.GetOracleConnectionString(_dbcontext);
            }
        }

        public bool CronJobsScheduler()
        {
            return true;
        }

        public List<KeyValuePair<int, string>> GetAllCustomersKP()
        {
            try
            {
                return _dbcontext.Customers.Where(o => o.customer_id >= 1000).OrderBy(p => p.customer_name).Select(o => new KeyValuePair<int, string>(o.customer_id, o.customer_name)).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool AddUpdateFormRule(FormRuleDTO dto)
        {
            try
            {
                var entity = _dbcontext.FormRules.FirstOrDefault(o => o.form_id == dto.form_id);
                if (entity == null)
                {
                    entity = new T_FormRule();
                    entity = _formRuleMapper.GetEntity(dto, entity);
                    _dbcontext.FormRules.Add(entity);
                }
                else
                {
                    _formRuleMapper.GetEntity(dto, entity);
                }
                _dbcontext.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<FormDetialsDTO> GetFormDetials(List<int> formids)
        {
            try
            {
                return _dbcontext.Forms.Include(p => p.Attachments).Include(q => q.FormLogs).Where(o => formids.Contains(o.form_id)).Select(o => new FormDetialsDTO() { form_id = o.form_id, attachment_count = o.Attachments.Count, latest_modified_date = o.FormLogs.Any() ? o.FormLogs.Max(r => r.time_stamp) : new DateTime(0) }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*       public List<int> GetRawIdsFromRulePeriod(int ruleId,string period)
               {
                   try
                   {
                       var config = new List<KPIRegistrationDTO>();
                       using (var client = new HttpClient())
                       {
                           var con = GetBSIServerURL();
                           var apiPath = "api/KPIRegistration/GetKPIRegistrations?ruleId="+ ruleId;
                           var output = QuantisUtilities.FixHttpURLForCall(con, apiPath);
                           client.BaseAddress = new Uri(output.Item1);
                           var response = client.GetAsync(output.Item2).Result;
                           if (response.IsSuccessStatusCode)
                           {
                               config = JsonConvert.DeserializeObject<List<KPIRegistrationDTO>>(response.Content.ReadAsStringAsync().Result);
                               var eventResource = config.Select(o => new EventResourceDTO()
                               {
                                   EventId = o.EventTypeId,
                                   ResourceId = o.ResourceId
                               }).ToList();

                               var rawIds = GetRawIdsFromResource(eventResource, period);
                               return rawIds;
                           }
                           else
                           {
                               var e = new Exception(string.Format("KPI registration API not working: basePath: {0} apipath: {1}", client.BaseAddress, apiPath));
                               throw e;
                           }
                       }
                   }
                   catch(Exception e)
                   {
                       throw e;
                   }
               }*/

        public List<EventResourceDTO> GetEventResourceFromRule(int ruleId)
        {
            try
            {
                var config = new List<KPIRegistrationDTO>();
                using (var client = new HttpClient())
                {
                    var con = GetBSIServerURL();
                    var apiPath = "/api/KPIRegistration/GetKPIRegistrations?ruleId=" + ruleId;
                    var output = QuantisUtilities.FixHttpURLForCall(con, apiPath);
                    client.BaseAddress = new Uri(output.Item1);
                    var response = client.GetAsync(output.Item2).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        config = JsonConvert.DeserializeObject<List<KPIRegistrationDTO>>(response.Content.ReadAsStringAsync().Result);
                        var eventResource = config.Select(o => new EventResourceDTO()
                        {
                            EventId = o.EventTypeId,
                            ResourceId = o.ResourceId
                        }).ToList();

                        //var rawIds = GetRawIdsFromResource(eventResource, period);
                        return eventResource;
                        //DO THE QUERY TO ARCHIVE
                    }
                    else
                    {
                        var e = new Exception(string.Format("KPI registration API not working: basePath: {0} apipath: {1}", client.BaseAddress, apiPath));
                        throw e;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<NotifierLogDTO> GetEmailHistory()
        {
            try
            {
                var entity = _dbcontext.NotifierLogs.ToList();
                return entity.Select(o => new NotifierLogDTO()
                {
                    email_body = o.email_body,
                    id_form = o.id_form,
                    is_ack = o.is_ack,
                    notify_timestamp = o.notify_timestamp,
                    period = o.period,
                    remind_timestamp = o.remind_timestamp,
                    year = o.year
                }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public FormRuleDTO GetFormRuleByKPIID(string kpiId)
        {
            try
            {
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool AddUpdateGroup(GroupDTO dto)
        {
            try
            {
                var entity = new T_Group();
                if (dto.group_id > 0)
                {
                    entity = _dbcontext.Groups.FirstOrDefault(o => o.group_id == dto.group_id);
                }
                entity = _groupMapper.GetEntity(dto, entity);
                if (dto.group_id == 0)
                {
                    _dbcontext.Groups.Add(entity);
                }

                _dbcontext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool AddUpdatePage(PageDTO dto)
        {
            try
            {
                var entity = new T_Page();
                if (dto.page_id > 0)
                {
                    entity = _dbcontext.Pages.FirstOrDefault(o => o.page_id == dto.page_id);
                }
                entity = _pageMapper.GetEntity(dto, entity);
                if (dto.page_id == 0)
                {
                    _dbcontext.Pages.Add(entity);
                }

                _dbcontext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetUserIdByUserName(string name)
        {
            try
            {
                var usr = _dbcontext.CatalogUsers.FirstOrDefault(o => o.ca_bsi_account == name);
                if (usr != null)
                {
                    return usr.userid;
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool AddUpdateUser(UserDTO dto)
        {
            using (var dbContextTransaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var entity = new T_CatalogUser();
                    if (dto.id > 0)
                    {
                        entity = _dbcontext.CatalogUsers.FirstOrDefault(o => o.id == dto.id);
                    }
                    entity = _userMapper.GetEntity(dto, entity);

                    if (dto.id == 0)
                    {
                        var usr = _dbcontext.TUsers.FirstOrDefault(o => dto.ca_bsi_user_id == o.user_id);
                        if (usr != null)
                        {
                            usr.in_catalog = true;
                            _dbcontext.SaveChanges(false);
                            _dbcontext.CatalogUsers.Add(entity);
                        }
                    }

                    _dbcontext.SaveChanges(false);
                    dbContextTransaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    throw e;
                }
            }
        }

        public bool AddUpdateWidget(WidgetDTO dto)
        {
            try
            {
                var entity = new T_Widget();
                if (dto.widget_id > 0)
                {
                    entity = _dbcontext.Widgets.FirstOrDefault(o => o.widget_id == dto.widget_id);
                }
                entity = _widgetMapper.GetEntity(dto, entity);
                if (dto.widget_id == 0)
                {
                    _dbcontext.Widgets.Add(entity);
                }
                _dbcontext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool AddUpdateKpi(CatalogKpiDTO dto)
        {
            using (var dbContextTransaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var entity = new T_CatalogKPI();
                    var get_day_workflow = GetWorkflowDay(dto.organization_unit, dto.sla_id_bsi);
                    dto.day_workflow = get_day_workflow;

                    if (dto.id > 0)
                    {
                        entity = _dbcontext.CatalogKpi.FirstOrDefault(o => o.id == dto.id);
                    }
                    entity = _catalogKpiMapper.GetEntity(dto, entity);

                    if (dto.id == 0)
                    {
                        /*var get_day_workflow = _dbcontext.CatalogKpi.FirstOrDefault(o => dto.sla_id_bsi == o.sla_id_bsi);
                        if (get_day_workflow != null)
                        { 
                            dto.day_workflow = get_day_workflow.day_workflow;
                            entity = _catalogKpiMapper.GetEntity(dto, entity);
                        }*/
                        var kpi = _dbcontext.TGlobalRules.FirstOrDefault(o => dto.global_rule_id_bsi == o.global_rule_id);
                        if (kpi != null)
                        {
                            kpi.in_catalog = true;
                            _dbcontext.SaveChanges(false);
                            _dbcontext.CatalogKpi.Add(entity);
                        }
                    }

                    _dbcontext.SaveChanges(false);
                    dbContextTransaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    throw e;
                }
            }
        }

        public List<GroupDTO> GetAllGroups()
        {
            try
            {
                var groups = _dbcontext.Groups.Where(o => o.delete_date != null);
                return _groupMapper.GetDTOs(groups.ToList());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<ApiDetailsDTO> GetAllAPIs()
        {
            try
            {
                var apis = _dbcontext.ApiDetails.ToList();
                return _apiMapper.GetDTOs(apis.ToList());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<CatalogKpiDTO> GetAllKpis()
        {
            try
            {
                var kpis = _dbcontext.CatalogKpi.Include(o => o.PrimaryCustomer).Include(o => o.SecondaryCustomer).Include(o => o.GlobalRule).Include(o => o.Sla).ToList();
                return _catalogKpiMapper.GetDTOs(kpis.ToList());
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public CatalogKpiDTO GetKpiByGlobalRuleId(int global_rule_id)
        {
            try
            {
                var kpi = _dbcontext.CatalogKpi.Include(o => o.GlobalRule).Include(o => o.PrimaryCustomer).Include(o => o.SecondaryCustomer).Include(o => o.Sla).FirstOrDefault(o => o.global_rule_id_bsi == global_rule_id);
                return _catalogKpiMapper.GetDTO(kpi);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<CatalogKpiDTO> GetAllKpisByUserId(List<int> globalruleIds)
        {
            try
            {
                if (!globalruleIds.Any() || globalruleIds == null)
                {
                    return new List<CatalogKpiDTO>();
                }
                var kpis = _dbcontext.CatalogKpi.Include(o => o.PrimaryCustomer).Include(o => o.SecondaryCustomer).Include(o => o.GlobalRule).Include(o => o.Form).Include(o => o.Sla).Where(o => globalruleIds.Contains(o.global_rule_id_bsi)).ToList();
                return _catalogKpiMapper.GetDTOs(kpis.ToList());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<PageDTO> GetAllPages()
        {
            try
            {
                var pages = _dbcontext.Pages.ToList();
                return _pageMapper.GetDTOs(pages.ToList());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public PagedList<UserDTO> GetAllPagedUsers(UserFilterDTO filter)
        {
            try
            {
                var query = CreateGetUserQuery(filter);
                filter.OrderBy = _userMapper.SortMap(filter.OrderBy);
                var users = query.GetPaged(filter);
                return _userMapper.GetPagedDTOs(users);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<UserDTO> GetAllUsers()
        {
            try
            {
                var users = _dbcontext.CatalogUsers.Where(o => o.ca_bsi_user_id != null).ToList();
                return _userMapper.GetDTOs(users.ToList());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<UserLandingPageLVDTO> GetAllUsersLandingPage()
        {
            try
            {
                var users = _dbcontext.CatalogUsers.Where(o => o.ca_bsi_user_id != null).ToList();
                var userDtos = _userMapper.GetDTOs(users.ToList());
                var landingpages = _dbcontext.UserLandingPages.ToList();
                return (from usrs in userDtos
                        join landpages in landingpages on usrs.ca_bsi_user_id equals landpages.user_id
                        into gj
                        from subset in gj.DefaultIfEmpty()
                        select new UserLandingPageLVDTO()
                        {
                            ca_bsi_account = usrs.ca_bsi_account,
                            ca_bsi_user_id = usrs.ca_bsi_user_id,
                            id = usrs.id,
                            mail = usrs.mail,
                            manager = usrs.manager,
                            name = usrs.name,
                            organization = usrs.organization,
                            surname = usrs.surname,
                            userid = usrs.userid,
                            show_landingpage = subset == null ? false : subset.show_landingpage
                        }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SetLandingPageByUser(int userId, bool set)
        {
            try
            {
                var lp = _dbcontext.UserLandingPages.FirstOrDefault(o => o.user_id == userId);
                if (lp == null)
                {
                    var newlp = new T_UserLandingPage()
                    {
                        user_id = userId,
                        show_landingpage = set,
                        selected_landingpage = false
                    };
                    _dbcontext.UserLandingPages.Add(newlp);
                    _dbcontext.SaveChanges();
                }
                else
                {
                    lp.show_landingpage = set;
                    if (set == false)
                    {
                        lp.selected_landingpage = false;
                    }
                    _dbcontext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public UserLandingPageDTO GetLandingPageInformation(int userId)
        {
            try
            {
                var lp = _dbcontext.UserLandingPages.FirstOrDefault(o => o.user_id == userId);
                if (lp == null)
                {
                    return new UserLandingPageDTO()
                    {
                        UserId = userId,
                        SelectedLandingPage = false,
                        ShowLandingPage = false
                    };
                }
                else
                {
                    return new UserLandingPageDTO()
                    {
                        UserId = userId,
                        ShowLandingPage = lp.show_landingpage,
                        SelectedLandingPage = lp.selected_landingpage
                    };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SelectLandingPage(int userId)
        {
            try
            {
                var lp = _dbcontext.UserLandingPages.FirstOrDefault(o => o.user_id == userId);
                lp.selected_landingpage = true;
                _dbcontext.SaveChanges();
                var dashboards = _dbcontext.DB_Dashboards.Where(o => o.UserId == userId);
                foreach (var db in dashboards)
                {
                    db.IsDefault = false;
                }
                _dbcontext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<UserDTO> GetUsersByRoleId(int roleId)
        {
            try
            {
                var usersids = _dbcontext.UserRoles.Where(o => o.role_id == roleId).Select(p => p.user_id).ToList();
                var users = _dbcontext.CatalogUsers.Where(o => usersids.Contains(o.ca_bsi_user_id ?? 0));
                return _userMapper.GetDTOs(users.ToList());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool SecurityMembers(int UserId)
        {
            bool isSecurityMember = _dbcontext.SecurityMembers.Any(o => o.user_group_id == UserId);
            return isSecurityMember;
        }

        public KpisAssociated GetKpiAssociatedByFormId(int Id)
        {
            try
            {
                var kpi = _dbcontext.CatalogKpi.Where(o => o.id_form == Id);
                if (!kpi.Any())
                {
                    return null;
                }

                var result = new List<KPIContractDTO>();
                result = kpi.Select(o => new KPIContractDTO()
                {
                    contract = o.contract,
                    id_kpi = o.id_kpi,
                    global_rule_id = o.global_rule_id_bsi,
                    kpi_name_bsi = o.kpi_name_bsi,
                    target = o.target
                }).ToList();
                return new KpisAssociated()
                {
                    Kpis_Associated = result
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<FormUsersDTO> GetAllFormUsers(int formId, int UserId)
        {
            try
            {
                var day_cutoffValue = _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_restserver" && o.key == "day_cutoff");
                int todayDay = Int32.Parse(DateTime.Now.ToString("dd"));
                int day_cutoff = Int32.Parse(day_cutoffValue.value);
                bool cutoff_result;
                if (day_cutoff == 0) { cutoff_result = false; } else { if (todayDay < day_cutoff) { cutoff_result = false; } else { cutoff_result = true; } }

                var formUsers = new List<T_FormUsers>();
                var formUser = new T_FormUsers();
                var resultList = new List<FormUsersDTO>();
                if (UserId > 0)
                {
                    bool isSecurityMember = _dbcontext.SecurityMembers.Any(o => o.user_group_id == UserId);
                    if (isSecurityMember)
                    {
                        formUsers = _dbcontext.FormUsers.Select(m => new T_FormUsers
                        {
                            form_id = m.form_id,
                            form_name = m.form_name,
                            form_description = m.form_description,
                            reader_configuration = m.reader_configuration,
                            form_schema = m.form_schema,
                            reader_id = m.reader_id,
                        }).Distinct().OrderBy(o => o.form_name).ToList();
                    }
                    else
                    {
                        formUsers = _dbcontext.FormUsers.Where(o => o.user_group_id == UserId).OrderBy(o => o.form_name).ToList();
                    }
                    resultList = formUsers.Select(o => new FormUsersDTO()
                    {
                        id = 0,
                        form_id = o.form_id,
                        form_name = o.form_name,
                        form_description = o.form_description,
                        AttachmentsCount = 0,//GetFormDetials(formUsers.Select(f => o.form_id).ToList()),
                        reader_configuration = null,//GetFormAdapterConfiguration(o.reader_configuration, GetFormConfiguration(o.form_schema)),
                        form_schema = o.form_schema,
                        reader_id = o.reader_id,
                        form_owner_id = isSecurityMember ? UserId : o.form_owner_id,
                        user_group_id = isSecurityMember ? UserId : o.user_group_id,
                        user_group_name = isSecurityMember ? UserId.ToString() : o.user_group_name,
                        day_cutoff = day_cutoff,
                        cutoff = cutoff_result,
                        kpis_associated = GetKpiAssociatedByFormId(o.form_id),
                        latest_input_date = isSecurityMember ? new DateTime(0) : _dbcontext.FormLogs.Any(p => p.id_form == o.form_id) ? _dbcontext.FormLogs.Where(q => q.id_form == o.form_id).Max(r => r.time_stamp) : new DateTime(0)
                    }).ToList();
                }
                if (formId > 0)
                {
                    formUser = _dbcontext.FormUsers.FirstOrDefault(o => o.form_id == formId);
                    var resultSingle = new FormUsersDTO()
                    {
                        id = formUser.id,
                        form_id = formUser.form_id,
                        form_name = formUser.form_name,
                        form_description = formUser.form_description,
                        reader_configuration = GetFormAdapterConfiguration(formUser.reader_configuration, GetFormConfiguration(formUser.form_schema)),
                        form_schema = formUser.form_schema,
                        reader_id = formUser.reader_id,
                        form_owner_id = formUser.form_owner_id,
                        user_group_id = formUser.user_group_id,
                        user_group_name = formUser.user_group_name,
                        day_cutoff = day_cutoff,
                        cutoff = cutoff_result,
                        latest_input_date = _dbcontext.FormLogs.Any(p => p.id_form == formId) ? _dbcontext.FormLogs.Where(q => q.id_form == formId).Max(r => r.time_stamp) : new DateTime(0)
                    };
                    resultList.Add(resultSingle);
                }
                return resultList;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<FormConfigurationDTO> GetFormConfiguration(string schema)
        {
            //var dto = new APIToWorkflowDTO() { input = schema };
            try
            {
                using (var client = new HttpClient())
                {
                    var bsiconf = _infomationAPI.GetConfiguration("be_bsi", "bsi_api_url");
                    var output = QuantisUtilities.FixHttpURLForCall(bsiconf.Value, "/home/APIToWorkflow");
                    client.BaseAddress = new Uri(output.Item1);
                    var dataAsString = JsonConvert.SerializeObject(new { input = schema });
                    var content = new StringContent(dataAsString);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = client.PostAsync(output.Item2, content).Result;
                    //var response = client.PostAsync(output.Item2, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
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
                                    into gj
                                    from subset in gj.DefaultIfEmpty()
                                    select new FormField()
                                    {
                                        name = f.name,
                                        label = subset?.text ?? String.Empty,
                                        mandatory = subset?.a_isMandatory ?? String.Empty,
                                        defaultValue = subset?.defaultValue ?? String.Empty,
                                        source = f.source,
                                        type = f.type,
                                        a_type = subset?.a_type ?? String.Empty,
                                    }).ToList()
            };
        }

        public List<WidgetDTO> GetAllWidgets()
        {
            try
            {
                var widget = _dbcontext.Widgets.Where(o => o.delete_date != null);
                return _widgetMapper.GetDTOs(widget.ToList());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool RemoveAttachment(int id)
        {
            try
            {
                var entity = _dbcontext.FormAttachments.FirstOrDefault(o => o.t_form_attachments_id == id);

                _dbcontext.Remove(entity);

                _dbcontext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public FormRuleDTO GetFormRuleByFormId(int Id)
        {
            try
            {
                var form = _dbcontext.FormRules.FirstOrDefault(o => o.form_id == Id);
                if (form == null)
                {
                    return null;
                }
                return _formRuleMapper.GetDTO(form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public CatalogKpiDTO GetKpiById(int Id)
        {
            try
            {
                var kpi = _dbcontext.CatalogKpi.Include(o => o.GlobalRule).Include(o => o.PrimaryCustomer).Include(o => o.SecondaryCustomer).Include(o => o.Sla).FirstOrDefault(o => o.id == Id);
                return _catalogKpiMapper.GetDTO(kpi);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public GroupDTO GetGroupById(int Id)
        {
            try
            {
                var group = _dbcontext.Groups.FirstOrDefault(o => o.group_id == Id);
                return _groupMapper.GetDTO(group);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public PageDTO GetPageById(int Id)
        {
            try
            {
                var page = _dbcontext.Pages.FirstOrDefault(o => o.page_id == Id);
                return _pageMapper.GetDTO(page);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public UserDTO GetUserById(string UserId)
        {
            try
            {
                var user = _dbcontext.CatalogUsers.FirstOrDefault(o => o.userid == UserId);
                return _userMapper.GetDTO(user);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<KPIOnlyContractDTO> GetKpiByFormId(int Id)
        {
            try
            {
                var kpi = _dbcontext.Forms.Include(o => o.CatalogKPIs).FirstOrDefault(o => o.form_id == Id);
                if (kpi == null && kpi.CatalogKPIs.Any())
                {
                    return null;
                }
                return kpi.CatalogKPIs.Select(o => new KPIOnlyContractDTO()
                {
                    contract = o.contract,
                    id_kpi = o.id_kpi,
                    global_rule_id = o.global_rule_id_bsi,
                    kpi_name_bsi = o.kpi_name_bsi,
                    target = o.target
                }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public WidgetDTO GetWidgetById(int Id)
        {
            try
            {
                var widget = _dbcontext.Widgets.FirstOrDefault(o => o.widget_id == Id);
                return _widgetMapper.GetDTO(widget);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<FormLVDTO> GetAllForms()
        {
            try
            {
                var forms = _dbcontext.Forms.Include(o => o.FormLogs).Include(o => o.Rules).OrderBy(o => o.form_name).ToList();
                var daycutoff = _infomationAPI.GetConfiguration("be_restserver", "day_cutoff");
                return forms.Select(o => new FormLVDTO()
                {
                    create_date = o.create_date,
                    form_description = o.form_description,
                    form_id = o.form_id,
                    form_name = o.form_name,
                    form_owner_id = o.form_owner_id,
                    modify_date = o.modify_date,
                    reader_id = o.reader_id,
                    rules_count = o.Rules.Count,
                    latest_input_date = o.FormLogs.Any() ? o.FormLogs.Max(p => p.time_stamp) : new DateTime(0),
                    day_cuttoff = (daycutoff == null) ? null : daycutoff.Value
                }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool SumbitForm(SubmitFormDTO dto)
        {
            using (var dbContextTransaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var form_log = new T_FormLog()
                    {
                        empty_form = dto.empty_form,
                        id_form = dto.form_id,
                        id_locale = dto.locale_id,
                        period = dto.period,
                        time_stamp = DateTime.Now,
                        user_id = dto.user_id,
                        year = dto.year
                    };
                    var form = _dbcontext.Forms.FirstOrDefault(o => o.form_id == dto.form_id);
                    if (form != null)
                    {
                        form.modify_date = DateTime.Now;
                        _dbcontext.SaveChanges(false);
                    }
                    _dbcontext.FormLogs.Add(form_log);
                    _dbcontext.SaveChanges(false);
                    T_NotifierLog notifier_log = _dbcontext.NotifierLogs.FirstOrDefault(o => o.id_form == form_log.id_form && o.period == form_log.period && o.year == form_log.year);
                    if (notifier_log != null)
                    {
                        notifier_log.is_ack = true;
                    }
                    else
                    {
                        notifier_log = new T_NotifierLog()
                        {
                            id_form = dto.form_id,
                            notify_timestamp = DateTime.Now,
                            remind_timestamp = null,
                            is_ack = true,
                            period = dto.period,
                            year = dto.year
                        };
                        _dbcontext.NotifierLogs.Add(notifier_log);
                        _dbcontext.SaveChanges(false);
                    }
                    if (CallFormAdapter(new FormAdapterDTO() { formID = dto.form_id, localID = dto.locale_id, forms = dto.inputs }))
                    {
                        dbContextTransaction.Commit();
                        return true;
                    }
                    else
                    {
                        dbContextTransaction.Rollback();
                        return false;
                    }
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    throw e;
                }
            };
        }

        public List<FormAttachmentDTO> GetAttachmentsByFormId(int formId)
        {
            try
            {
                var ents = _dbcontext.FormAttachments.Where(o => o.form_id == formId);
                var dtos = _fromAttachmentMapper.GetDTOs(ents.ToList());
                return dtos.OrderByDescending(o => o.create_date).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool SubmitAttachment(List<FormAttachmentDTO> dto)
        {
            try
            {
                List<T_FormAttachment> attachments = new List<T_FormAttachment>();
                foreach (var attach in dto)
                {
                    attachments.Add(_fromAttachmentMapper.GetEntity(attach, new T_FormAttachment()));
                }
                _dbcontext.FormAttachments.AddRange(attachments.ToArray());
                _dbcontext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetBSIServerURL()
        {
            try
            {
                var bsiconf = _infomationAPI.GetConfiguration("be_bsi", "bsi_api_url");
                return bsiconf.Value;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public LoginResultDTO Login(string username, string password)
        {
            try
            {
                if (password == "siteminderAccess")
                {
                    var usernameRete = "rete\\" + username.Replace("[", "").Replace("]", "").Replace("\"", "").Replace("\"", "");
                    var usr = _dbcontext.CatalogUsers.FirstOrDefault(o => o.userid.ToLower() == usernameRete.ToLower());
                    if (usr != null)
                    {
                        var token = MD5Hash(usr.userid + DateTime.Now.Ticks);
                        //var res = _oracleAPI.GetUserIdLocaleIdByUserName(usr.ca_bsi_account);
                        var res = _dbcontext.TUsers.FirstOrDefault(u => u.user_id == usr.ca_bsi_user_id && u.user_status == "ACTIVE");
                        if (res != null)
                        {
                            _dbcontext.Sessions.Add(new T_Session()
                            {
                                //user_id = res.Item1,
                                user_id = res.user_id,
                                user_name = usr.ca_bsi_account,
                                login_time = DateTime.Now,
                                session_token = token,
                                expire_time = DateTime.Now.AddMinutes(getSessionTimeOut())
                            });
                            _dbcontext.SaveChanges();
                            _cache.Remove("Permission_" + res.user_id);
                            var permissions = _infomationAPI.GetPermissionsByUserId(res.user_id).Select(o => o.Code).ToList();
                            _cache.GetOrCreate("Permission_" + res.user_id, entry => permissions);
                            var dash = _dbcontext.DB_Dashboards.FirstOrDefault(o => o.UserId == res.user_id && o.IsDefault);

                            return new LoginResultDTO()
                            {
                                Token = token,
                                UserID = res.user_id,
                                LocaleID = res.user_locale_id,
                                UserEmail = usr.mail,
                                UserName = usr.ca_bsi_account,
                                Permissions = permissions,
                                DefaultDashboardId = dash == null ? -1 : dash.Id,
                                UIVersion = _configuration["UIVersion"],
                            };
                        }
                    }
                    return null;
                }
                else
                {
                    var usr = _dbcontext.CatalogUsers.FirstOrDefault(o => o.ca_bsi_account.ToLower() == username.ToLower());
                    if (usr != null)
                    {
                        var secret_key = _infomationAPI.GetConfiguration("be_restserver", "secret_key");

                        var db_password = sha256_hash(secret_key.Value + usr.password);
                        if (password == db_password)
                        {
                            var token = MD5Hash(usr.userid + DateTime.Now.Ticks);
                            //var res = _oracleAPI.GetUserIdLocaleIdByUserName(usr.ca_bsi_account);
                            var res = _dbcontext.TUsers.FirstOrDefault(u => u.user_id == usr.ca_bsi_user_id && u.user_status == "ACTIVE");
                            if (res != null)
                            {
                                _dbcontext.Sessions.Add(new T_Session()
                                {
                                    //user_id = res.Item1,
                                    user_id = res.user_id,
                                    user_name = usr.ca_bsi_account,
                                    login_time = DateTime.Now,
                                    session_token = token,
                                    expire_time = DateTime.Now.AddMinutes(getSessionTimeOut())
                                });
                                _dbcontext.SaveChanges();
                                _cache.Remove("Permission_" + res.user_id);
                                var permissions = _infomationAPI.GetPermissionsByUserId(res.user_id).Select(o => o.Code).ToList();
                                _cache.GetOrCreate("Permission_" + res.user_id, entry => permissions);
                                var dash = _dbcontext.DB_Dashboards.FirstOrDefault(o => o.UserId == res.user_id && o.IsDefault);

                                return new LoginResultDTO()
                                {
                                    Token = token,
                                    UserID = res.user_id,
                                    LocaleID = res.user_locale_id,
                                    UserEmail = usr.mail,
                                    UserName = usr.ca_bsi_account,
                                    Permissions = permissions,
                                    DefaultDashboardId = dash == null ? -1 : dash.Id,
                                    UIVersion = _configuration["UIVersion"],
                                };
                            }
                        }
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Logout(string token)
        {
            try
            {
                var sesison = _dbcontext.Sessions.Single(o => o.session_token == token);
                sesison.login_time = DateTime.Now;
                _dbcontext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool ResetPassword(string username, string email)
        {
            try
            {
                var usr = _dbcontext.CatalogUsers.FirstOrDefault(o => o.ca_bsi_account == username && o.mail == email);
                if (usr != null)
                {
                    var randomPassword = RandomString(10);
                    usr.password = sha256_hash(randomPassword);
                    _dbcontext.SaveChanges();

                    List<string> listRecipients = new List<string>();
                    listRecipients.Add(email);
                    var emailSubject = "[KPI Management] Reset Password";
                    var emailBody = "<html>Nuova password: <b>" + randomPassword + "</b></html>";
                    return _smtpService.SendEmail(emailSubject, emailBody, listRecipients);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return false;
        }

        public int ArchiveKPIs(ArchiveKPIDTO dto)
        {
            try
            {
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlArchivedProvider")))
                {
                    con.Open();
                    var sp = @"save_record";
                    var command = new NpgsqlCommand(sp, con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue(":_name_kpi", dto.kpi_name);
                    command.Parameters.AddWithValue(":_interval_kpi", dto.kpi_interval);
                    command.Parameters.AddWithValue(":_value_kpi", dto.kpi_value);
                    command.Parameters.AddWithValue(":_ticket_id", dto.ticket_id);
                    command.Parameters.AddWithValue(":_close_timestamp_ticket", dto.ticket_close_timestamp);
                    command.Parameters.AddWithValue(":_archived", dto.isarchived);
                    command.Parameters.AddWithValue(":_raw_data_ids", dto.raw_data_ids);
                    var reader = (int)command.ExecuteScalar();
                    return reader;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool AddArchiveRawData(int global_rule_id, string period, string tracking_period)
        {
            try
            {
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                {
                    con.Open();
                    var query = @"select r.rule_id from t_rules r left join t_sla_versions sv on r.sla_version_id = sv.sla_version_id left join t_slas s on sv.sla_id = s.sla_id where sv.sla_status = 'EFFECTIVE' and s.sla_status = 'EFFECTIVE' and r.global_rule_id = :global_rule_id";
                    var command = new NpgsqlCommand(query, con);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue(":global_rule_id", global_rule_id);
                    using (var reader = command.ExecuteReader())
                    {
                        int rule_id = 0;
                        while (reader.Read())
                        {
                            rule_id = (reader.IsDBNull(reader.GetOrdinal("rule_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("rule_id")));
                        }
                        if (rule_id == 0) { return false; } // EXIT IF NO RULE_ID

                        List<EventResourceDTO> eventResource = GetEventResourceFromRule(rule_id);
                        string completewhereStatement = "";
                        var whereStatements = new List<string>();
                        foreach (var d in eventResource)
                        {
                            if (d.ResourceId == -1)
                            {
                                whereStatements.Add(string.Format("(event_type_id={0})", d.EventId));
                            }
                            else
                            {
                                whereStatements.Add(string.Format("(resource_id={0} AND event_type_id={1})", d.ResourceId, d.EventId));
                            }
                        }
                        if (eventResource.Any())
                        {
                            completewhereStatement = string.Format(" AND ({0})", string.Join(" OR ", whereStatements));
                        }
                        else { return false; } //EXIT IF NO eventResource
                        List<string> periods = new List<string>();
                        string month = period.Split('/').First();
                        string year = "20" + period.Split('/').Last();
                        switch (tracking_period)
                        {
                            case "TRIMESTRALE":
                                if (month == "03") { periods.Add(year + "_01"); periods.Add(year + "_02"); periods.Add(year + "_03"); }
                                if (month == "06") { periods.Add(year + "_04"); periods.Add(year + "_05"); periods.Add(year + "_06"); }
                                if (month == "09") { periods.Add(year + "_07"); periods.Add(year + "_08"); periods.Add(year + "_09"); }
                                if (month == "12") { periods.Add(year + "_10"); periods.Add(year + "_11"); periods.Add(year + "_12"); }
                                break;

                            case "QUADRIMESTRALE":
                                if (month == "04") { periods.Add(year + "_01"); periods.Add(year + "_02"); periods.Add(year + "_03"); periods.Add(year + "_04"); }
                                if (month == "08") { periods.Add(year + "_05"); periods.Add(year + "_06"); periods.Add(year + "_07"); periods.Add(year + "_08"); }
                                if (month == "12") { periods.Add(year + "_09"); periods.Add(year + "_10"); periods.Add(year + "_11"); periods.Add(year + "_12"); }
                                break;

                            case "SEMESTRALE":
                                if (month == "06") { periods.Add(year + "_01"); periods.Add(year + "_02"); periods.Add(year + "_03"); periods.Add(year + "_04"); periods.Add(year + "_05"); periods.Add(year + "_06"); }
                                if (month == "12") { periods.Add(year + "_07"); periods.Add(year + "_08"); periods.Add(year + "_09"); periods.Add(year + "_10"); periods.Add(year + "_11"); periods.Add(year + "_12"); }
                                break;

                            case "ANNUALE":
                                if (month == "12") { periods.Add(year + "_01"); periods.Add(year + "_02"); periods.Add(year + "_03"); periods.Add(year + "_04"); periods.Add(year + "_05"); periods.Add(year + "_06"); periods.Add(year + "_07"); periods.Add(year + "_08"); periods.Add(year + "_09"); periods.Add(year + "_10"); periods.Add(year + "_11"); periods.Add(year + "_12"); }
                                break;

                            default:
                                periods.Add(year + "_" + month);
                                break;
                        }

                        foreach (var tmp_period in periods)
                        {
                            using (var conOrig = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                            {
                                conOrig.Open();
                                var sp = @"insert into t_dt_de_archive_swap (created_by, event_type_id, reader_time_stamp, resource_id, time_stamp, data_source_id, raw_data_id, create_date, corrected_by, data, modify_date, reader_id, event_source_type_id, event_state_id, partner_raw_data_id, hash_data_key, global_rule_id) select created_by, event_type_id, reader_time_stamp, resource_id, time_stamp, data_source_id, raw_data_id, create_date, corrected_by, data, modify_date, reader_id, event_source_type_id, event_state_id, partner_raw_data_id, hash_data_key, " + global_rule_id + " as global_rule_id from t_dt_de_3_" + tmp_period + " WHERE 1=1 " + completewhereStatement;
                                var commandOrig = new NpgsqlCommand(sp, conOrig);
                                commandOrig.CommandType = CommandType.Text;
                                commandOrig.ExecuteScalar();
                                conOrig.Close();
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void AddArchiveKPI(ARulesDTO dto)
        {
            try
            {
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlArchivedProvider")))
                {
                    con.Open();
                    var sp = @"insert into a_rules (id_kpi,name_kpi,interval_kpi,value_kpi,ticket_id,close_timestamp_ticket,archived,customer_name,contract_name,kpi_name_bsi,rule_id_bsi,global_rule_id,tracking_period,symbol,kpi_description_bsi) values (:id_kpi,:name_kpi,:interval_kpi,:value_kpi,:ticket_id,:close_timestamp_ticket,:archived,:customer_name,:contract_name,:kpi_name_bsi,:rule_id_bsi,:global_rule_id,:tracking_period,:symbol,:kpi_description_bsi)";
                    var command = new NpgsqlCommand(sp, con);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue(":id_kpi", dto.id_kpi);
                    command.Parameters.AddWithValue(":name_kpi", dto.name_kpi);
                    command.Parameters.AddWithValue(":interval_kpi", dto.interval_kpi);
                    command.Parameters.AddWithValue(":value_kpi", dto.value_kpi);
                    command.Parameters.AddWithValue(":ticket_id", dto.ticket_id);
                    command.Parameters.AddWithValue(":close_timestamp_ticket", dto.close_timestamp_ticket);
                    command.Parameters.AddWithValue(":archived", dto.archived);
                    command.Parameters.AddWithValue(":customer_name", dto.customer_name);
                    command.Parameters.AddWithValue(":contract_name", dto.contract_name);
                    command.Parameters.AddWithValue(":kpi_name_bsi", dto.kpi_name_bsi);
                    command.Parameters.AddWithValue(":rule_id_bsi", dto.rule_id_bsi);
                    command.Parameters.AddWithValue(":global_rule_id", dto.global_rule_id);
                    command.Parameters.AddWithValue(":tracking_period", dto.tracking_period);
                    command.Parameters.AddWithValue(":symbol", dto.symbol);
                    command.Parameters.AddWithValue(":kpi_description_bsi", dto.kpi_description_bsi);
                    command.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<FormsFromCatalogDTO> GetFormsFromCatalog(int UserID, bool isSecurityMember, int fakeUserID, string type)
        {
            try
            {
                if (fakeUserID > 0) { UserID = fakeUserID; isSecurityMember = false; }
                var isCsv = false;
                if (type == "csvtracking" || type == "csvnotracking") { isCsv = true; }
                var day_cutoffValue = _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_restserver" && o.key == "day_cutoff");
                int todayDay = Int32.Parse(DateTime.Now.ToString("dd"));
                int day_cutoff = Int32.Parse(day_cutoffValue.value);
                bool cutoff_result;
                string thisMonth = DateTime.Now.ToString("MM");
                if (day_cutoff == 0) { cutoff_result = false; } else { if (todayDay < day_cutoff) { cutoff_result = false; } else { cutoff_result = true; } }

                var resultList = new List<FormsFromCatalogDTO>();
                string query = "";
                if (isSecurityMember)
                {
                    if (isCsv)
                    {
                        query = "select global_rule_name, global_rule_id, referent, monthtrigger, sla_name, file_name," +
                        " tracking_period, source_type, u.userid, u.ca_bsi_account, u.ca_bsi_user_id " +
                        "from(" +
                        " select ck.*, gr.global_rule_name, gr.global_rule_id, c.customer_name, c2.customer_name, s.sla_name " +
                        "from t_catalog_kpis ck " +
                        "left join t_global_rules gr on ck.global_rule_id_bsi = gr.global_rule_id " +
                        "left join t_customers c on c.customer_id = ck.primary_contract_party " +
                        "left join t_customers c2 on c2.customer_id = ck.secondary_contract_party " +
                        "left join t_slas s on s.sla_id = ck.sla_id_bsi ) temp " +
                        "left join t_catalog_users u on " +
                        "(u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 1) = '' then 'a' else split_part(temp.referent, '$', 1)end)  " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 1)) " +
                        "OR u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 2) = '' then 'a' else split_part(temp.referent, '$', 2)end) " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 2)) " +
                        "OR u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 3) = '' then 'a' else split_part(temp.referent, '$', 3)end) " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 3)) " +
                        "OR u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 4) = '' then 'a' else split_part(temp.referent, '$', 4)end) " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 4)) " +
                        "OR u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 5) = '' then 'a' else split_part(temp.referent, '$', 5)end) " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 5)) " +
                        ") where monthtrigger is not null and ca_bsi_user_id is not null " +
                        "and source_type = 'MANUALE CSV' ";
                    }
                    else
                    {
                        query = "select f.form_id, f.form_name, f.form_owner_id, f.form_description, attachments_count, " +
                        "latest_modified_date from t_forms f " +
                        "left join(select form_id, count(form_id) as attachments_count from t_form_attachments group by form_id) fa on f.form_id = fa.form_id " +
                        "left join(select id_form, max(time_stamp) as latest_modified_date from t_form_logs group by id_form) fl on f.form_id = fl.id_form ";
                    }
                }
                else
                {
                    if (isCsv)
                    {
                        query = "select global_rule_name, global_rule_id, referent, monthtrigger, sla_name, file_name," +
                        " tracking_period, source_type, u.userid, u.ca_bsi_account, u.ca_bsi_user_id " +
                        "from(" +
                        " select ck.*, gr.global_rule_name, gr.global_rule_id, c.customer_name, c2.customer_name, s.sla_name " +
                        "from t_catalog_kpis ck " +
                        "left join t_global_rules gr on ck.global_rule_id_bsi = gr.global_rule_id " +
                        "left join t_customers c on c.customer_id = ck.primary_contract_party " +
                        "left join t_customers c2 on c2.customer_id = ck.secondary_contract_party " +
                        "left join t_slas s on s.sla_id = ck.sla_id_bsi ) temp " +
                        "left join t_catalog_users u on " +
                        "(u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 1) = '' then 'a' else split_part(temp.referent, '$', 1)end)  " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 1)) " +
                        "OR u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 2) = '' then 'a' else split_part(temp.referent, '$', 2)end) " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 2)) " +
                        "OR u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 3) = '' then 'a' else split_part(temp.referent, '$', 3)end) " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 3)) " +
                        "OR u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 4) = '' then 'a' else split_part(temp.referent, '$', 4)end) " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 4)) " +
                        "OR u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 5) = '' then 'a' else split_part(temp.referent, '$', 5)end) " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 5)) " +
                        ") where monthtrigger is not null and ca_bsi_user_id is not null " +
                        "and source_type = 'MANUALE CSV' and u.ca_bsi_user_id = :ca_bsi_user_id";
                    }
                    else
                    {
                        query = "select global_rule_name, global_rule_id, temp.form_id, form_name, form_owner_id, form_description, referent, monthtrigger, sla_name," +
                        "attachments_count,latest_modified_date, tracking_period, source_type, u.userid, u.ca_bsi_account, u.ca_bsi_user_id " +
                        "from(" +
                        " select ck.*, gr.global_rule_name, gr.global_rule_id, f.*, c.customer_name, c2.customer_name, s.sla_name," +
                        "fa.attachments_count, fl.latest_modified_date " +
                        "from t_catalog_kpis ck " +
                        "left join t_global_rules gr on ck.global_rule_id_bsi = gr.global_rule_id " +
                        "left join t_forms f on ck.id_form = f.form_id " +
                        "left join t_customers c on c.customer_id = ck.primary_contract_party " +
                        "left join t_customers c2 on c2.customer_id = ck.secondary_contract_party " +
                        "left join t_slas s on s.sla_id = ck.sla_id_bsi " +
                        "left join (select form_id, count(form_id) as attachments_count from t_form_attachments group by form_id) fa on f.form_id = fa.form_id " +
                        "left join(select id_form, max(time_stamp) as latest_modified_date from t_form_logs group by id_form) fl on f.form_id = fl.id_form " +
                        "where f.form_id is not null ) temp " +
                        "left join t_catalog_users u on " +
                        "(u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 1) = '' then 'a' else split_part(temp.referent, '$', 1)end)  " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 1)) " +
                        "OR u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 2) = '' then 'a' else split_part(temp.referent, '$', 2)end) " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 2)) " +
                        "OR u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 3) = '' then 'a' else split_part(temp.referent, '$', 3)end) " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 3)) " +
                        "OR u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 4) = '' then 'a' else split_part(temp.referent, '$', 4)end) " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 4)) " +
                        "OR u.userid LIKE ('%\\' || case when split_part(temp.referent, '$', 5) = '' then 'a' else split_part(temp.referent, '$', 5)end) " +
                        "OR u.userid LIKE ('%/' || split_part(temp.referent, '$', 5)) " +
                        ") where monthtrigger is not null and ca_bsi_user_id is not null " +
                        "and u.ca_bsi_user_id = :ca_bsi_user_id and source_type != 'AUTOMATICO' and source_type != 'MANUALE CSV'";
                    }
                }
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                {
                    con.Open();
                    var command = new NpgsqlCommand(query, con);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue(":ca_bsi_user_id", UserID);
                    _dbcontext.Database.OpenConnection();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FormsFromCatalogDTO form = new FormsFromCatalogDTO();
                            form.id = 0;
                            form.form_id = isCsv ? 0 : reader.GetInt32(reader.GetOrdinal("form_id"));
                            form.form_name = isCsv ? (reader.IsDBNull(reader.GetOrdinal("file_name")) ? null : reader.GetString(reader.GetOrdinal("file_name"))) : reader.GetString(reader.GetOrdinal("form_name")); //file_name when csv
                            form.form_owner_id = isCsv ? 0 : reader.GetInt32(reader.GetOrdinal("form_owner_id"));
                            form.form_description = isCsv ? null : reader.IsDBNull(reader.GetOrdinal("form_description")) ? null : reader.GetString(reader.GetOrdinal("form_description"));
                            form.AttachmentsCount = isCsv ? 0 : reader.IsDBNull(reader.GetOrdinal("attachments_count")) ? 0 : reader.GetInt32(reader.GetOrdinal("attachments_count"));
                            form.latest_input_date = isCsv ? new DateTime(0) : reader.IsDBNull(reader.GetOrdinal("latest_modified_date")) ? new DateTime(0) : reader.GetDateTime(reader.GetOrdinal("latest_modified_date"));

                            form.user_group_id = 0;// isSecurityMember ? 0 : reader.GetInt32(reader.GetOrdinal("user_group_id"));
                            form.user_group_name = null; // isSecurityMember ? null : reader.GetString(reader.GetOrdinal("user_group_name"));
                            form.day_cutoff = day_cutoff;
                            form.cutoff = cutoff_result;
                            form.global_rule_name = isSecurityMember ? null : reader.GetString(reader.GetOrdinal("global_rule_name"));
                            form.referent = isSecurityMember ? null : reader.IsDBNull(reader.GetOrdinal("referent")) ? null : reader.GetString(reader.GetOrdinal("referent"));
                            form.monthtrigger = isSecurityMember ? null : reader.IsDBNull(reader.GetOrdinal("monthtrigger")) ? null : reader.GetString(reader.GetOrdinal("monthtrigger"));
                            form.sla_name = isSecurityMember ? null : reader.GetString(reader.GetOrdinal("sla_name"));
                            form.global_rule_id = isSecurityMember ? 0 : reader.GetInt32(reader.GetOrdinal("global_rule_id"));
                            form.tracking_period = isSecurityMember ? null : reader.IsDBNull(reader.GetOrdinal("tracking_period")) ? null : reader.GetString(reader.GetOrdinal("tracking_period"));

                            if (isSecurityMember)
                            {
                                resultList.Add(form);
                            }
                            else
                            {
                                if (type != null && (type == "nottracking" || type == "csvnotracking"))
                                {
                                    string[] arraySplit = reader.GetString(reader.GetOrdinal("monthtrigger")).Split(",");
                                    if (!arraySplit.Contains(thisMonth))
                                    {
                                        resultList.Add(form);
                                    }
                                }
                                else
                                {
                                    string[] arraySplit = reader.GetString(reader.GetOrdinal("monthtrigger")).Split(",");
                                    if (arraySplit.Contains(thisMonth))
                                    {
                                        resultList.Add(form);
                                    }
                                }
                            }
                        }
                        return resultList;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<LogDTO> GetLogs(int limit)
        {
            try
            {
                if (limit <= 0) { limit = 10; }
                var resultList = new List<LogDTO>();
                string query = "select * from t_exceptions order by 1 desc LIMIT :limit";
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                {
                    con.Open();
                    var command = new NpgsqlCommand(query, con);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue(":limit", limit);
                    _dbcontext.Database.OpenConnection();
                    var currentPeriod = DateTime.Now.AddMonths(-8).ToString("MM/yy");
                    var previousPeriod = DateTime.Now.AddMonths(-9).ToString("MM/yy");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LogDTO log = new LogDTO();
                            log.id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id"));
                            log.message = reader.IsDBNull(reader.GetOrdinal("message")) ? null : reader.GetString(reader.GetOrdinal("message"));
                            log.innerexceptions = reader.IsDBNull(reader.GetOrdinal("innerexceptions")) ? null : reader.GetString(reader.GetOrdinal("innerexceptions"));
                            log.stacktrace = reader.IsDBNull(reader.GetOrdinal("stacktrace")) ? null : reader.GetString(reader.GetOrdinal("stacktrace"));
                            log.loglevel = reader.IsDBNull(reader.GetOrdinal("loglevel")) ? null : reader.GetString(reader.GetOrdinal("loglevel"));
                            log.ex_timestamp = reader.IsDBNull(reader.GetOrdinal("ex_timestamp")) ? new DateTime(0) : reader.GetDateTime(reader.GetOrdinal("ex_timestamp"));

                            resultList.Add(log);
                        }
                    }
                    return resultList;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DistributionPslDTO GetDistributionByContract(string period, int sla_id)
        {
            try
            {
                var currentDistribution = new PslResultDTO();
                var previousDistribution = new PslResultDTO();
                int currentCompliant = 0;
                int currentNotCompliant = 0;
                int currentNotCalculated = 0;
                int previousCompliant = 0;
                int previousNotCompliant = 0;
                int previousNotCalculated = 0;
                string query = "select r.global_rule_id from t_rules r left join t_sla_versions s on r.sla_version_id = s.sla_version_id left join t_slas m on m.sla_id = s.sla_id where s.sla_status = 'EFFECTIVE' AND m.sla_status = 'EFFECTIVE' and m.sla_id = :sla_id";
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                {
                    con.Open();
                    var command = new NpgsqlCommand(query, con);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue(":sla_id", sla_id);
                    _dbcontext.Database.OpenConnection();
                    var currentPeriod = DateTime.Now.AddMonths(-8).ToString("MM/yy");
                    var previousPeriod = DateTime.Now.AddMonths(-9).ToString("MM/yy");
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            var currentPsl = _oracleAPI.GetPsl(currentPeriod, result.GetInt32(result.GetOrdinal("global_rule_id")), "MENSILE");
                            if (currentPsl != null && currentPsl.Any())
                            {
                                if (currentPsl.FirstOrDefault().result == "[Non Compliant]") { currentNotCompliant++; }
                                if (currentPsl.FirstOrDefault().result == "[Compliant]") { currentCompliant++; }
                                if (currentPsl.FirstOrDefault().result == "[NE]") { currentNotCalculated++; }
                            }
                            else
                            {
                                currentNotCalculated++;
                            }

                            var previousPsl = _oracleAPI.GetPsl(previousPeriod, result.GetInt32(result.GetOrdinal("global_rule_id")), "MENSILE");
                            if (previousPsl != null && previousPsl.Any())
                            {
                                if (previousPsl.FirstOrDefault().result == "[Non Compliant]") { previousNotCompliant++; }
                                if (previousPsl.FirstOrDefault().result == "[Compliant]") { previousCompliant++; }
                                if (previousPsl.FirstOrDefault().result == "[NE]") { previousNotCalculated++; }
                            }
                            else
                            {
                                previousNotCalculated++;
                            }
                        }

                        currentDistribution.Compliant = currentCompliant;
                        currentDistribution.NonCompliant = currentNotCompliant;
                        currentDistribution.NonCalcolato = currentNotCalculated;
                        currentDistribution.Escalation = null;
                        previousDistribution.Compliant = previousCompliant;
                        previousDistribution.NonCompliant = previousNotCompliant;
                        previousDistribution.NonCalcolato = previousNotCalculated;
                        previousDistribution.Escalation = null;

                        DistributionPslDTO distributionResult = new DistributionPslDTO();
                        distributionResult.currentPeriod = currentDistribution;
                        distributionResult.previousPeriod = previousDistribution;
                        return distributionResult;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<ARulesDTO> GetAllArchivedKPIs(string month, string year, string id_kpi, List<int> globalruleIds)
        {
            try
            {
                if (!globalruleIds.Any() || globalruleIds == null)
                {
                    return new List<ARulesDTO>();
                }
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlArchivedProvider")))
                {
                    con.Open();

                    var whereclause = " and (interval_kpi >=:interval_kpi and interval_kpi < (  :interval_kpi + interval '1 month') )";
                    var whereYear = " and (interval_kpi >=:interval_kpi and interval_kpi < (  :interval_kpi + interval '1 year') )";
                    var filterByKpiId = " and global_rule_id = :global_rule_id";
                    var sp = @"select * from a_rules where 1=1";
                    if ((month != null && month != "00") && (year != null))
                    {
                        sp += whereclause;
                    }
                    if ((month == "00" || month == null) && (year != null))
                    {
                        sp += whereYear;
                    }
                    if (id_kpi != null)
                    {
                        sp += filterByKpiId;
                    }
                    sp += " and global_rule_id in (" + string.Join(',', globalruleIds) + ")";
                    sp += " order by close_timestamp_ticket desc";
                    var command = new NpgsqlCommand(sp, con);

                    if ((month != null && month != "00") && (year != null))
                    {
                        command.Parameters.AddWithValue(":interval_kpi", new NpgsqlTypes.NpgsqlDate(Int32.Parse(year), Int32.Parse(month), Int32.Parse("01")));
                    }
                    if ((month == "00" || month == null) && (year != null))
                    {
                        command.Parameters.AddWithValue(":interval_kpi", new NpgsqlTypes.NpgsqlDate(Int32.Parse(year), Int32.Parse("01"), Int32.Parse("01")));
                    }
                    if ((id_kpi != null))
                    {
                        command.Parameters.AddWithValue(":global_rule_id", Int32.Parse(id_kpi));
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        List<ARulesDTO> list = new List<ARulesDTO>();
                        while (reader.Read())
                        {
                            ARulesDTO arules = new ARulesDTO();
                            arules.id_kpi = reader.GetString(reader.GetOrdinal("id_kpi"));
                            arules.name_kpi = reader.GetString(reader.GetOrdinal("name_kpi"));
                            arules.interval_kpi = reader.GetDateTime(reader.GetOrdinal("interval_kpi"));
                            arules.value_kpi = reader.GetString(reader.GetOrdinal("value_kpi"));
                            arules.ticket_id = reader.GetInt32(reader.GetOrdinal("ticket_id"));
                            arules.close_timestamp_ticket = reader.GetDateTime(reader.GetOrdinal("close_timestamp_ticket"));
                            arules.archived = reader.GetBoolean(reader.GetOrdinal("archived"));
                            arules.customer_name = reader.GetString(reader.GetOrdinal("customer_name"));
                            arules.contract_name = reader.GetString(reader.GetOrdinal("contract_name"));
                            arules.kpi_name_bsi = reader.GetString(reader.GetOrdinal("kpi_name_bsi"));
                            arules.rule_id_bsi = reader.GetInt32(reader.GetOrdinal("rule_id_bsi"));
                            arules.global_rule_id = reader.GetInt32(reader.GetOrdinal("global_rule_id"));
                            arules.tracking_period = reader.GetString(reader.GetOrdinal("tracking_period"));
                            arules.kpi_description_bsi = reader.IsDBNull(reader.GetOrdinal("kpi_description_bsi")) ? null : reader.GetString(reader.GetOrdinal("kpi_description_bsi"));
                            arules.symbol = (reader.IsDBNull(reader.GetOrdinal("symbol")) ? null : reader.GetString(reader.GetOrdinal("symbol")));
                            arules.progressive = _dbcontext.CatalogKpi.FirstOrDefault(o => o.global_rule_id_bsi == reader.GetInt32(reader.GetOrdinal("global_rule_id"))).progressive;
                            list.Add(arules);
                        }
                        return list;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public CreateTicketDTO GetKPICredentialToCreateTicket(int Id)
        {
            try
            {
                var kpi = _dbcontext.CatalogKpi.Include(o => o.GlobalRule).Include(o => o.PrimaryCustomer).Include(o => o.SecondaryCustomer).FirstOrDefault(o => o.id == Id);
                var psl = _oracleAPI.GetPsl(DateTime.Now.AddMonths(-1).ToString("MM/yy"), kpi.global_rule_id_bsi, kpi.tracking_period);
                string contractPartyName = (kpi.SecondaryCustomer == null) ? kpi.PrimaryCustomer.customer_name : kpi.PrimaryCustomer.customer_name + string.Format(" ({0})", kpi.SecondaryCustomer.customer_name);
                string customer = _infomationAPI.GetConfiguration("be_sdm", "customer")?.Value;
                return new CreateTicketDTO()
                {
                    Description = GenerateDiscriptionFromKPI(kpi, (psl != null && psl.Any()) ? psl.FirstOrDefault().result.Contains("[NE]") ? "[NE]" : psl.FirstOrDefault().provided_ce + " " + psl.FirstOrDefault().symbol + " " + psl.FirstOrDefault().result : "[NE]"),
                    ID_KPI = kpi.id_kpi,
                    GroupCategoryId = kpi.primary_contract_party,
                    Period = DateTime.Now.AddMonths(-1).ToString("MM/yy"),
                    Reference1 = kpi.referent_1,
                    Reference2 = kpi.referent_2,
                    Reference3 = kpi.referent_3,
                    SecondaryContractParty = kpi.secondary_contract_party,
                    Summary = kpi.id_kpi + "|" + kpi.GlobalRule.global_rule_name + "|" + kpi.contract + "|" + contractPartyName,
                    zz1_contractParties = kpi.primary_contract_party + "|" + (kpi.secondary_contract_party == null ? "" : kpi.secondary_contract_party.ToString()),
                    zz2_calcValue =
                        (psl != null && psl.Any()) ?
                        psl.FirstOrDefault().result.Contains("[NE]") ? "[NE]"
                        : psl.FirstOrDefault().result.Contains("[Nessun Evento]") ? "[Nessun Evento]"
                        : psl.FirstOrDefault().provided_ce + " " + psl.FirstOrDefault().symbol + " " + psl.FirstOrDefault().result
                        :
                        "[NE]",
                    zz3_KpiIds = kpi.id + "|" + kpi.global_rule_id_bsi,
                    Customer = customer
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string GenerateDiscriptionFromKPI(T_CatalogKPI kpi, string calc)
        {
            string skeleton = "INDICATORE: {0}\n" +
                "DESCRIZIONE: {1}\n" +
                "ESCALATION: {2}\n" +
                "TARGET: {3}\n" +
                "TIPILOGIA: {4}\n" +
                "VALORE: {5}\n" +
                "AUTORE: {6}\n" +
                "FREQUENZA: {7}";
            return string.Format(skeleton, kpi.GlobalRule.global_rule_name ?? "", kpi.kpi_description ?? "", kpi.escalation ?? "", kpi.target ?? "", kpi.kpi_type ?? "", calc, kpi.source_name ?? "", kpi.tracking_period ?? "");
        }

        public List<EventResourceName> GetEventResourceNames()
        {
            try
            {
                List<EventResourceName> list = new List<EventResourceName>();
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                {
                    con.Open();
                    var query = @"select * from t_event_resource_names";
                    var command = new NpgsqlCommand(query, con);
                    command.CommandType = CommandType.Text;
                    using (var readerTmp = command.ExecuteReader())
                    {
                        while (readerTmp.Read())
                        {
                            EventResourceName e_r_name = new EventResourceName();
                            e_r_name.id = readerTmp.GetInt32(readerTmp.GetOrdinal("id"));
                            e_r_name.name = readerTmp.GetString(readerTmp.GetOrdinal("name"));
                            e_r_name.type = readerTmp.GetString(readerTmp.GetOrdinal("type"));
                            list.Add(e_r_name);
                        }
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<ATDtDeDTO> GetRawDataByKpiID(string id_kpi, string month, string year)
        {
            try
            {
                List<ATDtDeDTO> list = new List<ATDtDeDTO>();
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                {
                    con.Open();
                    var query = @"select r.rule_id from t_rules r left join t_sla_versions sv on r.sla_version_id = sv.sla_version_id left join t_slas s on sv.sla_id = s.sla_id where sv.sla_status = 'EFFECTIVE' and s.sla_status = 'EFFECTIVE' and r.global_rule_id = :global_rule_id";
                    var command = new NpgsqlCommand(query, con);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue(":global_rule_id", Int32.Parse(id_kpi));
                    using (var readerTmp = command.ExecuteReader())
                    {
                        int rule_id = 0;
                        while (readerTmp.Read())
                        {
                            rule_id = (readerTmp.IsDBNull(readerTmp.GetOrdinal("rule_id")) ? 0 : readerTmp.GetInt32(readerTmp.GetOrdinal("rule_id")));
                        }
                        if (rule_id == 0) { return list; } // EXIT IF NO RULE_ID

                        List<EventResourceDTO> eventResource = GetEventResourceFromRule(rule_id);
                        string completewhereStatement = "";
                        var whereStatements = new List<string>();
                        foreach (var d in eventResource)
                        {
                            if (d.ResourceId == -1)
                            {
                                whereStatements.Add(string.Format("(event_type_id={0})", d.EventId));
                            }
                            else
                            {
                                whereStatements.Add(string.Format("(resource_id={0} AND event_type_id={1})", d.ResourceId, d.EventId));
                            }
                        }
                        if (eventResource.Any())
                        {
                            completewhereStatement = string.Format(" AND ({0})", string.Join(" OR ", whereStatements));
                        }
                        else { return list; } //EXIT IF NO eventResource

                        using (var con2 = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                        {
                            con2.Open();
                            var tablename2 = "t_dt_de_3_" + year + "_" + month;
                            var sp2 = @"select * from " + tablename2 + " where 1=1 " + completewhereStatement + " order by modify_date desc ";
                            var command2 = new NpgsqlCommand(sp2, con2);
                            using (var reader = command2.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ATDtDeDTO atdtde = new ATDtDeDTO();
                                    atdtde.created_by = reader.GetInt32(reader.GetOrdinal("created_by"));
                                    atdtde.event_type_id = reader.GetInt32(reader.GetOrdinal("event_type_id"));
                                    atdtde.reader_time_stamp = Convert.ToDateTime(reader.GetDateTime(reader.GetOrdinal("reader_time_stamp")));
                                    atdtde.resource_id = reader.GetInt32(reader.GetOrdinal("resource_id"));
                                    atdtde.time_stamp = Convert.ToDateTime(reader.GetDateTime(reader.GetOrdinal("time_stamp")));
                                    atdtde.data_source_id = (reader.IsDBNull(reader.GetOrdinal("data_source_id")) ? null : reader.GetString(reader.GetOrdinal("data_source_id")));
                                    atdtde.raw_data_id = reader.GetInt32(reader.GetOrdinal("raw_data_id"));
                                    atdtde.create_date = Convert.ToDateTime(reader.GetDateTime(reader.GetOrdinal("create_date")));
                                    atdtde.corrected_by = reader.GetInt32(reader.GetOrdinal("corrected_by"));
                                    atdtde.data = reader.GetString(reader.GetOrdinal("data"));
                                    atdtde.modify_date = Convert.ToDateTime(reader.GetDateTime(reader.GetOrdinal("modify_date")));
                                    atdtde.reader_id = reader.GetInt32(reader.GetOrdinal("reader_id"));
                                    atdtde.event_source_type_id = (reader.IsDBNull(reader.GetOrdinal("event_source_type_id")) ? null : reader.GetInt32(reader.GetOrdinal("event_source_type_id")).ToString());
                                    atdtde.event_state_id = reader.GetInt32(reader.GetOrdinal("event_state_id"));
                                    atdtde.partner_raw_data_id = reader.GetInt32(reader.GetOrdinal("partner_raw_data_id"));
                                    atdtde.hash_data_key = (reader.IsDBNull(reader.GetOrdinal("hash_data_key")) ? null : reader.GetString(reader.GetOrdinal("hash_data_key")));
                                    atdtde.id_kpi = id_kpi;//reader.GetInt32(reader.GetOrdinal("id_kpi"));
                                    list.Add(atdtde);
                                }
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<XYZDTO> GetDayLevelKPIData(int globalRuleId, int month, int year)
        {
            var res = new List<XYZDTO>();
            string query = @"select time_stamp,service_level_target,provided_ce from t_psl_0_day
                            where global_rule_id =:global_rule_id
                            and TRUNC(begin_time_stamp) >= TO_DATE(:start_period,'yyyy-mm-dd')
                            and TRUNC(time_stamp) <= TO_DATE(:end_period,'yyyy-mm-dd')
                            and complete_record=1";
            using (OracleConnection con = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    var startdate = new DateTime(year, month, 1);
                    var enddate = startdate.AddMonths(1).AddDays(-1);
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = query;
                    OracleParameter param1 = new OracleParameter("start_period", startdate.AddDays(-1).ToString("yyyy-MM-dd"));
                    OracleParameter param2 = new OracleParameter("end_period", enddate.ToString("yyyy-MM-dd"));
                    OracleParameter param3 = new OracleParameter("global_rule_id", globalRuleId);
                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var tar = new XYZDTO();
                        tar.XValue = ((DateTime)reader[0]).ToString("dd/MM/yyyy");
                        tar.YValue = (double)reader[1];
                        tar.ZValue = "Target";
                        res.Add(tar);
                        var pro = new XYZDTO();
                        pro.XValue = ((DateTime)reader[0]).ToString("dd/MM/yyyy");
                        pro.YValue = (reader[2] == DBNull.Value) ? null : (double?)reader[2];
                        pro.ZValue = "Provided";
                        res.Add(pro);
                    }
                }
            }
            return res;
        }

        public List<ATDtDeDTO> GetArchivedRawDataByKpiID(string id_kpi, string month, string year)
        {
            try
            {
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlArchivedProvider")))
                {
                    con.Open();
                    List<ATDtDeDTO> list = new List<ATDtDeDTO>();
                    var tablename = "a_dt_de_" + year + "_" + month;
                    if (TableExists(tablename))
                    {
                        var sp = @"select * from " + tablename + " WHERE global_rule_id = :global_rule_id order by modify_date desc";
                        var command = new NpgsqlCommand(sp, con);
                        command.Parameters.AddWithValue(":global_rule_id", Int32.Parse(id_kpi));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ATDtDeDTO atdtde = new ATDtDeDTO();
                                atdtde.created_by = reader.GetInt32(reader.GetOrdinal("created_by"));
                                atdtde.event_type_id = reader.GetInt32(reader.GetOrdinal("event_type_id"));
                                atdtde.reader_time_stamp = Convert.ToDateTime(reader.GetDateTime(reader.GetOrdinal("reader_time_stamp")));
                                atdtde.resource_id = reader.GetInt32(reader.GetOrdinal("resource_id"));
                                atdtde.time_stamp = Convert.ToDateTime(reader.GetDateTime(reader.GetOrdinal("time_stamp")));
                                atdtde.data_source_id = (reader.IsDBNull(reader.GetOrdinal("data_source_id")) ? null : reader.GetString(reader.GetOrdinal("data_source_id")));
                                atdtde.raw_data_id = reader.GetInt32(reader.GetOrdinal("raw_data_id"));
                                atdtde.create_date = Convert.ToDateTime(reader.GetDateTime(reader.GetOrdinal("create_date")));
                                atdtde.corrected_by = reader.GetInt32(reader.GetOrdinal("corrected_by"));
                                atdtde.data = reader.GetString(reader.GetOrdinal("data"));
                                atdtde.modify_date = Convert.ToDateTime(reader.GetDateTime(reader.GetOrdinal("modify_date")));
                                atdtde.reader_id = reader.GetInt32(reader.GetOrdinal("reader_id"));
                                atdtde.event_source_type_id = (reader.IsDBNull(reader.GetOrdinal("event_source_type_id")) ? null : reader.GetInt32(reader.GetOrdinal("event_source_type_id")).ToString());
                                atdtde.event_state_id = reader.GetInt32(reader.GetOrdinal("event_state_id"));
                                atdtde.partner_raw_data_id = reader.GetInt32(reader.GetOrdinal("partner_raw_data_id"));
                                atdtde.hash_data_key = (reader.IsDBNull(reader.GetOrdinal("hash_data_key")) ? null : reader.GetString(reader.GetOrdinal("hash_data_key")));
                                atdtde.id_kpi = id_kpi;//reader.GetInt32(reader.GetOrdinal("id_kpi"));
                                list.Add(atdtde);
                            }
                        }
                    }
                    return list;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*       public List<ATDtDeDTO> GetDetailsArchiveKPI(int idkpi, string month, string year) // NON USATA
               {
                   try
                   {
                       using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlArchivedProvider")))
                       {
                           con.Open();
                           List<ATDtDeDTO> list = new List<ATDtDeDTO>();
                           var tablename = "a_t_dt_de_" + idkpi + "_" + year + "_" + month ;

                           if( TableExists(tablename))
                           {
                               var sp = @"select * from " + tablename;

                               var command = new NpgsqlCommand(sp, con);

                               using (var reader = command.ExecuteReader())
                               {
                                   while (reader.Read())
                                   {
                                       //created_by | event_type_id | reader_time_stamp | resource_id | time_stamp | data_source_id | raw_data_id | create_date | corrected_by | data | modify_date | reader_id | event_source_type_id | event_state_id | partner_raw_data_id | hash_data_key | id_kpi
                                       ATDtDeDTO atdtde = new ATDtDeDTO();
                                       atdtde.created_by = reader.GetInt32(reader.GetOrdinal("created_by"));
                                       atdtde.event_type_id = reader.GetInt32(reader.GetOrdinal("event_type_id"));
                                       atdtde.reader_time_stamp = reader.GetDateTime(reader.GetOrdinal("reader_time_stamp"));
                                       atdtde.resource_id = reader.GetInt32(reader.GetOrdinal("resource_id"));
                                       atdtde.time_stamp = reader.GetDateTime(reader.GetOrdinal("time_stamp"));
                                       atdtde.data_source_id = reader.GetString(reader.GetOrdinal("data_source_id"));
                                       atdtde.raw_data_id = reader.GetInt32(reader.GetOrdinal("raw_data_id"));
                                       atdtde.create_date = reader.GetDateTime(reader.GetOrdinal("create_date"));
                                       atdtde.corrected_by = reader.GetInt32(reader.GetOrdinal("corrected_by"));
                                       atdtde.data = reader.GetString(reader.GetOrdinal("data"));
                                       atdtde.modify_date = reader.GetDateTime(reader.GetOrdinal("modify_date"));
                                       atdtde.reader_id = reader.GetInt32(reader.GetOrdinal("reader_id"));
                                       atdtde.event_source_type_id = reader.GetInt32(reader.GetOrdinal("event_source_type_id")).ToString();
                                       atdtde.event_state_id = reader.GetInt32(reader.GetOrdinal("event_state_id"));
                                       atdtde.partner_raw_data_id = reader.GetInt32(reader.GetOrdinal("partner_raw_data_id"));
                                       atdtde.hash_data_key = reader.GetString(reader.GetOrdinal("hash_data_key"));
                                       atdtde.id_kpi = reader.GetString(reader.GetOrdinal("id_kpi"));

                                       list.Add(atdtde);
                                   }
                               }
                           }

                           return list;
                       }
                   }
                   catch (Exception e)
                   {
                       throw e;
                   }
               } */

                        public List<FormAttachmentDTO> GetAttachmentsByKPIID(int kpiId)
        {
            try
            {
                var kpi = _dbcontext.CatalogKpi.Single(o => o.id == kpiId);
                var form = kpi.id_form;
                if (form == null || form == 0)
                {
                    return new List<FormAttachmentDTO>();
                }
                var attachments = _dbcontext.Forms.Include(o => o.Attachments).Single(p => p.form_id == form).Attachments;
                if (kpi.month != null)
                {
                    if (kpi.month.Split(',').Count() == 12)
                    {
                        attachments = attachments.Where(o => o.create_date.Year == DateTime.Now.AddMonths(-1).Year && o.create_date.Month == DateTime.Now.AddMonths(-1).Month).ToList();
                    }
                    else if (kpi.month.Split(',').Count() == 4)
                    {
                        attachments = attachments.Where(o => o.create_date.Year == DateTime.Now.AddMonths(-1).Year && o.create_date.Month <= DateTime.Now.AddMonths(-1).Month && o.create_date.Month >= DateTime.Now.AddMonths(-4).Month).ToList();
                    }
                    else if (kpi.month.Split(',').Count() == 2)
                    {
                        attachments = attachments.Where(o => o.create_date.Year == DateTime.Now.AddMonths(-1).Year && o.create_date.Month <= DateTime.Now.AddMonths(-1).Month && o.create_date.Month >= DateTime.Now.AddMonths(-7).Month).ToList();
                    }
                }
                return _fromAttachmentMapper.GetDTOs(attachments.ToList()).OrderByDescending(o => o.create_date).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<KPISDMExtraDTO> GetKPISDMExtraInformation(List<int> ids)
        {
            try
            {
                var form = _dbcontext.CatalogKpi.Where(o => ids.Contains(o.id));
                return form.Select(o => new KPISDMExtraDTO()
                {
                    id = o.id,
                    titolo = o.short_name,
                    referent = o.referent,
                    tipologia = o.kpi_type
                }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<EmailNotifierDTO> GetEmailNotifiers(string period)
        {
            try
            {
                var split = period.Split('/');
                var month = split[0];
                var year = split[1];
                var notifiers = month.Length > 0 ?
                        _dbcontext.EmailNotifiers.Include(o => o.Form).Where(p => p.notify_date.ToString("MM/yy") == period).ToList()
                    : _dbcontext.EmailNotifiers.Include(o => o.Form).Where(p => p.notify_date.ToString("yy") == year).ToList();

                return notifiers.Select(o => new EmailNotifierDTO()
                {
                    email_body = o.email_body,
                    id = o.id,
                    form_name = o.Form.form_name,
                    notify_date = o.notify_date,
                    period = o.period,
                    recipient = o.recipient,
                    type = o.type,
                    user_domain = o.user_domain
                }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<TUserDTO> GetAllTUsers()
        {
            try
            {
                var usr = _dbcontext.TUsers.Where(o => o.in_catalog == false && (o.user_status == "ACTIVE" || o.user_status == "INACTIVE") && o.user_organization_name != "INTERNAL").OrderByDescending(o => o.user_create_date);
                var dtos = usr.Select(o => new TUserDTO()
                {
                    user_email = o.user_email,
                    user_id = o.user_id,
                    user_locale_id = o.user_locale_id,
                    user_name = o.user_name,
                    user_status = o.user_status,
                    user_organization_name = o.user_organization_name
                }).ToList();
                return dtos;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<TRuleDTO> GetAllTRules()
        {
            try
            {
                using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                {
                    con.Open();
                    List<TRuleDTO> list = new List<TRuleDTO>();

                    var sp = @"select r.rule_id, r.global_rule_id, r.rule_name, r.rule_description, r.sla_version_id, r.service_level_target, r.rule_create_date, r.rule_modify_date, sv.sla_id, s.sla_name, sv.version_number, s.customer_id as primary_contract_party_id, c.customer_name as primary_contract_party_name, s.additional_customer_id as secondary_contract_party_id, c2.customer_name as secondary_contract_party_name
                            from t_rules r
                            left join t_sla_versions sv on r.sla_version_id = sv.sla_version_id
                            left join t_global_rules gr on r.global_rule_id = gr.global_rule_id
                            left join t_slas s on sv.sla_id = s.sla_id
                            left join t_customers c on s.customer_id = c.customer_id
                            left join t_customers c2 on s.additional_customer_id = c2.customer_id
                            where sv.sla_status = 'EFFECTIVE' and r.is_effective = 'Y' and s.sla_status = 'EFFECTIVE' and gr.in_catalog = false order by s.sla_name, r.rule_name";
                    var command = new NpgsqlCommand(sp, con);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TRuleDTO tRule = new TRuleDTO();
                            tRule.rule_id = reader.GetInt32(reader.GetOrdinal("rule_id"));
                            tRule.global_rule_id = reader.GetInt32(reader.GetOrdinal("global_rule_id"));
                            tRule.rule_name = reader.GetString(reader.GetOrdinal("rule_name"));
                            tRule.rule_description = (reader.IsDBNull(reader.GetOrdinal("rule_description")) ? null : reader.GetString(reader.GetOrdinal("rule_description")));
                            tRule.create_date = reader.GetDateTime(reader.GetOrdinal("rule_create_date"));
                            tRule.modify_date = reader.GetDateTime(reader.GetOrdinal("rule_modify_date"));
                            tRule.sla_version_id = reader.GetInt32(reader.GetOrdinal("sla_version_id"));
                            tRule.service_level_target = (reader.IsDBNull(reader.GetOrdinal("service_level_target")) ? 0 : reader.GetDouble(reader.GetOrdinal("service_level_target")));
                            tRule.sla_id = reader.GetInt32(reader.GetOrdinal("sla_id"));
                            tRule.sla_name = reader.GetString(reader.GetOrdinal("sla_name"));
                            tRule.version_number = reader.GetInt32(reader.GetOrdinal("version_number"));
                            tRule.primary_contract_party_id = reader.GetInt32(reader.GetOrdinal("primary_contract_party_id"));
                            tRule.primary_contract_party_name = reader.GetString(reader.GetOrdinal("primary_contract_party_name"));
                            tRule.secondary_contract_party_id = (reader.IsDBNull(reader.GetOrdinal("secondary_contract_party_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("secondary_contract_party_id")));
                            tRule.secondary_contract_party_name = (reader.IsDBNull(reader.GetOrdinal("secondary_contract_party_name")) ? null : reader.GetString(reader.GetOrdinal("secondary_contract_party_name")));
                            list.Add(tRule);
                        }
                    }
                    return list;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<ReportQueryLVDTO> GetOwnedReportQueries(int userId)
        {
            var entities = _dbcontext.ReportQueries.Include(o => o.Parameters).Where(o => o.owner_id == userId);
            var dtos = entities.Select(e => new ReportQueryLVDTO()
            {
                Id = e.id,
                CreatedOn = e.created_on,
                OwnerId = e.owner_id,
                OwnerName = e.Owner.user_name,
                QueryName = e.query_name,
                FolderName=e.folder_name,
                FolderOwnerName=e.folder_owner_name,
                ParameterCount = e.Parameters.Count,
                IsEnabled = e.is_enable
            });
            return dtos.OrderBy(o => o.QueryName).ToList();
        }

        public List<ReportQueryLVDTO> GetAssignedReportQueries(int userId)
        {
            var entities = _dbcontext.ReportQueryAssignments.Include(o => o.Query).Where(o => o.user_id == userId).Select(o => o.Query).Where(o => o.is_enable).ToList();
            var dtos = entities.Select(e => new ReportQueryLVDTO()
            {
                Id = e.id,
                CreatedOn = e.created_on,
                OwnerId = e.owner_id,
                OwnerName = e.Owner.user_name,
                QueryName = e.query_name,
                FolderName = e.folder_name,
                FolderOwnerName = e.folder_owner_name,
                ParameterCount = e.Parameters.Count
            });
            return dtos.OrderBy(o => o.QueryName).ToList();
        }

        public ReportQueryDetailDTO GetReportQueryDetailByID(int id, int userId)
        {
            if (_dbcontext.ReportQueries.Any(o => o.id == id && o.owner_id == userId) || _dbcontext.ReportQueryAssignments.Any(o => o.query_id == id && o.user_id == userId))
            {
                var entity = _dbcontext.ReportQueries.Include(o => o.Parameters).FirstOrDefault(o => o.id == id);
                var dto = new ReportQueryDetailDTO()
                {
                    Id = entity.id,
                    QueryName = entity.query_name,
                    QueryText = entity.query_text,
                    OwnerId = entity.owner_id,
                    FolderName=entity.folder_name,
                    FolderOwnerName=entity.folder_owner_name,
                    Parameters = entity.Parameters.Select(p => new KeyValuePairDTO()
                    {
                        Key = p.parameter_key,
                        Value = p.parameter_value
                    }).ToList()
                };
                return dto;
            }
            return null;
        }

        public void AddEditReportQuery(ReportQueryDetailDTO dto, int userId)
        {
            if (dto.Id == 0)
            {
                var entity = new T_ReportQuery();
                entity.query_name = dto.QueryName;
                entity.query_text = dto.QueryText;
                entity.created_on = DateTime.Now;
                entity.folder_name = dto.FolderName;
                entity.folder_owner_name = dto.FolderOwnerName;
                entity.owner_id = userId;
                entity.is_enable = true;
                entity.Parameters = dto.Parameters.Select(o => new T_ReportQueryParameter()
                {
                    parameter_value = o.Value,
                    parameter_key = o.Key,
                }).ToList();
                _dbcontext.ReportQueries.Add(entity);
                _dbcontext.SaveChanges();
            }
            else
            {
                var entity = _dbcontext.ReportQueries.FirstOrDefault(o => o.id == dto.Id);
                entity.query_name = dto.QueryName;
                entity.query_text = dto.QueryText;
                entity.folder_name = dto.FolderName;
                entity.folder_owner_name = dto.FolderOwnerName;
                _dbcontext.SaveChanges();
                var parameters = _dbcontext.ReportQueryParameters.Where(o => o.query_id == dto.Id);
                _dbcontext.RemoveRange(parameters.ToArray());
                var param = dto.Parameters.Select(o => new T_ReportQueryParameter()
                {
                    parameter_value = o.Value,
                    parameter_key = o.Key,
                    query_id = dto.Id
                }).ToList();
                _dbcontext.ReportQueryParameters.AddRange(param.ToArray());
                _dbcontext.SaveChanges();
            }
        }

        public void EnableDisableReportQuery(int id, bool isenable, int userId)
        {
            var entity = _dbcontext.ReportQueries.FirstOrDefault(o => o.id == id);
            if (entity.owner_id == userId)
            {
                entity.is_enable = isenable;
                _dbcontext.SaveChanges();
            }
        }

        public string GetCatalogEmailByUser(int userId)
        {
            return _dbcontext.CatalogUsers.FirstOrDefault(o => o.ca_bsi_user_id == userId)?.mail;
        }

        public int CreateBooklet(CreateBookletDTO dto, int userId)
        {
            var sender = _cache.GetOrCreate("SMTP_from", p => _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "notifier_from").value);
            var senderPassword = _cache.GetOrCreate("SMTP_senderPassword", p => _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "sender_password").value);
            var senderUsername = _cache.GetOrCreate("SMTP_senderUsername", p => _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "sender_username").value);
            var serverHost = _cache.GetOrCreate("SMTP_serverHost", p => _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "server_host").value);
            var mailingList = new List<string>() { serverHost, senderUsername, senderPassword, sender, dto.RecipientEmail };
            string mainPath = _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_booklet" && o.key == "booklet_main_path").value;
            var contractIds = dto.BookletContracts.Select(p => p.ContractId).ToList();
            var slas = _dbcontext.Slas.Where(o => contractIds.Contains(o.sla_id)).Select(o=>new 
            {
                contractVersion=o.current_version_id,
                contractId=o.sla_id,
                contractName=o.sla_name
            });
            var contractList = (from d in dto.BookletContracts
                                join s in slas on d.ContractId equals s.contractId
                                select new CreateBookletWebBaseDTO()
                                {
                                    BookletDocumentId = d.BookletDocumentId,
                                    ContractName = s.contractName,
                                    ContractVersionId = s.contractVersion+""
                                }).ToList();

            var bookletDTO = new CreateBookletWebServiceDTO()
            {
                BookletContracts=contractList,
                MailSetup = mailingList,
                MainPath = mainPath,
                UserId = userId                
            };
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(10);
                var con = GetBSIServerURL();
                var apiPath = "/api/Booklet/CreateBooklet";
                var output = QuantisUtilities.FixHttpURLForCall(con, apiPath);
                client.BaseAddress = new Uri(output.Item1);
                var dataAsString = JsonConvert.SerializeObject(bookletDTO);
                var content = new StringContent(dataAsString);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = client.PostAsync(output.Item2, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var respo = response.Content.ReadAsStringAsync().Result;
                    respo = respo.Replace("\"", "");
                    _dbcontext.LogInformation($"The return from Create Booklet is valid the input is:{dataAsString} and response as {respo}");
                    return 1;
                }
                else
                {
                    throw new Exception("The return from Create Booklet is not valid the input is:" + dataAsString);
                }
            }

            return 0;
        }

        //public void DeleteReportQuery(int id, int userId)
        //{
        //    var entity = _dbcontext.ReportQueries.FirstOrDefault(o => o.id == id);
        //    if (entity.owner_id == userId)
        //    {
        //        var param = _dbcontext.ReportQueryParameters.Where(o => o.query_id == id);
        //        _dbcontext.ReportQueryParameters.RemoveRange(param.ToArray());
        //        var assign = _dbcontext.ReportQueryAssignments.Where(o => o.query_id == id);
        //        _dbcontext.ReportQueryAssignments.RemoveRange(assign.ToArray());
        //        _dbcontext.SaveChanges();
        //        _dbcontext.ReportQueries.Remove(entity);
        //        _dbcontext.SaveChanges();
        //    }
        //}

        public void AssignReportQuery(MultipleRecordsDTO records, int ownerId)
        {
            if (_dbcontext.ReportQueries.Any(o => o.id == records.Id && o.owner_id == ownerId))
            {
                var assigns = _dbcontext.ReportQueryAssignments.Where(o => o.query_id == records.Id);
                _dbcontext.ReportQueryAssignments.RemoveRange(assigns.ToArray());
                _dbcontext.SaveChanges();
                var assignments = records.Ids.Distinct().Select(o => new T_ReportQueryAssignment()
                {
                    query_id = records.Id,
                    user_id = o
                }).ToArray();
                _dbcontext.ReportQueryAssignments.AddRange(assignments);
                _dbcontext.SaveChanges();
            }
        }

        public object ExecuteReportQuery(ReportQueryDetailDTO dto, int userId)
        {
            string query = dto.QueryText;
            foreach (var p in dto.Parameters)
            {
                if (p.Key.Length > 0 && p.Value.Length > 0)
                {
                    query = query.Replace(p.Key, p.Value);
                }
            }
            if (query.Trim() == "$kpiCalculationStatus")
            {
                var rules = _dbcontext.UserKPIs.Where(o => o.user_id == userId).Select(o => o.global_rule_id).ToList();
                var conditionString = QuantisUtilities.GetOracleGlobalRuleInQuery("g.global_rule_id", rules);
                query = string.Format(WorkFlowConstants.KPI_Calculation_Status_Query, conditionString);
                if (rules.Count == 0)
                {
                    List<object> arrayerror = new List<object>();
                    arrayerror.Add(new { Errore = "Nessun KPI associato all'utente" });
                    return arrayerror;
                }
            }
            using (OracleConnection con = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = query;
                        OracleDataReader reader = cmd.ExecuteReader();
                        DataTable myTable = new DataTable();
                        myTable.Load(reader);
                        return myTable;
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                    }
                }
            }
        }

        public List<UserReportQueryAssignmentDTO> GetAllUsersAssignedQueries(int queryid, int userId)
        {
            try
            {
                var users = _dbcontext.CatalogUsers.Where(o => o.ca_bsi_user_id != null && o.ca_bsi_user_id != userId).ToList();
                var userDtos = _userMapper.GetDTOs(users.ToList());
                var landingpages = _dbcontext.ReportQueryAssignments.Where(o => o.query_id == queryid).ToList();
                return (from usrs in userDtos
                        join landpages in landingpages on usrs.ca_bsi_user_id equals landpages.user_id
                        into gj
                        from subset in gj.DefaultIfEmpty()
                        select new UserReportQueryAssignmentDTO()
                        {
                            ca_bsi_account = usrs.ca_bsi_account,
                            ca_bsi_user_id = usrs.ca_bsi_user_id,
                            id = usrs.id,
                            mail = usrs.mail,
                            manager = usrs.manager,
                            name = usrs.name,
                            organization = usrs.organization,
                            surname = usrs.surname,
                            userid = usrs.userid,
                            isAssigned = subset == null ? false : true
                        }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<PersonalReportLVDTO> GetPersonalReportsLV(int userId)
        {
            var reports = _dbcontext.PersonalReports.Where(o => o.user_id == userId);
            var kpiIds = reports.Select(o => o.global_rule_id).ToList();
            var kpisDetails=_infomationAPI.GetKPIDetails(kpiIds);
            var dtos= (from r in reports
                join d in kpisDetails on r.global_rule_id equals d.Global_Rule_Id
                select new PersonalReportLVDTO()
                {
                    id = r.id,
                    global_rule_id = r.global_rule_id,
                    name = r.name,
                    contract_name = d.Sla_Name,
                    contract_party_name = d.Customer_name,
                    kpi_name = d.Rule_Name

                }).ToList();
            return dtos;
        }

        public PersonalReportDTO GetPersonalReportDetail(int id)
        {
            var report = _dbcontext.PersonalReports.FirstOrDefault(o => o.id == id);
            if (report == null)
                return null;
            var dto = _personalReportMapper.GetDTO(report);
            return dto;
        }

        public void AddUpdatePersonalReport(PersonalReportDTO dto,int userId)
        {
            if (dto.id == 0)
            {
                var entity=new T_PersonalReport();
                entity=_personalReportMapper.GetEntity(dto, entity);
                entity.user_id = userId;
                _dbcontext.PersonalReports.Add(entity);
                _dbcontext.SaveChanges();

            }
            else
            {
                var entity = _dbcontext.PersonalReports.FirstOrDefault(o => o.id == dto.id);
                entity = _personalReportMapper.GetEntity(dto, entity);
                _dbcontext.SaveChanges();
            }
        }

        public void DeletePersonalReport(int id)
        {
            var entity = _dbcontext.PersonalReports.FirstOrDefault(o => o.id == id);
            if (entity != null)
            {
                _dbcontext.PersonalReports.Remove(entity);
                _dbcontext.SaveChanges();
            }
            
        }


        #region privateFunctions

        /*       private List<int> GetRawIdsFromResource(List<EventResourceDTO> dto, string period)
               {
                   try
                   {
                       using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
                       {
                           con.Open();
                           var output = new List<int>();
                           var month = period.Split('/').FirstOrDefault();
                           var year = "20" + period.Split('/').LastOrDefault();
                           string completewhereStatement = "";
                           var whereStatements = new List<string>();
                           foreach (var d in dto)
                           {
                               if (d.ResourceId == -1)
                               {
                                   whereStatements.Add(string.Format("(event_type_id={0})", d.EventId));
                               }
                               else
                               {
                                   whereStatements.Add(string.Format("(resource_id={0} AND event_type_id={1})", d.ResourceId, d.EventId));
                               }
                           }
                           if (dto.Any())
                           {
                               completewhereStatement = string.Format(" AND ({0})", string.Join(" OR ", whereStatements));
                           }
                           var sp = string.Format("Select event_type_id,resource_id,raw_data_id from t_dt_de_3_{0}_{1} where 1=1 {2}", year, month, completewhereStatement);
                           var command = new NpgsqlCommand(sp, con);
                           using (var reader = command.ExecuteReader())
                           {
                               while (reader.Read())
                               {
                                   output.Add(reader.GetInt32(reader.GetOrdinal("raw_data_id")));
                               }

                               return output;
                           }
                       }
                   }
                   catch (Exception e)
                   {
                       throw e;
                   }
               }*/
        private int GetWorkflowDay(string organizationUnit, int sla_id)
        {
            var query = "";
            int workflow_day = 0;
            int organizationUnitID = organizationUnit != null ? organizationUnit != "" ? Int32.Parse(organizationUnit) : 0 : 0;
            if(organizationUnitID == 0)
            {
                query = "SELECT workflow_day FROM t_organization_unit_workflow WHERE sla_id = :sla_id and organization_unit_id = -1";
            }
            else
            {
                query = "SELECT workflow_day FROM t_organization_unit_workflow WHERE sla_id = :sla_id and organization_unit_id = :organizationUnitID";
            }
            
            
            using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")))
            {
                con.Open();
                var command = new NpgsqlCommand(query, con);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue(":sla_id", sla_id);
                command.Parameters.AddWithValue(":organizationUnitID", organizationUnitID);
                _dbcontext.Database.OpenConnection();
                var configExists = command.ExecuteReader();
                configExists.Read();
                if (configExists != null && configExists.HasRows)
                {
                    workflow_day = configExists.GetInt32(configExists.GetOrdinal("workflow_day"));
                }
            }
            return workflow_day;
        }

        private bool CallFormAdapter(FormAdapterDTO dto)
        {
            using (var client = new HttpClient())
            {
                var con = GetBSIServerURL();
                var apiPath = "/api/FormAdapter/RunAdapter";
                var output = QuantisUtilities.FixHttpURLForCall(con, apiPath);
                client.BaseAddress = new Uri(output.Item1);
                var dataAsString = JsonConvert.SerializeObject(dto);
                var content = new StringContent(dataAsString);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = client.PostAsync(output.Item2, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var res = response.Content.ReadAsStringAsync().Result;
                    if (res == "2" || res == "1" || res == "3")
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("The return from Form Adapter is not valid value is:" + res);
                    }
                }
                else
                {
                    throw new Exception(string.Format("Call to form adapter has failed. BaseURL: {0} APIPath: {1} Data:{2}", output.Item1, output.Item2, dataAsString));
                }
            }
        }

        private int getSessionTimeOut()
        {
            var session = _infomationAPI.GetConfiguration("be_restserver", "session_timeout");
            if (session != null)
            {
                int value = Int32.Parse(session.Value);
                return value;
            }
            return 15;
        }

        private string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        private string RandomString(int size)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[size];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }

        private string sha256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();
            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }

        private bool TableExists(string tableName)
        {
            string sql = "SELECT * FROM information_schema.tables WHERE table_name = '" + tableName + "'";
            using (var con = new NpgsqlConnection(_configuration.GetConnectionString("DataAccessPostgreSqlArchivedProvider")))
            {
                using (var cmd = new NpgsqlCommand(sql))
                {
                    if (cmd.Connection == null)
                        cmd.Connection = con;
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    lock (cmd)
                    {
                        using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                        {
                            try
                            {
                                if (rdr != null && rdr.HasRows)
                                    return true;
                                return false;
                            }
                            catch (Exception)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }

        private IQueryable<T_CatalogUser> CreateGetUserQuery(UserFilterDTO filter)
        {
            var users = _dbcontext.CatalogUsers as IQueryable<T_CatalogUser>;
            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                users = users.Where(o => o.name.Contains(filter.SearchText) ||
                o.surname.Contains(filter.SearchText) ||
                o.ca_bsi_account.Contains(filter.SearchText) ||
                o.organization.Contains(filter.SearchText) ||
                o.mail.Contains(filter.SearchText) ||
                o.manager.Contains(filter.SearchText));
            }
            if (!string.IsNullOrEmpty(filter.Name))
            {
                users = users.Where(o => o.name.Contains(filter.Name));
            }
            if (!string.IsNullOrEmpty(filter.Surname))
            {
                users = users.Where(o => o.surname.Contains(filter.Surname));
            }
            return users;
        }

        #endregion privateFunctions
    }
}