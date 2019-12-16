using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_FormAttachment
    {
        public int t_form_attachments_id { get; set; }
        public int form_id { get; set; }
        public byte[] content { get; set; }
        public string period { get; set; }
        public int year { get; set; }
        public string doc_name { get; set; }
        public string checksum { get; set; }
        public DateTime create_date { get; set; }
        public virtual T_Form Form { get; set; }
    }

    public class T_FormAttachment_Configuration : IEntityTypeConfiguration<T_FormAttachment>
    {
        public void Configure(EntityTypeBuilder<T_FormAttachment> builder)
        {
            builder.ToTable("t_form_attachments");
            builder.HasKey(o => o.t_form_attachments_id);
            builder.HasOne(o => o.Form).WithMany(o => o.Attachments).IsRequired();
            builder.Property(o => o.create_date).HasColumnName("create_timestamp");
        }
    }
}