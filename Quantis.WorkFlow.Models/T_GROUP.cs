using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_Group
    {
        public int group_id { get; set; }
        public string group_description { get; set; }
        public DateTime create_date { get; set; }
        public DateTime? modify_date { get; set; }
        public DateTime? delete_date { get; set; }
    }

    public class T_Group_Configuration : IEntityTypeConfiguration<T_Group>
    {
        public void Configure(EntityTypeBuilder<T_Group> builder)
        {
            builder.ToTable("t_groups");
            builder.HasKey(o => o.group_id);
        }
    }
}