using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Quantis.WorkFlow.Models
{
    public class T_Exception
    {
        public int id { get; set; }
        public string message { get; set; }
        public string innerexceptions { get; set; }
        public string stacktrace { get; set; }
        public string loglevel { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class T_Exception_Configuration : IEntityTypeConfiguration<T_Exception>
    {
        public void Configure(EntityTypeBuilder<T_Exception> builder)
        {
            builder.ToTable("t_exceptions");
            builder.HasKey(o => o.id);
            builder.Property(o => o.timestamp).HasColumnName("ex_timestamp");
        }
    }
}