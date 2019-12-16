using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models.Information
{
    public class T_UserRole
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int role_id { get; set; }
        public virtual T_Role Role { get; set; }
    }

    public class T_UserRole_Configuration : IEntityTypeConfiguration<T_UserRole>
    {
        public void Configure(EntityTypeBuilder<T_UserRole> builder)
        {
            builder.ToTable("t_user_roles");
            builder.HasKey(o => o.id);
            builder.HasOne(o => o.Role).WithMany(p => p.UserRole).HasForeignKey(q => q.role_id);
        }
    }
}