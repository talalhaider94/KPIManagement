using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models.Information
{
    public class T_Configuration
    {
        public int id { get; set; }
        public string owner { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public bool enable { get; set; }
        public bool isvisible { get; set; }
        public bool iseditable { get; set; }
        public string description { get; set; }
        public string category { get; set; }
    }

    public class T_Configuration_Configuration : IEntityTypeConfiguration<T_Configuration>
    {
        public void Configure(EntityTypeBuilder<T_Configuration> builder)
        {
            builder.ToTable("t_configurations");
            builder.HasKey(o => o.id);
        }
    }
}