using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models
{
    public class T_OrganizationUnit
    {
        public int id { get; set; }
        public string organization_unit { get; set; }
    }
    public class T_OrganizationUnit_Configuration : IEntityTypeConfiguration<T_OrganizationUnit>
    {
        public void Configure(EntityTypeBuilder<T_OrganizationUnit> builder)
        {
            builder.ToTable("t_organization_units");
            builder.HasKey(o => o.id);
        }
    }
}
