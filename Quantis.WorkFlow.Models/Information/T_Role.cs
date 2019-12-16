using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Models.Information
{
    public class T_Role
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public DateTime created_on { get; set; }
        public virtual IList<T_RolePermission> RolePermissions { get; set; }

        public virtual IList<T_UserRole> UserRole { get; set; }
    }

    public class T_Role_Configuration : IEntityTypeConfiguration<T_Role>
    {
        public void Configure(EntityTypeBuilder<T_Role> builder)
        {
            builder.ToTable("t_roles");
            builder.HasKey(o => o.id);
            builder.HasMany(o => o.RolePermissions).WithOne(p => p.Role).HasForeignKey(q => q.role_id);
            builder.HasMany(o => o.UserRole).WithOne(p => p.Role).HasForeignKey(q => q.role_id);
        }
    }
}