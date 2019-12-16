using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models.Information
{
    public class T_RolePermission
    {
        public int Id { get; set; }
        public int role_id { get; set; }
        public int permission_id { get; set; }
        public virtual T_Role Role { get; set; }
        public virtual T_Permission Permission { get; set; }
    }

    public class T_RolePermission_Configuration : IEntityTypeConfiguration<T_RolePermission>
    {
        public void Configure(EntityTypeBuilder<T_RolePermission> builder)
        {
            builder.ToTable("t_role_permissions");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasColumnName("id");
            builder.HasOne(o => o.Role).WithMany(p => p.RolePermissions).HasForeignKey(q => q.role_id);
            builder.HasOne(o => o.Permission).WithMany(p => p.RolePermissions).HasForeignKey(q => q.permission_id);
        }
    }
}