using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models.SDM
{
    public class SDM_TicketGroup
    {
        public int id { get; set; }
        public string handle { get; set; }
        public string name { get; set; }
        public int category_id { get; set; }
        public virtual T_Customer category { get; set; }
        public int step { get; set; }
    }

    public class SDM_TicketGroup_Configuration : IEntityTypeConfiguration<SDM_TicketGroup>
    {
        public void Configure(EntityTypeBuilder<SDM_TicketGroup> builder)
        {
            builder.ToTable("sdm_ticket_group");
            builder.HasKey(o => o.id);
            builder.HasOne(o => o.category).WithMany(p => p.sdm_groups).HasForeignKey(q => q.category_id);
        }
    }
}