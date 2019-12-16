using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Models.SDM
{
    public class SDM_TicketFact
    {
        public int id { get; set; }
        public int ticket_id { get; set; }
        public int ticket_refnum { get; set; }
        public int period_month { get; set; }
        public int period_year { get; set; }
        public int global_rule_id { get; set; }
        public bool complaint { get; set; }
        public bool notcomplaint { get; set; }
        public bool notcalculated { get; set; }
        public bool refused { get; set; }
        public int customer_id { get; set; }
        public int primary_contract_party_id { get; set; }
        public int? secondary_contract_party_id { get; set; }
        public string result_value { get; set; }
        public DateTime created_on { get; set; }
        public virtual List<SDM_TicketLog> TicketLogs { get; set; }
    }

    public class SDM_TicketFact_Configuration : IEntityTypeConfiguration<SDM_TicketFact>
    {
        public void Configure(EntityTypeBuilder<SDM_TicketFact> builder)
        {
            builder.ToTable("sdm_ticket_fact");
            builder.HasKey(o => o.id);
            builder.HasMany(o => o.TicketLogs).WithOne(o => o.TicketFact).HasForeignKey(o => o.ticket_fact_id);
        }
    }
}