using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models.Dashboard
{
    public class DB_WidgetCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class DB_WidgetCategory_Configuration : IEntityTypeConfiguration<DB_WidgetCategory>
    {
        public void Configure(EntityTypeBuilder<DB_WidgetCategory> builder)
        {
            builder.ToTable("db_widget_categories");
            builder.HasKey(o => o.Id);
        }
    }
}