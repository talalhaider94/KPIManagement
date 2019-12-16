using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_Rule
    {
        public int rule_id { get; set; }
        public string status { get; set; } //('N','U','D') -- New, Updated, Deleted
        public string prev_status { get; set; }
        public int formula_id { get; set; }
        public string rule_name { get; set; }
        public string rule_description { get; set; }
        public int sla_version_id { get; set; }
        public int application_id { get; set; }
        public int compound_ts_id { get; set; }
        public int domain_category_id { get; set; }
        public int service_level_target { get; set; }
        public string rule_period_time_unit { get; set; }
        public int rule_period_interval_length { get; set; }
        public string is_effective { get; set; }
        public int is_clustered { get; set; }
        public int is_all_items { get; set; }
        public int cluster_id { get; set; }
        public int locale_id { get; set; }
        public int global_rule_id { get; set; }
        public string hour_tu_calc_status { get; set; }
        public string day_tu_calc_status { get; set; }
        public string week_tu_calc_status { get; set; }
        public string month_tu_calc_status { get; set; }
        public string quarter_tu_calc_status { get; set; }
        public string year_tu_calc_status { get; set; }
        public int psl_rule_id { get; set; }
        public DateTime rule_create_date { get; set; }
        public DateTime rule_modify_date { get; set; }
        public string objective_statement { get; set; }
        public string is_os_compiled { get; set; }
        public string is_param_compiled { get; set; }
        public string objective_statement_text { get; set; }
        public int? parent_rule_id { get; set; }
        public int metric_type_id { get; set; }
        public int? main_indicator { get; set; }
        public string is_shadow_id { get; set; }
        public int unit_id { get; set; }
        public int section_id { get; set; }
        public int is_target_dynamic { get; set; }
        public int? unit_of_consumption_id { get; set; }
        public int is_forecasted { get; set; }
        public string is_registrations_compiled { get; set; }
        public int is_cluster_recursive { get; set; }
        public int is_use_all_nodes { get; set; }
        public string is_mandatory { get; set; }
        public string measurability_status { get; set; }
        public string is_dirty { get; set; }
        public string is_parameters_dirty { get; set; }
        public bool in_catalog { get; set; }
        //public virtual T_GlobalRule GlobalRule { get; set; }
    }

    public class T_Rule_Configuration : IEntityTypeConfiguration<T_Rule>
    {
        public void Configure(EntityTypeBuilder<T_Rule> builder)
        {
            builder.ToTable("t_rules");
            builder.HasKey(o => o.rule_id);
            //builder.HasOne(o => o.GlobalRule).WithMany(p => p.Rules).HasForeignKey(r => r.global_rule_id);
        }
    }
}