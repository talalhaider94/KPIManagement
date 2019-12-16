using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quantis.WorkFlow.Models
{
    public class T_UserLandingPage
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public bool show_landingpage { get; set; }
        public bool selected_landingpage { get; set; }
    }

    public class T_UserLandingPage_Configuration : IEntityTypeConfiguration<T_UserLandingPage>
    {
        public void Configure(EntityTypeBuilder<T_UserLandingPage> builder)
        {
            builder.ToTable("t_user_landingpages");
            builder.HasKey(o => o.id);
        }
    }
}