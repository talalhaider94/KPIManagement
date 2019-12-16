using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Models.Dashboard
{
    public class DB_DashboardWidget
    {
        public int Id { get; set; }
        public int WidgetId { get; set; }
        public int DashboardId { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public string WidgetName { get; set; }
        public string Note { get; set; }
        public virtual DB_Widget Widget { get; set; }
        public virtual DB_Dashboard Dashboard { get; set; }
        public virtual List<DB_DashboardWidgetSetting> DashboardWidgetSettings { get; set; }
    }

    public class DB_DashboardWidget_Configuration : IEntityTypeConfiguration<DB_DashboardWidget>
    {
        public void Configure(EntityTypeBuilder<DB_DashboardWidget> builder)
        {
            builder.ToTable("db_dashboard_widgets");
            builder.HasKey(o => o.Id);
            builder.HasOne(o => o.Widget).WithMany(o => o.DashboardWidgets).HasForeignKey(o => o.WidgetId).IsRequired(true);
            builder.HasOne(o => o.Dashboard).WithMany(o => o.DashboardWidgets).HasForeignKey(o => o.DashboardId).IsRequired(true);
            builder.HasMany(o => o.DashboardWidgetSettings).WithOne(o => o.DashboardWidget).HasForeignKey(o => o.DashboardWidgetId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}