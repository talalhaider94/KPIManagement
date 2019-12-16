using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models.SDM
{
    public class SDM_TicketLog
    {
        public int id { get; set; }
        public int ticket_fact_id { get; set; }
        public string log_type { get; set; }
        public string note { get; set; }
        public DateTime created_on { get; set; }
        public virtual SDM_TicketFact TicketFact { get; set; }
    }

    public class SDM_TicketLog_Configuration : IEntityTypeConfiguration<SDM_TicketLog>
    {
        public void Configure(EntityTypeBuilder<SDM_TicketLog> builder)
        {
            builder.ToTable("sdm_ticket_logs_new");
            builder.HasKey(o => o.id);
            builder.HasOne(o => o.TicketFact).WithMany(o => o.TicketLogs).HasForeignKey(o => o.ticket_fact_id);
        }
    }
}