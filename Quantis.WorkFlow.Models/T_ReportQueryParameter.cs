using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantis.WorkFlow.Models
{
    public class T_ReportQueryParameter
    {
        public int id { get; set; }
        public int query_id { get; set; }
        public string parameter_key { get; set; }
        public string parameter_value { get; set; }

        [ForeignKey("query_id")]
        public virtual T_ReportQuery Query { get; set; }
    }

    public class T_ReportQueryParameter_Configuration : IEntityTypeConfiguration<T_ReportQueryParameter>
    {
        public void Configure(EntityTypeBuilder<T_ReportQueryParameter> builder)
        {
            builder.ToTable("t_report_query_parameters");
            builder.HasKey(o => o.id);
        }
    }
}