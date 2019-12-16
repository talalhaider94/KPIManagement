using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_EmailNotifiers
    {
        public int id { get; set; }
        public string type { get; set; }
        public string user_domain { get; set; }
        public string period { get; set; }
        public DateTime notify_date { get; set; }
        public string email_body { get; set; }
        public int id_form { get; set; }
        public string recipient { get; set; }
        public virtual T_Form Form { get; set; }
    }

    public class T_EmailNotifiers_Configuration : IEntityTypeConfiguration<T_EmailNotifiers>
    {
        public void Configure(EntityTypeBuilder<T_EmailNotifiers> builder)
        {
            builder.ToTable("t_email_notifiers");
            builder.HasKey(o => o.id);
            builder.HasOne(o => o.Form).WithMany(o => o.EmailNotifiers).IsRequired();
        }
    }
}