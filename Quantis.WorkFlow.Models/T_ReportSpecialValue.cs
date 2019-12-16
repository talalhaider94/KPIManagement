using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models
{
    public class T_ReportSpecialValue
    {
        public int id { get; set; }
        public int special_key { get; set; }
        public string special_value { get; set; }
        public string note { get; set; }
    }

    public class T_ReportSpecialValue_Configuration : IEntityTypeConfiguration<T_ReportSpecialValue>
    {
        public void Configure(EntityTypeBuilder<T_ReportSpecialValue> builder)
        {
            builder.ToTable("t_report_special_values");
            builder.HasKey(o => o.id);
        }
    }

}
