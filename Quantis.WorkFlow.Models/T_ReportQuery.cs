using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantis.WorkFlow.Models
{
    public class T_ReportQuery
    {
        public int id { get; set; }
        public string query_name { get; set; }
        public string query_text { get; set; }
        public int owner_id { get; set; }
        public string folder_name { get; set; }
        public string folder_owner_name { get; set; }
        public DateTime created_on { get; set; }
        public bool is_enable { get; set; }

        [ForeignKey("owner_id")]
        public virtual T_User Owner { get; set; }

        public virtual List<T_ReportQueryParameter> Parameters { get; set; }
    }

    public class T_ReportQuery_Configuration : IEntityTypeConfiguration<T_ReportQuery>
    {
        public void Configure(EntityTypeBuilder<T_ReportQuery> builder)
        {
            builder.ToTable("t_report_queries");
            builder.HasKey(o => o.id);
            builder.HasMany(o => o.Parameters).WithOne(o => o.Query).HasForeignKey(o => o.query_id);
        }
    }
}