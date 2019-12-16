using Quantis.WorkFlow.Services.DTOs.Information;
using System;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.API
{
    public interface IInformationService
    {
        List<ConfigurationDTO> GetAllAdvancedConfigurations();

        List<ConfigurationDTO> GetAllBasicConfigurations();

        void DeleteConfiguration(string owner, string key);

        void AddUpdateBasicConfiguration(ConfigurationDTO dto);

        void AddUpdateAdvancedConfiguration(ConfigurationDTO dto);

        ConfigurationDTO GetConfiguration(string owner, string key);
        List<int> GetContractIdsByUserId(int userId);
        void AddUpdateRole(BaseNameCodeDTO dto);

        void DeleteRole(int roleId);

        List<BaseNameCodeDTO> GetAllRoles();

        List<PermissionDTO> GetAllPermissions();

        List<BaseNameCodeDTO> GetRolesByUserId(int userid);

        List<PermissionDTO> GetPermissionsByUserId(int userid);

        List<PermissionDTO> GetPermissionsByRoleID(int roleId);

        void AssignRolesToUser(MultipleRecordsDTO dto);

        void AssignPermissionsToRoles(MultipleRecordsDTO dto);

        List<SDMStatusDTO> GetAllSDMStatusConfigurations();

        List<SDMGroupDTO> GetAllSDMGroupConfigurations();

        void DeleteSDMGroupConfiguration(int id);

        void DeleteSDMStatusConfiguration(int id);

        void AddUpdateSDMStatusConfiguration(SDMStatusDTO dto);

        void AddUpdateSDMGroupConfiguration(SDMGroupDTO dto);

        List<int> GetGlobalRulesByUserId(int userId);

        void AssignGlobalRulesToUserId(MultipleRecordsDTO dto);

        List<ContractPartyDetailDTO> GetContractPartyByUser(int userId);

        List<BaseNameCodeDTO> GetAllContractPariesByUserId(int userId);

        List<BaseNameCodeDTO> GetAllContractsByUserId(int userId, int contractpartyId);

        List<BaseNameCodeDTO> GetAllKpisByUserId(int userId, int contractId);

        void AssignKpisToUserByContractParty(int userId, int contractpartyId, bool assign);

        void AssignKpisToUserByContract(int userId, int contractId, bool assign);

        void AssignKpisToUserByKpis(int userId, int contractId, List<int> kpiIds);

        int GetContractIdByGlobalRuleId(int globalruleid);

        List<UserProfilingDTO> GetUserProfilingCSV();

        void AddUpdateUserSettings(int userId, string key, string value);
        string GetUserSetting(int userId, string key);
        List<KeyValuePair<string, string>> GetAllUserSettings(int userId);
        List<ContractPartyContractDTO> GetAllContractPartiesContracts();
        void UploadFileToSFTPServer(BaseFileDTO fileDTO);
        void AssignCuttoffWorkflowDayByContractId(int contractId, int daycuttoff, int workflowday);
        void AssignCuttoffWorkflowDayByContractIdAndOrganization(int contractId,string organizationunit, int daycuttoff, int workflowday);
        List<ContractPartyContractDTO> GetContractsWithContractParties(int userId);
        List<ContractPartyContractDTO> GetContractsByContractParty(int contractPartyId, int userId);
        List<OrganizationUnitDTO> GetWorkflowByContract(int contractId);
        
        List<OrganizationUnitDTO> GetOrganizationUnits();
        List<OrganizationUnitDTO> GetOrganizationUnitsByContract(int contractid);
        List<UserKPIDTO> GetKPIDetails(List<int> KpiIds);

        List<KeyValuePair<int, string>> GetAllOrganizationUnits();
        bool DeleteOrganizationUnit(int id);
        bool AddUpdateOrganizationUnit(KeyValuePair<int, string> dto);
        List<ReportSpecialValueDTO> GetAllReportSpecialValues();
        void DeleteReportSpecialValue(int key);
        void AddUpdateReportSpecialValue(ReportSpecialValueDTO dto);
    }
}