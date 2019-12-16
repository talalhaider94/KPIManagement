using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models.Dashboard
{
    public class DB_DashboardWidgetSetting
    {
        public int Id { get; set; }
        public int SettingType { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public int DashboardWidgetId { get; set; }
        public virtual DB_DashboardWidget DashboardWidget { get; set; }
    }

    public class DB_DashboardWidgetSetting_Configuration : IEntityTypeConfiguration<DB_DashboardWidgetSetting>
    {
        public void Configure(EntityTypeBuilder<DB_DashboardWidgetSetting> builder)
        {
            builder.ToTable("db_dashboard_widgets_settings");
            builder.HasKey(o => o.Id);
            builder.HasOne(o => o.DashboardWidget).WithMany(o => o.DashboardWidgetSettings).HasForeignKey(o => o.DashboardWidgetId);
        }
    }
}