using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantis.WorkFlow.Models.Dashboard
{
    public class DB_Widget
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string Help { get; set; }
        public DateTime CreatedOn { get; set; }

        [ForeignKey("WidgetCategoryId")]
        public virtual DB_WidgetCategory WidgetCategory { get; set; }

        public int WidgetCategoryId { get; set; }
        public string IconURL { get; set; }
        public string UIIdentifier { get; set; }
        public bool IsActive { get; set; }
        public virtual List<DB_DashboardWidget> DashboardWidgets { get; set; }
    }

    public class DB_Widget_Configuration : IEntityTypeConfiguration<DB_Widget>
    {
        public void Configure(EntityTypeBuilder<DB_Widget> builder)
        {
            builder.ToTable("db_widgets");
            builder.HasKey(o => o.Id);
            builder.HasOne(o => o.WidgetCategory);
            builder.HasMany(o => o.DashboardWidgets).WithOne(o => o.Widget).HasForeignKey(o => o.WidgetId);
        }
    }
}