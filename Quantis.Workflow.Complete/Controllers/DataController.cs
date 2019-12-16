using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quantis.WorkFlow.Services;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.API;
using Quantis.WorkFlow.Services.DTOs.Information;
using Quantis.WorkFlow.Services.DTOs.Widgets;
using Quantis.WorkFlow.Services.Framework;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class DataController : ControllerBase
    {
        private IDataService _dataAPI { get; set; }
        private IInformationService _informationAPI { get; set; }

        public DataController(IDataService dataAPI, IInformationService informationAPI)
        {
            _dataAPI = dataAPI;
            _informationAPI = informationAPI;
        }

        [HttpGet("CronJobsScheduler")]
        public bool CronJobsScheduler()
        {
            return _dataAPI.CronJobsScheduler();
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetAllWidgets")]
        public List<WidgetDTO> GetAllWidgets()
        {
            return _dataAPI.GetAllWidgets();
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("RemoveAttachment/{id}")]
        public bool RemoveAttachment(int id)
        {
            return _dataAPI.RemoveAttachment(id);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetEmailHistory")]
        public List<NotifierLogDTO> GetEmailHistory()
        {
            return _dataAPI.GetEmailHistory();
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetWidgetById/{id}")]
        public WidgetDTO GetWidgetById(int id)
        {
            return _dataAPI.GetWidgetById(id);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpPost("AddUpdateWidget")]
        public bool AddUpdateWidget([FromBody]WidgetDTO dto)
        {
            return _dataAPI.AddUpdateWidget(dto);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpPost("GetAllPagedUsers")]
        public PagedList<UserDTO> GetAllPagedUsers([FromBody]UserFilterDTO filter)
        {
            return _dataAPI.GetAllPagedUsers(filter);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetAllUsers")]
        public List<UserDTO> GetAllUsers()
        {
            return _dataAPI.GetAllUsers();
        }

        [Authorize(WorkFlowPermissions.VIEW_CONFIGURATION_USER_ROLES)]
        [HttpGet("GetUsersByRoleId")]
        public List<UserDTO> GetUsersByRoleId(int roleId)
        {
            return _dataAPI.GetUsersByRoleId(roleId);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetUserById")]
        public UserDTO GetUserById(string UserId)
        {
            return _dataAPI.GetUserById(UserId);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpPost("AddUpdateUser")]
        public bool AddUpdateUser([FromBody]UserDTO dto)
        {
            return _dataAPI.AddUpdateUser(dto);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetAllPages")]
        public List<PageDTO> GetAllPages()
        {
            return _dataAPI.GetAllPages();
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetPageById/{id}")]
        public PageDTO GetPageById(int id)
        {
            return _dataAPI.GetPageById(id);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpPost("AddUpdatePage")]
        public bool AddUpdatePage([FromBody]PageDTO dto)
        {
            return _dataAPI.AddUpdatePage(dto);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetAllGroups")]
        public List<GroupDTO> GetAllGroups()
        {
            return _dataAPI.GetAllGroups();
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetGroupById/{id}")]
        public GroupDTO GetGroupById(int id)
        {
            return _dataAPI.GetGroupById(id);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpPost("AddUpdateGroup")]
        public bool AddUpdateGroup([FromBody]GroupDTO dto)
        {
            return _dataAPI.AddUpdateGroup(dto);
        }

        [Authorize(WorkFlowPermissions.VIEW_CATALOG_KPI)]
        [HttpGet("GetAllKpis")]
        public List<CatalogKpiDTO> GetAllKpis()
        {
            return _dataAPI.GetAllKpis();
        }

        [Authorize(WorkFlowPermissions.VIEW_CATALOG_KPI)]
        [HttpGet("GetKpiById/{id}")]
        public CatalogKpiDTO GetKpiById(int id)
        {
            return _dataAPI.GetKpiById(id);
        }
        [Authorize(WorkFlowPermissions.VIEW_CATALOG_KPI)]
        [HttpGet("GetKpiByGlobalRuleId/{global_rule_id}")]
        public CatalogKpiDTO GetKpiByGlobalRuleId(int global_rule_id)
        {
            return _dataAPI.GetKpiByGlobalRuleId(global_rule_id);
        }
        [Authorize(WorkFlowPermissions.VIEW_CATALOG_KPI)]
        [HttpPost("AddUpdateKpi")]
        public bool AddUpdateKpi([FromBody]CatalogKpiDTO dto)
        {
            return _dataAPI.AddUpdateKpi(dto);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetKpiByFormId/{id}")]
        public List<KPIOnlyContractDTO> GetKpiByFormId(int id)
        {
            return _dataAPI.GetKpiByFormId(id);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetFormRuleByFormId/{id}")]
        public FormRuleDTO GetFormRuleByFormId(int id)
        {
            return _dataAPI.GetFormRuleByFormId(id);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpPost("AddUpdateFormRule")]
        public bool AddUpdateFormRule([FromBody]FormRuleDTO dto)
        {
            return _dataAPI.AddUpdateFormRule(dto);
        }

        [HttpGet("Login")]
        public IActionResult Login(string username, string password)
        {
            var data = _dataAPI.Login(username, password);
            if (data != null)
            {
                return Ok(data);
            }
            var json = new { error = "Errore durente il Login", description = "Username o Password errati." };
            return StatusCode(StatusCodes.Status403Forbidden, json);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("Logout")]
        public void Logout()
        {
            var usr = (HttpContext.User) as AuthUser;
            if (usr != null)
            {
                _dataAPI.Logout(usr.SessionToken);
            }
        }

        [HttpGet("ResetPassword")]
        public bool ResetPassword(string username, string email)
        {
            return _dataAPI.ResetPassword(username, email);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpPost("SubmitForm")]
        [DisableRequestSizeLimit]
        public bool SubmitForm([FromBody]SubmitFormDTO dto)
        {
            return _dataAPI.SumbitForm(dto);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpPost("SubmitAttachment")]
        [DisableRequestSizeLimit]
        public bool SubmitAttachment([FromBody]List<FormAttachmentDTO> dto)
        {
            return _dataAPI.SubmitAttachment(dto);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetAttachmentsByFormId")]
        public List<FormAttachmentDTO> GetAttachmentsByFormId(int formId)
        {
            return _dataAPI.GetAttachmentsByFormId(formId);
        }

        [Authorize(WorkFlowPermissions.VIEW_CATALOG_UTENTI)]
        [HttpGet("GetAllTUsers")]
        public List<TUserDTO> GetAllTUsers()
        {
            return _dataAPI.GetAllTUsers();
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpPost("ArchiveKPIs")]
        public int ArchiveKPIs([FromBody]ArchiveKPIDTO dto)
        {
            return _dataAPI.ArchiveKPIs(dto);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetAllForms")]
        public List<FormLVDTO> GetAllForms()
        {
            return _dataAPI.GetAllForms();
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetAllAPIs")]
        public List<ApiDetailsDTO> GetAllAPIs()
        {
            return _dataAPI.GetAllAPIs();
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetAllArchivedKPIs")]
        public List<ARulesDTO> GetAllArchivedKPIs(string month, string year, string id_kpi)
        {
            var user = HttpContext.User as AuthUser;
            var globalrules = _informationAPI.GetGlobalRulesByUserId(user.UserId);
            return _dataAPI.GetAllArchivedKPIs(month, year, id_kpi, globalrules);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetAllKpisByUserId")]
        public List<CatalogKpiDTO> GetAllKpisByUserId()
        {
            var user = HttpContext.User as AuthUser;
            var globalrules = _informationAPI.GetGlobalRulesByUserId(user.UserId);
            return _dataAPI.GetAllKpisByUserId(globalrules);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetRawDataByKpiID")]
        public List<ATDtDeDTO> GetRawDataByKpiID(string id_kpi, string month, string year)
        {
            return _dataAPI.GetRawDataByKpiID(id_kpi, month, year);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetArchivedRawDataByKpiID")]
        public List<ATDtDeDTO> GetArchivedRawDataByKpiID(string id_kpi, string month, string year)
        {
            return _dataAPI.GetArchivedRawDataByKpiID(id_kpi, month, year);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        /*      [HttpGet("GetDetailsArchivedKPI")]
                public List<ATDtDeDTO> GetDetailsArchivedKPIs(int idkpi, string month, string year)
                {
                    return _dataAPI.GetDetailsArchiveKPI(idkpi, month, year);
                }*/
        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetAllCustomersKP")]
        public List<KeyValuePair<int, string>> GetAllCustomersKP()
        {
            return _dataAPI.GetAllCustomersKP();
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetAllTRules")]
        public List<TRuleDTO> GetAllTRules()
        {
            return _dataAPI.GetAllTRules();
        }

        [Authorize(WorkFlowPermissions.VIEW_NOTIFIER_EMAILS)]
        [HttpGet("GetEmailNotifiers")]
        public List<EmailNotifierDTO> GetEmailNotifiers(string period)
        {
            return _dataAPI.GetEmailNotifiers(period);
        }

        /*       [HttpGet("GetRawIdsFromRulePeriod")]
               public List<int> GetRawIdsFromRulePeriod(int ruleId, string period)
               {
                   return _dataAPI.GetRawIdsFromRulePeriod(ruleId, period);
               }*/

        [HttpGet("AddArchiveRawData")]
        public bool AddArchiveRawData(int global_rule_id, string period, string tracking_period)
        {
            return _dataAPI.AddArchiveRawData(global_rule_id, period, tracking_period);
        }

        [HttpGet("GetEventResourceNames")]
        public List<EventResourceName> GetEventResourceNames()
        {
            return _dataAPI.GetEventResourceNames();
        }

        [HttpGet("GetDistributionByContract")]
        public DistributionPslDTO GetDistributionByContract(string period, int sla_id)
        {
            return _dataAPI.GetDistributionByContract(period, sla_id);
        }

        [Authorize(WorkFlowPermissions.VIEW_CONFIGURATION_STANDARD_DASHBOARD)]
        [HttpGet("GetAllUsersLandingPage")]
        public List<UserLandingPageLVDTO> GetAllUsersLandingPage()
        {
            return _dataAPI.GetAllUsersLandingPage();
        }

        [Authorize(WorkFlowPermissions.VIEW_CONFIGURATION_STANDARD_DASHBOARD)]
        [HttpGet("SetLandingPageByUser")]
        public void SetLandingPageByUser(int userId, bool set)
        {
            _dataAPI.SetLandingPageByUser(userId, set);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetLandingPageInformation")]
        public UserLandingPageDTO GetLandingPageInformation()
        {
            var usr = (HttpContext.User) as AuthUser;
            return _dataAPI.GetLandingPageInformation(usr.UserId);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("SelectLandingPage")]
        public void SelectLandingPage()
        {
            var usr = (HttpContext.User) as AuthUser;
            _dataAPI.SelectLandingPage(usr.UserId);
        }

        [Authorize(WorkFlowPermissions.VIEW_FREE_FORM_REPORT)]
        [HttpGet("GetOwnedReportQueries")]
        public List<ReportQueryLVDTO> GetOwnedReportQueries()
        {
            var usr = (HttpContext.User) as AuthUser;
            return _dataAPI.GetOwnedReportQueries(usr.UserId);
        }

        [Authorize(WorkFlowPermissions.VIEW_FREE_FORM_REPORT)]
        [HttpGet("GetAssignedReportQueries")]
        public List<ReportQueryLVDTO> GetAssignedReportQueries()
        {
            var usr = (HttpContext.User) as AuthUser;
            return _dataAPI.GetAssignedReportQueries(usr.UserId);
        }

        [Authorize(WorkFlowPermissions.VIEW_FREE_FORM_REPORT)]
        [HttpGet("GetReportQueryDetailByID")]
        public ReportQueryDetailDTO GetReportQueryDetailByID(int id)
        {
            var usr = (HttpContext.User) as AuthUser;
            return _dataAPI.GetReportQueryDetailByID(id, usr.UserId);
        }

        [Authorize(WorkFlowPermissions.VIEW_FREE_FORM_REPORT)]
        [HttpPost("AddEditReportQuery")]
        public void AddEditReportQuery([FromBody]ReportQueryDetailDTO dto)
        {
            var usr = (HttpContext.User) as AuthUser;

            _dataAPI.AddEditReportQuery(dto, (dto.OwnerId>0)?dto.OwnerId: usr.UserId);
        }

        [Authorize(WorkFlowPermissions.VIEW_FREE_FORM_REPORT)]
        [HttpGet("EnableDisableReportQuery")]
        public void EnableDisableReportQuery(int id, bool isenable)
        {
            var usr = (HttpContext.User) as AuthUser;
            _dataAPI.EnableDisableReportQuery(id, isenable, usr.UserId);
        }

        [Authorize(WorkFlowPermissions.VIEW_FREE_FORM_REPORT)]
        [HttpPost("AssignReportQuery")]
        public void AssignReportQuery([FromBody]MultipleRecordsDTO records)
        {
            var usr = (HttpContext.User) as AuthUser;
            _dataAPI.AssignReportQuery(records, usr.UserId);
        }

        [Authorize(WorkFlowPermissions.VIEW_FREE_FORM_REPORT)]
        [HttpGet("GetAllUsersAssignedQueries")]
        public List<UserReportQueryAssignmentDTO> GetAllUsersAssignedQueries(int queryid)
        {
            var usr = (HttpContext.User) as AuthUser;
            return _dataAPI.GetAllUsersAssignedQueries(queryid, usr.UserId);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpPost("ExecuteReportQuery")]
        public object ExecuteReportQuery([FromBody]ReportQueryDetailDTO dto)
        {
            var usr = (HttpContext.User) as AuthUser;
            return _dataAPI.ExecuteReportQuery(dto,usr.UserId);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpPost("CreateBooklet")]
        public int CreateBooklet([FromBody]CreateBookletDTO dto)
        {
            var usr = (HttpContext.User) as AuthUser;
            return _dataAPI.CreateBooklet(dto, usr.UserId);
        }
        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetCatalogEmailByUser")]
        public string GetCatalogEmailByUser()
        {
            var usr = (HttpContext.User) as AuthUser;
            return _dataAPI.GetCatalogEmailByUser(usr.UserId);
        }
        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetDayLevelKPIData")]
        public List<XYZDTO> GetDayLevelKPIData(int globalRuleId, int month, int year)
        {
            return _dataAPI.GetDayLevelKPIData(globalRuleId, month, year);
        }

        ////////////////////////////
        /*[HttpGet("GetFormsByUser")]
        public List<FormUsersDTO> GetFormsByUser()
        {
            var usr = HttpContext.User as AuthUser;
            if (usr != null)
            {
                bool isSecurityMember = _dataAPI.SecurityMembers(usr.UserId);
                var dtos = _dataAPI.GetAllFormUsers(0, usr.UserId);
                var attachmentCount = _dataAPI.GetFormDetials(dtos.Select(o => o.form_id).ToList());

                return (from d in dtos
                        join a in attachmentCount on d.form_id equals a.form_id
                        select new FormUsersDTO
                        {
                            form_id = d.form_id,
                            AttachmentsCount = a.attachment_count,
                            //create_date = d.create_date,
                            cutoff = d.cutoff,
                            form_description = d.form_description,
                            form_name = d.form_name,
                            form_owner_id = d.form_owner_id,
                            //modify_date = d.modify_date,
                            reader_configuration = d.reader_configuration,
                            reader_id = d.reader_id,
                            user_group_id = d.user_group_id,
                            user_group_name = d.user_group_name,
                            latest_input_date = a.latest_modified_date
                        }).ToList();

                //return dtos;
            }
            return null;
        }*/

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetForms")]
        public List<FormUsersDTO> GetForms(int id)
        {
            return _dataAPI.GetAllFormUsers(0, id);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetFormById/{id}")]
        public List<FormUsersDTO> GetFormById(int id)
        {
            return _dataAPI.GetAllFormUsers(id, 0);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetFormsByUser/{fakeUserID}/{type}")]
        public List<FormsFromCatalogDTO> GetFormsFromCatalog(int fakeUserID, string type)
        {
            var usr = HttpContext.User as AuthUser;
            if (usr != null)
            {
                bool isSecurityMember = _dataAPI.SecurityMembers(usr.UserId);
                return _dataAPI.GetFormsFromCatalog(usr.UserId, isSecurityMember, fakeUserID, type);
            }
            return null;
        }
        [HttpGet("GetLogs/{limit}")]
        public List<LogDTO> GetLogs(int limit)
        {
            return _dataAPI.GetLogs(limit);
        }
        [HttpGet("GetHeaders")]
        public IActionResult GetHeaders()
        {
            //var json = new { identity = HttpContext.User.Identity.Name, websocket = HttpContext.WebSockets.WebSocketRequestedProtocols, /*requestservice = HttpContext.RequestServices, features = HttpContext.Features,*/ request = HttpContext.Request.Headers, response = HttpContext.Response.Headers };
            //return Ok(json);

            var userid = HttpContext.Request.Headers["USERID"].ToString();
            if ( userid != "") { 
                var user = HttpContext.Request.Headers["USERID"][0].ToString();
                var data = _dataAPI.Login(user, "siteminderAccess");

                if (data != null)
                {
                    return Ok(data);
                }
                var json = new { error = "Errore durente il Login con " + user, description = "Username o Password errati.", request = user };
                return StatusCode(StatusCodes.Status403Forbidden, json);
            }
            else
            {
                var json = new { error = "Errore Siteminder", description = "Siteminder non inizializzato."};
                return StatusCode(StatusCodes.Status406NotAcceptable, json);
            }
        }
        [Authorize(WorkFlowPermissions.VIEW_REPORT_CUSTOM)]
        [HttpGet("GetPersonalReportsLV")]
        public List<PersonalReportLVDTO> GetPersonalReportsLV()
        {
            var usr = (HttpContext.User) as AuthUser;
            return _dataAPI.GetPersonalReportsLV(usr.UserId);

        }
        [Authorize(WorkFlowPermissions.VIEW_REPORT_CUSTOM)]
        [HttpGet("GetPersonalReportDetail")]
        public PersonalReportDTO GetPersonalReportDetail(int id)
        {
            return _dataAPI.GetPersonalReportDetail(id);
        }
        [Authorize(WorkFlowPermissions.VIEW_REPORT_CUSTOM)]
        [HttpPost("AddUpdatePersonalReport")]
        public void AddUpdatePersonalReport([FromBody]PersonalReportDTO dto)
        {
            var usr = (HttpContext.User) as AuthUser;
            _dataAPI.AddUpdatePersonalReport(dto,usr.UserId);
        }
        [Authorize(WorkFlowPermissions.VIEW_REPORT_CUSTOM)]
        [HttpGet("DeletePersonalReport")]
        public void DeletePersonalReport(int id)
        {
            _dataAPI.DeletePersonalReport(id);
        }



    }
}