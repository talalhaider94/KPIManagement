using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantis.WorkFlow.Models
{
    public class T_CatalogKPI
    {
        public int id { get; set; }
        public string short_name { get; set; }
        public string group_type { get; set; }
        public string id_kpi { get; set; }
        public string id_alm { get; set; }
        public int? id_form { get; set; }
        public string kpi_description { get; set; }
        public string kpi_computing_description { get; set; }
        public string source_type { get; set; }
        public string computing_variable { get; set; }
        public string computing_mode { get; set; }
        public string tracking_period { get; set; }
        public string measure_unit { get; set; }
        public string kpi_type { get; set; }
        public string escalation { get; set; }
        public string target { get; set; }
        public string penalty_value { get; set; }
        public string source_name { get; set; }
        public string organization_unit { get; set; }
        public string id_booklet { get; set; }
        public string file_name { get; set; }
        public string file_path { get; set; }
        public string referent { get; set; }
        public string referent_1 { get; set; }
        public string referent_2 { get; set; }
        public string referent_3 { get; set; }
        public string referent_4 { get; set; }
        public string frequency { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public string daytrigger { get; set; }
        public string monthtrigger { get; set; }
        public bool enable { get; set; }
        public bool enable_wf { get; set; }
        public bool enable_rm { get; set; }
        public string contract { get; set; }
        public string wf_last_sent { get; set; }
        public string rm_last_sent { get; set; }
        public string supply { get; set; }
        public int day_cutoff { get; set; }
        public int day_workflow { get; set; }
        public int primary_contract_party { get; set; }
        public int? secondary_contract_party { get; set; }
        public string kpi_name_bsi { get; set; }
        public int global_rule_id_bsi { get; set; }
        public int sla_id_bsi { get; set; }
        public bool progressive { get; set; }
        public string kpi_category { get; set; }
        public virtual T_Form Form { get; set; }

        [ForeignKey("sla_id_bsi")]
        public virtual T_Sla Sla { get; set; }

        [ForeignKey("primary_contract_party")]
        public virtual T_Customer PrimaryCustomer { get; set; }

        [ForeignKey("secondary_contract_party")]
        public virtual T_Customer SecondaryCustomer { get; set; }

        [ForeignKey("global_rule_id_bsi")]
        public virtual T_GlobalRule GlobalRule { get; set; }
    }

    public class T_CatalogKPI_Configuration : IEntityTypeConfiguration<T_CatalogKPI>
    {
        public void Configure(EntityTypeBuilder<T_CatalogKPI> builder)
        {
            builder.ToTable("t_catalog_kpis");
            builder.HasKey(o => o.id);
            builder.HasOne(o => o.Form).WithMany(p => p.CatalogKPIs).HasForeignKey(r => r.id_form);
            builder.HasOne(o => o.PrimaryCustomer);
            builder.HasOne(o => o.SecondaryCustomer);
            builder.HasOne(o => o.GlobalRule);
            builder.HasOne(o => o.Sla);
        }
    }
}