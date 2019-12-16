using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models
{
    public class T_FormUsers
    {
        public int id { get; set; }
        public int form_id { get; set; }
        public string form_name { get; set; }
        public string form_description { get; set; }
        public string reader_configuration { get; set; }
        public string form_schema { get; set; }
        public int? reader_id { get; set; }
        public int? form_owner_id { get; set; }
        public int? user_group_id { get; set; }
        public string user_group_name { get; set; }
    }

    public class T_FormUsers_Configuration : IEntityTypeConfiguration<T_FormUsers>
    {
        public void Configure(EntityTypeBuilder<T_FormUsers> builder)
        {
            builder.ToTable("t_form_users");
            builder.HasKey(o => o.id);
        }
    }
}