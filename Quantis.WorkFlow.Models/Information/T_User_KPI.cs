using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models.Information
{
    public class T_User_KPI
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int global_rule_id { get; set; }
    }

    public class T_User_KPI_Configuration : IEntityTypeConfiguration<T_User_KPI>
    {
        public void Configure(EntityTypeBuilder<T_User_KPI> builder)
        {
            builder.ToTable("t_user_kpis");
            builder.HasKey(o => o.id);
        }
    }
}