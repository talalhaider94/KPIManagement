using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models
{
    public class T_SecurityMembers
    {
        public int user_group_id { get; set; }
        public int security_group_id { get; set; }
    }

    public class T_SecurityMembers_Configuration : IEntityTypeConfiguration<T_SecurityMembers>
    {
        public void Configure(EntityTypeBuilder<T_SecurityMembers> builder)
        {
            builder.ToTable("t_security_members");
            builder.HasKey(o => o.user_group_id);
        }
    }
}