using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_NotifierLog
    {
        public int id { get; set; }
        public int id_form { get; set; }
        public DateTime notify_timestamp { get; set; }
        public DateTime? remind_timestamp { get; set; }
        public bool is_ack { get; set; }
        public string period { get; set; }
        public int year { get; set; }
        public string email_body { get; set; }
        public virtual T_Form Form { get; set; }
    }

    public class T_NotifierLog_Configuration : IEntityTypeConfiguration<T_NotifierLog>
    {
        public void Configure(EntityTypeBuilder<T_NotifierLog> builder)
        {
            builder.ToTable("t_notifier_logs");
            builder.HasKey(o => o.id);
            builder.HasOne(o => o.Form).WithMany(o => o.NotifierLogs).IsRequired().HasForeignKey(p => p.id_form);
        }
    }
}