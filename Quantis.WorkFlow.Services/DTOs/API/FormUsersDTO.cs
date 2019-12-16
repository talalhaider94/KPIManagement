using System;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class FormUsersDTO
    {
        public int? id { get; set; }
        public int form_id { get; set; }
        public string form_name { get; set; }
        public string form_description { get; set; }

        // public string reader_configuration { get; set; }
        public int? AttachmentsCount { get; set; }

        public string form_schema { get; set; }
        public int? reader_id { get; set; }
        public int? form_owner_id { get; set; }
        public int? user_group_id { get; set; }
        public string user_group_name { get; set; }
        public int day_cutoff { get; set; }
        public bool cutoff { get; set; }
        public ReaderConfiguration reader_configuration { get; set; }
        public DateTime latest_input_date { get; set; }
        public KpisAssociated kpis_associated { get; set; }
    }

    public class KpisAssociated
    {
        public List<KPIContractDTO> Kpis_Associated { get; set; }
    }

    public class ReaderConfiguration
    {
        public List<FormField> inputformatfield { get; set; }
    }

    public class FormField
    {
        public string name { get; set; }
        public string type { get; set; }
        public string a_type { get; set; }
        public string source { get; set; }
        public string label { get; set; }
        public string mandatory { get; set; }
        public string defaultValue { get; set; }
    }

    public class KPIContractDTO
    {
        public string id_kpi { get; set; }
        public string contract { get; set; }
        public int global_rule_id { get; set; }
        public string kpi_name_bsi { get; set; }
        public string target { get; set; }
    }
}