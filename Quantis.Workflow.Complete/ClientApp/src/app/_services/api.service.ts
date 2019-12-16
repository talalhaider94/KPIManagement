import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import Headers from '../_helpers/headers';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ApiService {
    constructor(private http: HttpClient) { }

    getBooklet(): Observable<any> {
        const getBookletEndPoint = `${environment.API_URL}/Oracle/GetBooklets`;
        return this.http.get(getBookletEndPoint, Headers.setHeaders('GET'));
    }
    getEventResourceNames(): Observable<any> {
        const getEventResourceNamesEndPoint = `${environment.API_URL}/Data/GetEventResourceNames`;
        return this.http.get(getEventResourceNamesEndPoint);
    }
    getCatalogoUsers(): Observable<any> {
        const getUtentiEndPoint = `${environment.API_URL}/data/GetAllUsers`;
        return this.http.get(getUtentiEndPoint);
    }
    getTUsers(): Observable<any> {
        const getTUsersEndPoint = `${environment.API_URL}/data/GetAllTUsers`;
        return this.http.get(getTUsersEndPoint);
    }
    getTRules(): Observable<any> {
        const getTRulesEndPoint = `${environment.API_URL}/data/GetAllTRules`;
        return this.http.get(getTRulesEndPoint);
    }

    getKpiArchivedRawData(id, month, year): Observable<any> {
        const getDateKpiId = `${environment.API_URL}/data/GetArchivedRawDataByKpiID?id_kpi=${id}&month=${month}&year=${year}`;
        return this.http.get(getDateKpiId);
    }
    getKpiRawData(id, month, year): Observable<any> {
        const getDateKpiId = `${environment.API_URL}/data/GetRawDataByKpiID?id_kpi=${id}&month=${month}&year=${year}`;
        return this.http.get(getDateKpiId);
    }
    getCatalogoKpis(): Observable<any> {
        const getKpiEndPoint = `${environment.API_URL}/data/GetAllKpis`;
        return this.http.get(getKpiEndPoint);
    }
    getCatalogoKpisByUserId(): Observable<any> {
        const getKpiEndPoint = `${environment.API_URL}/data/GetAllKpisByUserId`;
        return this.http.get(getKpiEndPoint);
    }
    getConfigurations(): Observable<any> {
        const getConfigurationsEndPoint = `${environment.API_URL}/Information/GetAllBasicConfigurations`;
        return this.http.get(getConfigurationsEndPoint);
    }

    getEmails(month, year): Observable<any> {
        const getEmailsEndPoint = `${environment.API_URL}/data/GetEmailNotifiers?period=${month}/${year}`;
        return this.http.get(getEmailsEndPoint);
    }

    getAdvancedConfigurations(): Observable<any> {
        const getAdvancedConfigurationsEndPoint = `${environment.API_URL}/Information/GetAllAdvancedConfigurations`;
        return this.http.get(getAdvancedConfigurationsEndPoint);
    }

    getSDMGroupConfigurations(): Observable<any> {
        const getSDMGroupConfigurationsEndPoint = `${environment.API_URL}/Information/GetAllSDMGroupConfigurations`;
        return this.http.get(getSDMGroupConfigurationsEndPoint);
    }

    getSDMStatusConfigurations(): Observable<any> {
        const getSDMStatusConfigurationsEndPoint = `${environment.API_URL}/Information/GetAllSDMStatusConfigurations`;
        return this.http.get(getSDMStatusConfigurationsEndPoint);
    }

    getArchivedKpis(month, year): Observable<any> {
        const getArchivedKpisEndPoint = `${environment.API_URL}/data/getallarchivedkpis?month=${month}&year=${year}`;
        return this.http.get(getArchivedKpisEndPoint);
    }

    getArchivedKpiById(id): Observable<any> {
        const getArchivedKpisEndPoint = `${environment.API_URL}/data/getallarchivedkpis?id_kpi=${id}`;
        return this.http.get(getArchivedKpisEndPoint);
    }
    getCatalogEmailByUser(): Observable<any> {
        const getValidEmail = `${environment.API_URL}/data/GetCatalogEmailByUser`
        
        return this.http.get(getValidEmail);
    }
    deleteSDMGroupConfiguration(id): Observable<any> {
        const deleteSDMGroupConfiguration = `${environment.API_URL}/information/DeleteSDMGroupConfiguration/${id}`;
        return this.http.get(deleteSDMGroupConfiguration);
    }

    deleteSDMStatusConfiguration(id): Observable<any> {
        const deleteSDMStatusConfiguration = `${environment.API_URL}/information/DeleteSDMStatusConfiguration/${id}`;
        return this.http.get(deleteSDMStatusConfiguration);
    }

    getDataKpis(month, year): Observable<any> {
        const getDataKpisEndPoint = `${environment.API_URL}/data/getallarchivedkpis?month=${month}&year=${year}`;
        return this.http.get(getDataKpisEndPoint);
    }

    getUsersByRole(roleId): Observable<any> {
        const getUsersByRole = `${environment.API_URL}/data/GetUsersByRoleId?roleId=${roleId}`;
        return this.http.get(getUsersByRole);
    }

    getCustomersKP(): Observable<any> {
        const getCustomersKP = `${environment.API_URL}/data/GetAllCustomersKP`;
        return this.http.get(getCustomersKP);
    }

    getAllRoles(): Observable<any> {
        const getAllRolesEndPoint = `${environment.API_URL}/information/GetAllRoles`;
        return this.http.get(getAllRolesEndPoint);
    }
    getRolesByUserId(userid): Observable<any> {
        const getRolesByUserIdEndPoint = `${environment.API_URL}/information/GetRolesByUserId/?userid=${userid}`;
        return this.http.get(getRolesByUserIdEndPoint);
    }
    getLandingPage(month, year): Observable<any> {
        const getLandingPageEndPoint = `${environment.API_URL}/oracle/GetLandingPageByUser?period=${month}/${year}`;
        return this.http.get(getLandingPageEndPoint);
    }
    getLandingPageLevel1(contractpartyid,month,year): Observable<any> {
        const getLandingPageLevel1EndPoint = `${environment.API_URL}/oracle/getLandingPageLevel1?contractPartyId=${contractpartyid}&period=${month}/${year}`;
        return this.http.get(getLandingPageLevel1EndPoint);
    }
    getTestLandingPage(): Observable<any> {
        const getLandingPageEndPoint = `${environment.API_URL}/data/GetAllUsersLandingPage`;
        return this.http.get(getLandingPageEndPoint);
    }
    getThresholdDetails(key): Observable<any> {
        const getThresholdEndPoint = `${environment.API_URL}/information/GetUserSetting?key=${key}`;
        return this.http.get(getThresholdEndPoint);
    }
    GetAllContractPartiesContracts(): Observable<any> {
        const getcpcEndPoint = `${environment.API_URL}/information/GetAllContractPartiesContracts`;
        return this.http.get(getcpcEndPoint);
    }
    GetLandingPageKPIDetails(contractPartyId, month, year): Observable<any> {
        const getkpidetailsEndPoint = `${environment.API_URL}/oracle/GetLandingPageKPIDetails?contractPartyId=${contractPartyId}&period=${month}/${year}`;
        return this.http.get(getkpidetailsEndPoint);
    }
    GetDayLevelKPIData(globalRuleId, month, year): Observable<any> {
        const getDayLevelEndPoint = `${environment.API_URL}/data/GetDayLevelKPIData?globalRuleId=${globalRuleId}&month=${month}&year=${year}`;
        return this.http.get(getDayLevelEndPoint);
    }


    AddUpdateUserSettings(key,value): Observable<any> {
        const setThresholdEndPoint = `${environment.API_URL}/information/AddUpdateUserSettings?key=${key}&value=${value}`;
        return this.http.get(setThresholdEndPoint);
    }
    AssignCuttoffWorkflowDayByContractId(contractId,daycuttoff,workflowday): Observable<any> {
        const setCutoffEndPoint = `${environment.API_URL}/information/AssignCuttoffWorkflowDayByContractId?contractId=${contractId}&daycuttoff=${daycuttoff}&workflowday=${workflowday}`;
        return this.http.get(setCutoffEndPoint);
    }

    addRole(data): Observable<any> {
        const addrole = `${environment.API_URL}/information/AddUpdateRole`;
        return this.http.post(addrole, data);
    }

    addSDMGroup(data): Observable<any> {
        const addSDMGroup = `${environment.API_URL}/information/AddUpdateSDMGroupConfiguration`;
        return this.http.post(addSDMGroup, data);
    }

    addSDMStatus(data): Observable<any> {
        const addSDMStatus = `${environment.API_URL}/information/AddUpdateSDMStatusConfiguration`;
        return this.http.post(addSDMStatus, data);
    }

    addConfig(data): Observable<any> {
        const addConfig = `${environment.API_URL}/information/AddUpdateBasicConfiguration`;
        return this.http.post(addConfig, data);
    }

    addAdvancedConfig(data): Observable<any> {
        const addConfig = `${environment.API_URL}/information/AddUpdateAdvancedConfiguration`;
        return this.http.post(addConfig, data);
    }
    
    CreateBooklet(data): Observable<any> {
        const booklet = `${environment.API_URL}/data/CreateBooklet`;
        return this.http.post(booklet, data);
    }
    GetPersonalReport(PersonalReportFilterDTO): Observable<any> {
        const personal_report = `${environment.API_URL}/oracle/GetPersonalReport`;
        return this.http.post(personal_report, PersonalReportFilterDTO);
    }

    deleteRole(roleId): Observable<any> {
        const deleteroles = `${environment.API_URL}/information/DeleteRole/${roleId}`;
        return this.http.get(deleteroles);
    }

    getAllPermisisons(): Observable<any> {
        const getAllPermisisonsEndPoint = `${environment.API_URL}/information/GetAllPermissions`;
        return this.http.get(getAllPermisisonsEndPoint);
    }
    getPermissionsByRoldId(roleId): Observable<any> {
        const getPermissionsByRoldIdEndPoint = `${environment.API_URL}/information/GetPermissionsByRoleID/?roleId=${roleId}`;
        return this.http.get(getPermissionsByRoldIdEndPoint);
    }

    getAllKpiHierarchy(userId): Observable<any> {
        const getAllKpiHierarchyEndPoint = `${environment.API_URL}/information/GetAllContractPariesByUserId?userId=${userId}`;
        return this.http.get(getAllKpiHierarchyEndPoint);
    }
    getGlobalRulesByUserId(userId): Observable<any> {
        const getGlobalRulesByRoleIdEndPoint = `${environment.API_URL}/information/GetGlobalRulesByUserId/?userId=${userId}`;
        return this.http.get(getGlobalRulesByRoleIdEndPoint);
    }
    GetContractParties(): Observable<any> {
      const gteContractPartiesEndPoint = `${environment.API_URL}/information/GetContractParties`;
      return this.http.get(gteContractPartiesEndPoint);
    }
    GetContractsByContractParty(contractPartyId): Observable<any> {
      const gteContractPartiesEndPoint = `${environment.API_URL}/information/GetContractsByContractParty?contractPartyId=${contractPartyId}`;
      return this.http.get(gteContractPartiesEndPoint);
    }
    getContracts(userId, contractPartyId): Observable<any> {
        const getContractsEndPoint = `${environment.API_URL}/information/GetAllContractsByUserId?userId=${userId}&contractpartyId=${contractPartyId}`;
        return this.http.get(getContractsEndPoint);
    }
    getKpis(userId, contractId): Observable<any> {
        const getKpisEndPoint = `${environment.API_URL}/information/GetAllKpisByUserId?userId=${userId}&contractId=${contractId}`;
        return this.http.get(getKpisEndPoint);
    }
    getSeconds(): Observable<any> {
        const getSecondsEndPoint = `${environment.API_URL}/information/GetDashboardTickInterval`;
        return this.http.get(getSecondsEndPoint);
    }
    getUserProfilingCSV(): Observable<any> {
        const getCSVEndPoint = `${environment.API_URL}/information/GetUserProfilingCSV`;
        return this.http.get(getCSVEndPoint);
    }
    getPersonalReports(): Observable<any> {
        const getPREndPoint = `${environment.API_URL}/data/GetPersonalReportsLV`;
        return this.http.get(getPREndPoint);
    }
    editPersonalReport(id): Observable<any> {
        const editPREndPoint = `${environment.API_URL}/data/GetPersonalReportDetail?id=${id}`;
        return this.http.get(editPREndPoint);
    }
    getAllNormalReports(): Observable<any> {
        const getPREndPoint = `${environment.API_URL}/bsi/GetAllNormalReports`;
        return this.http.get(getPREndPoint);
    }
    getReportDetails(reportId): Observable<any> {
        const reportIdEndPoint = `${environment.API_URL}/bsi/GetReportDetail?ReportId=${reportId}`;
        return this.http.get(reportIdEndPoint);
    }
    getFreeFormReports(): Observable<any> {
        const getFFrREndPoint = `${environment.API_URL}/oracle/GetBSIFreeFormReports`;
        return this.http.get(getFFrREndPoint);
    }
  getKpiByGlobalRuleId(global_rule_id): Observable<any> {
    const getKpiEndPoint = `${environment.API_URL}/data/getKpiByGlobalRuleId/${global_rule_id}`;
    return this.http.get(getKpiEndPoint);
  }
    updateConfig(config) {
        return this.http.post(`${environment.API_URL}/information/AddUpdateBasicConfiguration`, config)
            .pipe(
                tap(
                    data => data,
                    error => error
                )
            );
    }

    updateAdvancedConfig(config) {
        return this.http.post(`${environment.API_URL}/information/AddUpdateAdvancedConfiguration`, config)
            .pipe(
                tap(
                    data => data,
                    error => error
                )
            );
    }

    updateRole(data) {
        return this.http.post(`${environment.API_URL}/information/AddUpdateRole`, data)
            .pipe(
                tap(
                    data => data,
                    error => error
                )
            );
    }

    updateSDMGroupConfig(config) {
        return this.http.post(`${environment.API_URL}/information/AddUpdateSDMGroupConfiguration`, config)
            .pipe(
                tap(
                    data => data,
                    error => error
                )
            );
    }

    updateSDMStatusConfig(config) {
        return this.http.post(`${environment.API_URL}/information/AddUpdateSDMStatusConfiguration`, config)
            .pipe(
                tap(
                    data => data,
                    error => error
                )
            );
    }

    updateCatalogUtenti(config) {
        return this.http.post(`${environment.API_URL}/data/AddUpdateUser`, config)
            .pipe(
                tap(
                    data => data,
                    error => error
                )
            );
    }

    updateCatalogKpi(config) {
        return this.http.post(`${environment.API_URL}/data/AddUpdateKpi`, config)
            .pipe(
                tap(
                    data => data,
                    error => error
                )
            );
    }

    assignRolesToUser(postData) {
        return this.http.post(`${environment.API_URL}/information/AssignRolesToUser`, postData)
            .pipe(
                tap(
                    data => data,
                    error => error
                )
            );
    }

    assignPermissionsToRoles(postData) {
        return this.http.post(`${environment.API_URL}/information/AssignPermissionsToRoles`, postData)
            .pipe(
                tap(
                    data => data,
                    error => error
                )
            );
    }

    assignContractParty(userId, contractpartyId): Observable<any> {
        const assignContractPartyEndPoint = `${environment.API_URL}/information/AssignKpisToUserByContractParty?userId=${userId}&contractpartyId=${contractpartyId}&assign=true`;
        return this.http.get(assignContractPartyEndPoint);
    }

    unassignContractParty(userId, contractpartyId): Observable<any> {
        const assignContractPartyEndPoint = `${environment.API_URL}/information/AssignKpisToUserByContractParty?userId=${userId}&contractpartyId=${contractpartyId}&assign=false`;
        return this.http.get(assignContractPartyEndPoint);
    }

    assignContracts(userId, contractId): Observable<any> {
        const assignContractsEndPoint = `${environment.API_URL}/information/AssignKpisToUserByContract?userId=${userId}&contractId=${contractId}&assign=true`;
        return this.http.get(assignContractsEndPoint);
    }

    unassignContracts(userId, contractId): Observable<any> {
        const assignContractsEndPoint = `${environment.API_URL}/information/AssignKpisToUserByContract?userId=${userId}&contractId=${contractId}&assign=false`;
        return this.http.get(assignContractsEndPoint);
    }

    assignKpistoUser(postData): Observable<any> {
        const assignKpisEndPoint = `${environment.API_URL}/information/AssignKpisToUserByKpis`;
        return this.http.post(assignKpisEndPoint, postData);
    }

    assignGlobalRulesToUserId(postData) {
        return this.http.post(`${environment.API_URL}/information/AssignGlobalRulesToUserId`, postData
        )
            .pipe(
                tap(
                    data => data,
                    error => error
                )
            );
    }

    setLandingPagePermission(userId, set): Observable<any> {
        const lpPermissionsEndPoint = `${environment.API_URL}/data/SetLandingPageByUser?userId=${userId}&set=${set}`;
        return this.http.get(lpPermissionsEndPoint);
    }

    getContractWithContractParties():Observable<any> {
        const getContract = `${environment.API_URL}/information/GetContractsWithContractParties`;
        return this.http.get(getContract);
    }
    GetAllOrganizationUnits(): Observable<any> {
        const getOrganizations = `${environment.API_URL}/information/GetAllOrganizationUnits`;
        return this.http.get(getOrganizations);
    }
    GetAllReportSpecialValues(): Observable<any> {
        const getOrganizations = `${environment.API_URL}/information/GetAllReportSpecialValues`;
        return this.http.get(getOrganizations);
    }
    GetAllMonitorings(month,year): Observable<any> {
        const getMonitoring = `${environment.API_URL}/monitoring/GetTicketsMonitoringByPeriod?period=${month}/${year}`;
        return this.http.get(getMonitoring);
    }
    GetMonitoringDay(month, year): Observable<any> {
        const getDayLevelMonitoring = `${environment.API_URL}/monitoring/GetDayLevelTicketsMonitoring?period=${month}/${year}`;
        return this.http.get(getDayLevelMonitoring);
    }
    GetBookletsLV(): Observable<any> {
        const GetBookletsLV = `${environment.API_URL}/oracle/GetBookletsLV`;
        return this.http.get(GetBookletsLV);
    }
    DeleteOrganizationUnit(id): Observable<any> {
        const DeleteOrganizationUnit = `${environment.API_URL}/information/DeleteOrganizationUnit?id=${id}`;
        return this.http.get(DeleteOrganizationUnit);
    }
    DeleteReportSpecialValue(id): Observable<any> {
        const DeleteOrganizationUnit = `${environment.API_URL}/information/DeleteReportSpecialValue?key=${id}`;
        return this.http.get(DeleteOrganizationUnit);
    }
    GetOrganizationUnits(): Observable<any> {
      const getOrganization = `${environment.API_URL}/information/GetOrganizationUnits`;
      return this.http.get(getOrganization);
    }
    GetOrganizationUnitsByContract(contractid): Observable<any> {
        const getOrganization = `${environment.API_URL}/information/GetOrganizationUnitsByContract?contractid=${contractid}`;
        return this.http.get(getOrganization);
    }
    GetWorkflowByContract(contractid): Observable<any> {
      const getOrganization = `${environment.API_URL}/information/GetWorkflowByContract?contractid=${contractid}`;
      return this.http.get(getOrganization);
    }
    AssignCuttoffWorkflowDayByContractIdAndOrganization(contractid, organizationunit, daycuttoff, workflowday): Observable<any> {
        const AssignWorkflowEndPoint = `${environment.API_URL}/information/AssignCuttoffWorkflowDayByContractIdAndOrganization?contractid=${contractid}&organizationunit=${organizationunit}&daycuttoff=${daycuttoff}&workflowday=${workflowday}`;
        return this.http.get(AssignWorkflowEndPoint);
    }
    AddUpdatePersonalReport(obj) {
        return this.http.post(`${environment.API_URL}/data/AddUpdatePersonalReport`, obj);
    }
    DeletePersonalReport(id): Observable<any> {
        const DeletePersonalReportEP = `${environment.API_URL}/data/DeletePersonalReport?id=${id}`;
        return this.http.get(DeletePersonalReportEP);
    }
    AddUpdateOrganizationUnit(obj) {
        return this.http.post(`${environment.API_URL}/information/AddUpdateOrganizationUnit`, obj);
    }
    AddUpdateReportSpecialValue(obj) {
        return this.http.post(`${environment.API_URL}/information/AddUpdateReportSpecialValue`, obj);
    }
}
