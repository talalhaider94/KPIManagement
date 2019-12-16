using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Models
{
    public class T_Form
    {
        public int form_id { get; set; }
        public string status { get; set; }
        public string prev_status { get; set; }
        public string form_name { get; set; }
        public string form_description { get; set; }
        public string form_schema { get; set; }
        public int reader_id { get; set; }
        public int form_owner_id { get; set; }
        public string form_error { get; set; }
        public DateTime create_date { get; set; }
        public DateTime modify_date { get; set; }
        public virtual IList<T_CatalogKPI> CatalogKPIs { get; set; }
        public virtual IList<T_FormAttachment> Attachments { get; set; }
        public virtual IList<T_FormLog> FormLogs { get; set; }
        public virtual IList<T_FormRule> Rules { get; set; }
        public virtual IList<T_NotifierLog> NotifierLogs { get; set; }
        public virtual IList<T_EmailNotifiers> EmailNotifiers { get; set; }
    }

    public class T_Form_Configuration : IEntityTypeConfiguration<T_Form>
    {
        public void Configure(EntityTypeBuilder<T_Form> builder)
        {
            builder.ToTable("t_forms");
            builder.HasKey(o => o.form_id);
            builder.HasMany(o => o.CatalogKPIs).WithOne(p => p.Form).HasForeignKey(e => e.id_form);
            builder.HasMany(o => o.Attachments).WithOne(p => p.Form).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(o => o.FormLogs).WithOne(p => p.Form).OnDelete(DeleteBehavior.Cascade).HasForeignKey(q => q.id_form);
            builder.HasMany(o => o.Rules).WithOne(p => p.Form).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(o => o.NotifierLogs).WithOne(p => p.Form).OnDelete(DeleteBehavior.Cascade).HasForeignKey(q => q.id_form);
            builder.HasMany(o => o.EmailNotifiers).WithOne(p => p.Form).OnDelete(DeleteBehavior.Cascade).HasForeignKey(q => q.id_form);
        }
    }
}