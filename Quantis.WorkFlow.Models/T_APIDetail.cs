using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models
{
    public class T_APIDetail
    {
        public int api_id { get; set; }
        public string server_address { get; set; }
        public string api_method { get; set; }
        public string return_type { get; set; }
        public string parameters { get; set; }
        public string details { get; set; }
        public string api_type { get; set; }
        public string db_source { get; set; }
        public string table_used { get; set; }
    }

    public class T_APIDetail_Configuration : IEntityTypeConfiguration<T_APIDetail>
    {
        public void Configure(EntityTypeBuilder<T_APIDetail> builder)
        {
            builder.ToTable("t_api_details");
            builder.HasKey(o => o.api_id);
        }
    }
}