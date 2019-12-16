using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantis.WorkFlow.Models
{
    public class T_ReportQueryAssignment
    {
        public int id { get; set; }
        public int query_id { get; set; }
        public int user_id { get; set; }

        [ForeignKey("query_id")]
        public virtual T_ReportQuery Query { get; set; }

        [ForeignKey("user_id")]
        public virtual T_User User { get; set; }
    }

    public class T_ReportQueryAssignment_Configuration : IEntityTypeConfiguration<T_ReportQueryAssignment>
    {
        public void Configure(EntityTypeBuilder<T_ReportQueryAssignment> builder)
        {
            builder.ToTable("t_report_query_assignment");
            builder.HasKey(o => o.id);
        }
    }
}