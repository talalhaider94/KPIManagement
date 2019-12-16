namespace Quantis.WorkFlow.Services
{
    public static class WorkFlowConstants
    {
        public static string Configuration_SessionTimeOutKey = "";
        //t.interval_length || ' ' || initcap(t.time_unit) ||
        //decode(t.is_period, 1, ' (Rule''s tracking period)', '') ""Tracking Period"" ,
        public static string KPI_Calculation_Status_Query = @"select * from (
        select
        t.customer_name  ""Cliente"",
        t.sla_name ""Contratto"" ,
        t.rule_name ""KPI"" ,
        to_char(to_date(decode (to_char (t.last_psl_record_date , 'dd/mm/yyyy') , '02/01/1970' , null , t.last_psl_record_date)),'dd/mm/yyyy HH:mm:ss') ""Aggiornato al"" 
        from
        (
            select cu.customer_name,
            s.customer_id,
            s.sla_name ,
            r.rule_name             ,
            t.interval_length       ,
            t.time_unit             ,
            t.is_period             ,
            t.last_psl_record_date  ,
            t.rule_tu_modify_date   ,
            t.last_psl_cycle_date   ,
            t.min_time_not_used     ,
            t.min_time_of_exception ,
            t.min_time_of_version   ,
            g.psl_instance_id       ,
            least (nvl (t.min_time_not_used, to_date ('18/01/2038' ,'dd/mm/yyyy'))           ,
                nvl(t.min_time_of_version   , to_date ('18/01/2038' ,'dd/mm/yyyy'))           ,
                nvl(t.min_time_of_exception , to_date ('18/01/2038' ,'dd/mm/yyyy'))) min_time
            from oblicore.t_rules_time_units t,
        oblicore.t_rules r,
        oblicore.t_sla_versions sv,
        oblicore.t_slas s,
        oblicore.t_global_rules g,
        oblicore.t_customers cu
        where s.sla_id          = sv.sla_id
        and sv.sla_version_id = r.sla_version_id
        and r.psl_rule_id     = t.rule_id
        and sv.status         in ('EFFECTIVE')
        and s.sla_status      not in ('ARCHIVED','PURGING')
        and t.status          = 'ON'
        and t.is_period = 1
        and r.measurability_status = 'TXT_MEASURABILITY_STATUS_ACTIVE'
        and r.global_rule_id  = g.global_rule_id
        and {0}
        and s.CUSTOMER_ID=cu.CUSTOMER_ID) t order by t.min_time , t.rule_tu_modify_date)
        order by 4 DESC";


    }

    public static class WorkFlowPermissions
    {
        public const string BASIC_LOGIN = "BASIC_LOGIN";
        public const string VIEW_WORKFLOW_KPI_VERIFICA = "VIEW_WORKFLOW_KPI_VERIFICA";
        public const string VIEW_WORKFLOW_RICERCA = "VIEW_WORKFLOW_RICERCA";
        public const string VIEW_CATALOG_UTENTI = "VIEW_CATALOG_UTENTI";
        public const string VIEW_CATALOG_KPI = "VIEW_CATALOG_KPI";
        public const string EDIT_CATALOG_KPI = "EDIT_CATALOG_KPI";
        public const string VIEW_UTENTI_DA_CONSOLIDARE = "VIEW_UTENTI_DA_CONSOLIDARE";
        public const string VIEW_KPI_DA_CONSOLIDARE = "VIEW_KPI_DA_CONSOLIDARE";
        public const string VIEW_KPI_CERTICATI = "VIEW_KPI_CERTICATI";
        public const string VIEW_ADMIN_LOADING_FORM = "VIEW_ADMIN_LOADING_FORM";
        public const string VIEW_LOADING_FORM_UTENTI = "VIEW_LOADING_FORM_UTENTI";
        public const string VIEW_CONFIGURATION_GENERAL = "VIEW_CONFIGURATION_GENERAL";
        public const string VIEW_CONFIGURATION_ROLES = "VIEW_CONFIGURATION_ROLES";
        public const string VIEW_CONFIGURATION_USER_ROLES = "VIEW_CONFIGURATION_USER_ROLES";
        public const string VIEW_CONFIGURATION_USER_PROFILING = "VIEW_CONFIGURATION_USER_PROFILING";
        public const string VIEW_CONFIGURATION_ADVANCED = "VIEW_CONFIGURATION_ADVANCED";
        public const string VIEW_CONFIGURATION_SDM_GROUP = "VIEW_CONFIGURATION_SDM_GROUP";
        public const string VIEW_CONFIGURATION_SDM_TICKET_STATUS = "VIEW_CONFIGURATION_SDM_TICKET_STATUS";
        public const string VIEW_LINK_BSI = "VIEW_LINK_BSI";
        public const string VIEW_NOTIFIER_EMAILS = "VIEW_NOTIFIER_EMAILS";
        public const string VIEW_RAW_DATA = "VIEW_RAW_DATA";
        public const string VIEW_BOOKLET_FROM_TEMPLATE = "VIEW_BOOKLET_FROM_TEMPLATE";
        public const string VIEW_BOOKLET = "VIEW_BOOKLET";
        public const string VIEW_WORKFLOW_ADMIN = "VIEW_WORKFLOW_ADMIN";
        public const string VIEW_DEBUG = "VIEW_DEBUG";
        public const string VIEW_CONFIGURATION_STANDARD_DASHBOARD = "VIEW_CONFIGURATION_STANDARD_DASHBOARD";
        public const string VIEW_FREE_FORM_REPORT = "VIEW_FREE_FORM_REPORT";
        public const string IMPORT_FREE_FORM_REPORT = "IMPORT_FREE_FORM_REPORT";
        public const string VIEW_REPORT_FROM_BSI = "VIEW_REPORT_FROM_BSI";
        public const string VIEW_REPORT_CUSTOM = "VIEW_REPORT_CUSTOM";
        public const string VIEW_CUTOFF_WORKFLOW_DAY = "VIEW_CUTOFF_WORKFLOW_DAY";
        public const string VIEW_LOADING_CSV = "VIEW_LOADING_CSV";
        public const string VIEW_LINK_POWERBI = "VIEW_LINK_POWERBI";
        public const string VIEW_CONFIGURATION_ADVANCED_2 = "VIEW_CONFIGURATION_ADVANCED_2";
        public const string VIEW_WORKFLOW_MONITORING_ORG = "VIEW_WORKFLOW_MONITORING_ORG";
        public const string VIEW_WORKFLOW_MONITORING_DAY = "VIEW_WORKFLOW_MONITORING_DAY";
    }
}