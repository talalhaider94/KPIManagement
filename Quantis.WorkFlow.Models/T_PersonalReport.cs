using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models
{
    public class T_PersonalReport
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string name { get; set; }
        public int global_rule_id { get; set; }
        public string report_type { get; set; }
        public string aggregation_option { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public DateTime modification_date { get; set; }
        public int? global_rule2_id { get; set; }
        public string aggregation_option2 { get; set; }
        public string start_date2 { get; set; }
        public string end_date2 { get; set; }
    }
    public class T_PersonalReport_Configuration : IEntityTypeConfiguration<T_PersonalReport>
    {
        public void Configure(EntityTypeBuilder<T_PersonalReport> builder)
        {
            builder.ToTable("t_personal_reports");
            builder.HasKey(o => o.id);
        }
    }
}
