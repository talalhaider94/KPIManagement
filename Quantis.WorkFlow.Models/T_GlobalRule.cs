using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_GlobalRule
    {
        public int global_rule_id { get; set; }
        public string status { get; set; } //('N','U','D') -- New, Updated, Deleted
        public string prev_status { get; set; }
        public string global_rule_name { get; set; }
        public int sla_id { get; set; }
        public string is_preliminary { get; set; }
        public int? psl_instance_id { get; set; }
        public DateTime global_rule_create_date { get; set; }
        public DateTime global_rule_modify_date { get; set; }
        public string current_status_time_unit { get; set; }
        public int? current_status_interval_length { get; set; }
        public string current_status_freq_time_unit { get; set; }
        public string is_current_status { get; set; }
        public int? assigned_psl_instance_id { get; set; }
        public string global_rule_guid { get; set; }
        public int? calculation_policy_id { get; set; }
        public int? calc_policy_change_seq { get; set; }
        public string global_rule_name_key { get; set; }
        public bool in_catalog { get; set; }
        // public virtual List<T_Rule> Rules { get; set; }
    }

    public class T_GlobalRule_Configuration : IEntityTypeConfiguration<T_GlobalRule>
    {
        public void Configure(EntityTypeBuilder<T_GlobalRule> builder)
        {
            builder.ToTable("t_global_rules");
            builder.HasKey(o => o.global_rule_id);
            //builder.HasMany(o => o.Rules).WithOne(p => p.GlobalRule).HasForeignKey(r => r.global_rule_id);
        }
    }
}