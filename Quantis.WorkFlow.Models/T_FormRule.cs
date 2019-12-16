using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_FormRule
    {
        public int id { get; set; }
        public int form_id { get; set; }
        public string form_body { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public virtual T_Form Form { get; set; }
    }

    public class T_FormRule_Configuration : IEntityTypeConfiguration<T_FormRule>
    {
        public void Configure(EntityTypeBuilder<T_FormRule> builder)
        {
            builder.ToTable("t_form_rules");
            builder.HasKey(o => o.id);
            builder.Property(o => o.form_body).HasColumnType("jsonb");
            builder.HasOne(o => o.Form).WithMany(o => o.Rules).IsRequired();
        }
    }
}