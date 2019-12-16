using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Quantis.WorkFlow.Models.Information
{
    public class T_UserSetting
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }
    public class T_UserSetting_Configuration : IEntityTypeConfiguration<T_UserSetting>
    {
        public void Configure(EntityTypeBuilder<T_UserSetting> builder)
        {
            builder.ToTable("t_user_settings");
            builder.HasKey(o => o.id);
        }
    }
}
