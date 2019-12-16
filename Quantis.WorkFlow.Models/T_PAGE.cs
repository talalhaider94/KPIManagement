using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models
{
    public class T_Page
    {
        public int page_id { get; set; }
        public string page_name { get; set; }
        public int page_sequence { get; set; }
        public int user_id { get; set; }
    }

    public class T_Page_Configuration : IEntityTypeConfiguration<T_Page>
    {
        public void Configure(EntityTypeBuilder<T_Page> builder)
        {
            builder.ToTable("t_pages");
            builder.HasKey(o => o.page_id);
        }
    }
}