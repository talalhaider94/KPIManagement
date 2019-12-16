using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_FormLog
    {
        public int id { get; set; }
        public int id_form { get; set; }
        public int id_locale { get; set; }
        public int user_id { get; set; }
        public bool empty_form { get; set; }
        public string period { get; set; }
        public int year { get; set; }
        public DateTime time_stamp { get; set; }
        public virtual T_Form Form { get; set; }
    }

    public class T_FormLog_Configuration : IEntityTypeConfiguration<T_FormLog>
    {
        public void Configure(EntityTypeBuilder<T_FormLog> builder)
        {
            builder.ToTable("t_form_logs");
            builder.HasKey(o => o.id);
            builder.HasOne(o => o.Form).WithMany(o => o.FormLogs).IsRequired().HasForeignKey(p => p.id_form);
        }
    }
}