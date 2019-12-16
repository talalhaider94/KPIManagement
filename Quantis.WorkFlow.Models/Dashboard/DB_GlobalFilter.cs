using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Models.Dashboard
{
    public class DB_GlobalFilter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UserId { get; set; }
        public bool IsAll { get; set; }
        public virtual List<DB_GlobalFilterSetting> GlobalFilterSettings { get; set; }
    }

    public class DB_GlobalFilter_Configuration : IEntityTypeConfiguration<DB_GlobalFilter>
    {
        public void Configure(EntityTypeBuilder<DB_GlobalFilter> builder)
        {
            builder.ToTable("db_globalfilters");
            builder.HasKey(o => o.Id);
            builder.HasMany(o => o.GlobalFilterSettings).WithOne(o => o.GlobalFilter).HasForeignKey(o => o.GlobalFilterId);
        }
    }
}