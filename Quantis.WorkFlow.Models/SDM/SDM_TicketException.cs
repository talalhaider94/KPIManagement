using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models.SDM
{
    public class SDM_TicketException
    {
        public int id { get; set; }
        public int global_rule_id { get; set; }
        public int period_month { get; set; }
        public int period_year { get; set; }
        public string exception_text { get; set; }
    }
    public class SDM_TicketException_Configuration : IEntityTypeConfiguration<SDM_TicketException>
    {
        public void Configure(EntityTypeBuilder<SDM_TicketException> builder)
        {
            builder.ToTable("sdm_ticket_exceptions");
            builder.HasKey(o => o.id);
        }
    }
}
