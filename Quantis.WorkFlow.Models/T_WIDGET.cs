using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_Widget
    {
        public int widget_id { get; set; }
        public string widget_configuration { get; set; }
        public int user_id { get; set; }
        public int page_id { get; set; }
        public string widget_data { get; set; }
        public string chart_type { get; set; }
        public string chart_option { get; set; }
        public string chart_description { get; set; }
        public bool chart_description_see { get; set; }
        public DateTime create_date { get; set; }
        public DateTime? modify_date { get; set; }
        public DateTime? delete_date { get; set; }
    }

    public class T_Widget_Configuration : IEntityTypeConfiguration<T_Widget>
    {
        public void Configure(EntityTypeBuilder<T_Widget> builder)
        {
            builder.ToTable("t_widgets");
            builder.HasKey(o => o.widget_id);
        }
    }
}