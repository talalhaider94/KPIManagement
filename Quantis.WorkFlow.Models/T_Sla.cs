using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_Sla
    {
        public int sla_id { get; set; }
        public string status { get; set; } //('N','U','D') -- New, Updated, Deleted
        public string prev_status { get; set; }
        public int? customer_id { get; set; }
        public string sla_name { get; set; }
        public string sla_status { get; set; }
        public int? sla_versions { get; set; }
        public DateTime? sla_valid_from { get; set; }
        public DateTime? sla_valid_to { get; set; }
        public DateTime? last_archived { get; set; }
        public string is_data_to_be_purged { get; set; }
        public string is_penalty_purge_completed { get; set; }
        public string is_reportable { get; set; }
        public string is_psl_purge_completed { get; set; }
        public int? current_version_id { get; set; }
        public string current_version_status { get; set; }
        public int? latest_version_id { get; set; }
        public string latest_version_status { get; set; }
        public int? locale_id { get; set; }
        public DateTime? create_date { get; set; }
        public DateTime? modify_date { get; set; }
        public string workflow_status { get; set; }
        public string workflow { get; set; }
        public string approver_address { get; set; }
        public string author_address { get; set; }
        public int? precalc_interval { get; set; }
        public int? sla_type_id { get; set; }
        public int? additional_customer_id { get; set; }
        public int? customer_category_id { get; set; }
        public DateTime? calculation_freeze_date { get; set; }
        public int? committed_version_seq_num { get; set; }
    }

    public class T_Sla_Configuration : IEntityTypeConfiguration<T_Sla>
    {
        public void Configure(EntityTypeBuilder<T_Sla> builder)
        {
            builder.ToTable("t_slas");
            builder.HasKey(o => o.sla_id);
        }
    }
}