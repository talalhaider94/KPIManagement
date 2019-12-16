using Quantis.WorkFlow.Services.DTOs.API;
using Quantis.WorkFlow.Services.DTOs.OracleAPI;
using System;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.API
{
    public interface IOracleDataService
    {
        List<OracleCustomerDTO> GetCustomer(int id, string name);

        //List<OracleFormDTO> GetForm(int id, int userid);
        List<OracleGroupDTO> GetGroup(int id, string name);

        List<OracleSlaDTO> GetSla(int id, string name);

        List<OracleRuleDTO> GetRule(int id, string name);

        List<OracleUserDTO> GetUser(int id, string name);
        List<ReportPersonalDTO> GetPersonalReport(PersonalReportFilterDTO filter);

        //List<PslDTO> GetPsl(string period, string sla_name, string rule_name, string tracking_period);
        List<PslDTO> GetPsl(string period, int global_rule_id, string tracking_period);

        Tuple<int, int> GetUserIdLocaleIdByUserName(string username);

        List<OracleBookletDTO> GetBooklets();
        List<BookletLVDTO> GetBookletsLV(int userId);
        List<BSIFreeFormReportDTO> GetBSIFreeFormReports();

        List<LandingPageDTO> GetLandingPageByUser(int userId, string period);

        List<LandingPageLevel1DTO> GetLandingPageLevel1(int userId, int contractPartyId, string period);
        List<LandingPageBaseDTO> GetLandingPageKPIDetails(int userId, int contractPartyId, string period);
        LandingPageLevel1DTO GetLandingPageLevel1ByContract(int userId, int contractId, string period);
    }
}