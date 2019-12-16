using Quantis.WorkFlow.Services.DTOs.API;
using Quantis.WorkFlow.Services.DTOs.BusinessLogic;
using Quantis.WorkFlow.Services.DTOs.Information;
using Quantis.WorkFlow.Services.DTOs.Widgets;
using Quantis.WorkFlow.Services.Framework;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.API
{
    public interface IDataService
    {
        bool CronJobsScheduler();

        string GetBSIServerURL();

        List<WidgetDTO> GetAllWidgets();
        int CreateBooklet(CreateBookletDTO dto, int userId);

        WidgetDTO GetWidgetById(int Id);

        bool AddUpdateWidget(WidgetDTO dto);
        string GetCatalogEmailByUser(int userId);

        List<FormLVDTO> GetAllForms();

        List<FormUsersDTO> GetAllFormUsers(int formId, int userId);
        List<XYZDTO> GetDayLevelKPIData(int globalRuleId, int month, int year);
        List<FormsFromCatalogDTO> GetFormsFromCatalog(int userid, bool isSecurityMember, int fakeUserID, string type);
        List<LogDTO> GetLogs(int limit);

        bool SecurityMembers(int userId);

        List<KeyValuePair<int, string>> GetAllCustomersKP();

        List<UserDTO> GetAllUsers();

        //        List<int> GetRawIdsFromRulePeriod(int ruleId, string period);
        UserDTO GetUserById(string UserId);

        bool AddUpdateUser(UserDTO dto);

        PagedList<UserDTO> GetAllPagedUsers(UserFilterDTO filter);

        List<NotifierLogDTO> GetEmailHistory();

        List<FormDetialsDTO> GetFormDetials(List<int> formids);

        List<PageDTO> GetAllPages();

        PageDTO GetPageById(int Id);

        bool AddUpdatePage(PageDTO dto);

        List<FormAttachmentDTO> GetAttachmentsByFormId(int formId);

        List<GroupDTO> GetAllGroups();

        GroupDTO GetGroupById(int Id);

        bool AddUpdateGroup(GroupDTO dto);

        List<KPISDMExtraDTO> GetKPISDMExtraInformation(List<int> ids);

        List<TUserDTO> GetAllTUsers();

        List<TRuleDTO> GetAllTRules();

        List<EmailNotifierDTO> GetEmailNotifiers(string period);

        List<CatalogKpiDTO> GetAllKpis(); //List<CatalogKPILVDTO> GetAllKpis();

        List<CatalogKpiDTO> GetAllKpisByUserId(List<int> globalruleIds);

        CatalogKpiDTO GetKpiById(int Id);
        CatalogKpiDTO GetKpiByGlobalRuleId(int global_rule_id);
        bool AddUpdateKpi(CatalogKpiDTO dto);

        List<KPIOnlyContractDTO> GetKpiByFormId(int Id);

        List<ApiDetailsDTO> GetAllAPIs();

        FormRuleDTO GetFormRuleByFormId(int Id);

        bool AddUpdateFormRule(FormRuleDTO dto);

        bool RemoveAttachment(int Id);

        void AddArchiveKPI(ARulesDTO dto);

        LoginResultDTO Login(string username, string password);

        void Logout(string token);

        bool SumbitForm(SubmitFormDTO dto);

        bool SubmitAttachment(List<FormAttachmentDTO> dto);

        int ArchiveKPIs(ArchiveKPIDTO dto);

        bool AddArchiveRawData(int global_rule_id, string period, string tracking_period);

        bool ResetPassword(string username, string email);

        List<UserDTO> GetUsersByRoleId(int roleId);

        List<ARulesDTO> GetAllArchivedKPIs(string month, string year, string id_kpi, List<int> globalruleIds);

        //        List<ATDtDeDTO> GetDetailsArchiveKPI(int idkpi, string month, string year);

        List<ATDtDeDTO> GetRawDataByKpiID(string id_kpi, string month, string year);

        List<ATDtDeDTO> GetArchivedRawDataByKpiID(string id_kpi, string month, string year);

        string GetUserIdByUserName(string name);

        CreateTicketDTO GetKPICredentialToCreateTicket(int Id);

        List<EventResourceName> GetEventResourceNames();

        List<FormAttachmentDTO> GetAttachmentsByKPIID(int kpiId);

        DistributionPslDTO GetDistributionByContract(string period, int sla_id);

        List<UserLandingPageLVDTO> GetAllUsersLandingPage();

        void SetLandingPageByUser(int userId, bool set);

        UserLandingPageDTO GetLandingPageInformation(int userId);

        void SelectLandingPage(int userId);

        List<ReportQueryLVDTO> GetOwnedReportQueries(int userId);

        List<ReportQueryLVDTO> GetAssignedReportQueries(int userId);

        ReportQueryDetailDTO GetReportQueryDetailByID(int id, int userId);

        void AddEditReportQuery(ReportQueryDetailDTO dto, int userId);

        void EnableDisableReportQuery(int id, bool isenable, int userId);

        void AssignReportQuery(MultipleRecordsDTO records, int ownerId);

        List<UserReportQueryAssignmentDTO> GetAllUsersAssignedQueries(int queryid, int userId);

        object ExecuteReportQuery(ReportQueryDetailDTO dto, int userId);
        List<PersonalReportLVDTO> GetPersonalReportsLV(int userId);
        PersonalReportDTO GetPersonalReportDetail(int id);
        void AddUpdatePersonalReport(PersonalReportDTO dto, int userId);
        void DeletePersonalReport(int id);

    }
}