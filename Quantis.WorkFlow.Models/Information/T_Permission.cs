using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Models.Information
{
    public class T_Permission
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string category { get; set; }
        public string permission_type { get; set; }
        public DateTime created_on { get; set; }
        public virtual IList<T_RolePermission> RolePermissions { get; set; }
    }

    public class T_Permission_Configuration : IEntityTypeConfiguration<T_Permission>
    {
        public void Configure(EntityTypeBuilder<T_Permission> builder)
        {
            builder.ToTable("t_permissions");
            builder.HasKey(o => o.id);
            builder.HasMany(o => o.RolePermissions).WithOne(p => p.Permission).HasForeignKey(q => q.permission_id);
        }
    }
}