using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantis.WorkFlow.Models.Dashboard
{
    public class DB_Dashboard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual T_User User { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public int? GlobalFilterId { get; set; }

        [ForeignKey("GlobalFilterId")]
        public virtual DB_GlobalFilter GlobalFilter { get; set; }

        public virtual List<DB_DashboardWidget> DashboardWidgets { get; set; }
    }

    public class DB_Dashboard_Configuration : IEntityTypeConfiguration<DB_Dashboard>
    {
        public void Configure(EntityTypeBuilder<DB_Dashboard> builder)
        {
            builder.ToTable("db_dashboards");
            builder.HasKey(o => o.Id);
            builder.HasOne(o => o.GlobalFilter);
            builder.HasOne(o => o.User);
            builder.HasMany(o => o.DashboardWidgets).WithOne(o => o.Dashboard).HasForeignKey(o => o.DashboardId);
        }
    }
}