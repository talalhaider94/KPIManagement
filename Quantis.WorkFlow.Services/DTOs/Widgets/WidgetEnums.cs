using System.Collections.Generic;
using System.ComponentModel;

namespace Quantis.WorkFlow.Services.DTOs.Widgets
{
    //Always add new enum at the end never in the middle
    public enum Measures : int
    {
        [Description("Numero di ticket in KPI in Verifica")]
        Number_of_ticket_in_KPI_in_Verifica,

        [Description("Numero di ticket con KPI Compliant")]
        Number_of_ticket_of_KPI_Compliant,

        [Description("Numero di ticket con KPI Non Compliant")]
        Number_of_ticket_of_KPI_Non_Compliant,

        [Description("Numero di ticket con KPI NE")]
        Number_of_ticket_of_KPI_Non_Calcolato,

        [Description("Numero di ticket rifiutati")]
        Number_of_ticket_refused,

        [Description("Numero di contract party assegnati all'utente")]
        Number_of_contract_party_assigned_to_the_user,

        [Description("Numero di contratti assegnati all'utente")]
        Number_of_contracts_assigned_to_the_user,

        [Description("Numero di KPI assegnati all'utente")]
        Number_of_kpis_assigned_to_the_user,

        [Description("Numero di KPI totali not compliant")]
        Number_of_Total_KPI_not_compliant,

        [Description("Numero di KPI totali compliant")]
        Number_of_Total_KPI_compliant,

        [Description("Numero di KPI totali in escalation")]
        Number_of_Total_KPI_in_escalation,

        [Description("Numero di reminder ricevuti")]
        Number_of_reminder_received,

        [Description("Number of escalation type 1 received")]
        Number_of_escalation_type_1_received,

        [Description("Number of escalation type 2 received")]
        Number_of_escalation_type_2_received,

        [Description("Numero di KPI da Consolidare")]
        Pending_KPIs,

        [Description("Numero di Utenti da Consolidare")]
        Pending_Users,

        [Description("Numero Totale di KPI")]
        Number_of_Total_KPI_in_verifica,

        [Description("Numero Totale di KPI Non Calcolati")]
        Number_of_Total_KPI_non_calcolato
    }

    public static class ChartType
    {
        public static KeyValuePair<string, string> LINE = new KeyValuePair<string, string>("line", "Line");
        public static KeyValuePair<string, string> BAR = new KeyValuePair<string, string>("bar", "Bar");
        public static KeyValuePair<string, string> TABLE = new KeyValuePair<string, string>("table", "Table");
    }

    public static class AggregationOption
    {
        public static KeyValuePair<string, string> PERIOD = new KeyValuePair<string, string>("period", "Period");
        public static KeyValuePair<string, string> ANNAUL = new KeyValuePair<string, string>("annual", "Annual");
        public static KeyValuePair<string, string> TRACKINGPERIOD = new KeyValuePair<string, string>("trackingperiod", "Tracking Period");


        public static KeyValuePair<string, string> KPI = new KeyValuePair<string, string>("kpi", "KPI");
        public static KeyValuePair<string, string> CONTRACT = new KeyValuePair<string, string>("contract", "Contract");
        public static KeyValuePair<string, string> CONTRACTPARTY = new KeyValuePair<string, string>("contractparty", "Contract Party");
    }
}