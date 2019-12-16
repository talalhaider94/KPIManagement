using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_Session
    {
        public int session_id { get; set; }
        public string session_token { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public DateTime expire_time { get; set; }
        public DateTime login_time { get; set; }
        public DateTime? logout_time { get; set; }
    }

    public class T_Session_Configuration : IEntityTypeConfiguration<T_Session>
    {
        public void Configure(EntityTypeBuilder<T_Session> builder)
        {
            builder.ToTable("t_sessions");
            builder.HasKey(o => o.session_id);
        }
    }
}