using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_User
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public int user_locale_id { get; set; }
        public string user_status { get; set; }
        public string user_email { get; set; }
        public bool in_catalog { get; set; }
        public string user_organization_name { get; set; }
        public DateTime user_create_date { get; set; }
    }

    public class T_User_Configuration : IEntityTypeConfiguration<T_User>
    {
        public void Configure(EntityTypeBuilder<T_User> builder)
        {
            builder.ToTable("t_users");
            builder.HasKey(o => o.user_id);
        }
    }
}