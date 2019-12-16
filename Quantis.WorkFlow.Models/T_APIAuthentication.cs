using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models
{
    public class T_APIAuthentication
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class T_APIAuthentication_Configuration : IEntityTypeConfiguration<T_APIAuthentication>
    {
        public void Configure(EntityTypeBuilder<T_APIAuthentication> builder)
        {
            builder.ToTable("t_api_authentications");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasColumnName("id");
            builder.Property(o => o.Password).HasColumnName("password");
            builder.Property(o => o.Username).HasColumnName("username");
        }
    }
}