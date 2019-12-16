using Microsoft.EntityFrameworkCore;
using Quantis.WorkFlow.Models;
using Quantis.WorkFlow.Models.Dashboard;
using Quantis.WorkFlow.Models.Information;
using Quantis.WorkFlow.Models.SDM;
using System.Linq;

namespace Quantis.WorkFlow.APIBase.Framework
{
    public class WorkFlowPostgreSqlContext : DbContext
    {
        public WorkFlowPostgreSqlContext(DbContextOptions<WorkFlowPostgreSqlContext> options) : base(options)
        {
            Database.SetCommandTimeout(150000);
        }

        public DbSet<T_Group> Groups { get; set; }
        public DbSet<T_Widget> Widgets { get; set; }
        public DbSet<T_FormUsers> FormUsers { get; set; }
        public DbSet<T_SecurityMembers> SecurityMembers { get; set; }
        public DbSet<T_Page> Pages { get; set; }
        public DbSet<T_FormRule> FormRules { get; set; }
        public DbSet<T_Form> Forms { get; set; }
        public DbSet<T_Configuration> Configurations { get; set; }
        public DbSet<T_APIAuthentication> Authentication { get; set; }
        public DbSet<T_Exception> Exceptions { get; set; }
        public DbSet<T_CatalogUser> CatalogUsers { get; set; }
        public DbSet<T_Session> Sessions { get; set; }
        public DbSet<T_CatalogKPI> CatalogKpi { get; set; }
        public DbSet<T_GlobalRule> TGlobalRules { get; set; }
        public DbSet<T_APIDetail> ApiDetails { get; set; }
        public DbSet<T_FormAttachment> FormAttachments { get; set; }
        public DbSet<T_FormLog> FormLogs { get; set; }
        public DbSet<T_NotifierLog> NotifierLogs { get; set; }

        public DbSet<T_UserRole> UserRoles { get; set; }
        public DbSet<T_Role> Roles { get; set; }
        public DbSet<T_Rule> Rules { get; set; }
        public DbSet<T_RolePermission> RolePermissions { get; set; }
        public DbSet<T_Permission> Permissions { get; set; }
        public DbSet<T_User> TUsers { get; set; }
        public DbSet<SDM_TicketStatus> SDMTicketStatus { get; set; }
        public DbSet<SDM_TicketGroup> SDMTicketGroup { get; set; }
        public DbSet<SDM_TicketLog> SDMTicketLogs { get; set; }
        public DbSet<SDM_TicketFact> SDMTicketFact { get; set; }
        public DbSet<T_User_KPI> UserKPIs { get; set; }
        public DbSet<T_Customer> Customers { get; set; }
        public DbSet<T_EmailNotifiers> EmailNotifiers { get; set; }

        public DbSet<DB_Dashboard> DB_Dashboards { get; set; }
        public DbSet<DB_DashboardWidget> DB_DashboardWidgets { get; set; }
        public DbSet<DB_DashboardWidgetSetting> DB_DashboardWidgetSettings { get; set; }
        public DbSet<DB_GlobalFilter> DB_GlobalFilters { get; set; }
        public DbSet<DB_GlobalFilterSetting> DB_GlobalFilterSettings { get; set; }
        public DbSet<DB_Widget> DB_Widgets { get; set; }
        public DbSet<DB_WidgetCategory> DB_WidgetCategories { get; set; }
        public DbSet<T_Sla> Slas { get; set; }
        public DbSet<T_UserLandingPage> UserLandingPages { get; set; }
        public DbSet<T_ReportQuery> ReportQueries { get; set; }
        public DbSet<T_ReportQueryParameter> ReportQueryParameters { get; set; }
        public DbSet<T_ReportQueryAssignment> ReportQueryAssignments { get; set; }
        public DbSet<T_UserSetting> UserSettings { get; set; }
        public DbSet<T_PersonalReport> PersonalReports { get; set; }
        public DbSet<T_OrganizationUnit> OrganizationUnits { get; set; }
        public DbSet<T_ReportSpecialValue> ReportSpecialValues { get; set; }
        public DbSet<SDM_TicketException> SDMTicketExceptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new T_Group_Configuration());
            builder.ApplyConfiguration(new T_SecurityMembers_Configuration());
            builder.ApplyConfiguration(new T_Page_Configuration());
            builder.ApplyConfiguration(new T_Widget_Configuration());
            builder.ApplyConfiguration(new T_Form_Configuration());
            builder.ApplyConfiguration(new T_FormRule_Configuration());
            builder.ApplyConfiguration(new T_APIAuthentication_Configuration());
            builder.ApplyConfiguration(new T_Configuration_Configuration());
            builder.ApplyConfiguration(new T_Exception_Configuration());
            builder.ApplyConfiguration(new T_CatalogUser_Configuration());
            builder.ApplyConfiguration(new T_Session_Configuration());
            builder.ApplyConfiguration(new T_CatalogKPI_Configuration());
            builder.ApplyConfiguration(new T_APIDetail_Configuration());
            builder.ApplyConfiguration(new T_FormAttachment_Configuration());
            builder.ApplyConfiguration(new T_FormLog_Configuration());
            builder.ApplyConfiguration(new T_NotifierLog_Configuration());
            builder.ApplyConfiguration(new T_UserRole_Configuration());
            builder.ApplyConfiguration(new T_Role_Configuration());
            builder.ApplyConfiguration(new T_RolePermission_Configuration());
            builder.ApplyConfiguration(new T_Permission_Configuration());
            builder.ApplyConfiguration(new T_Rule_Configuration());
            builder.ApplyConfiguration(new T_User_Configuration());
            builder.ApplyConfiguration(new SDM_TicketStatus_Configuration());
            builder.ApplyConfiguration(new SDM_TicketGroup_Configuration());
            builder.ApplyConfiguration(new T_User_KPI_Configuration());
            builder.ApplyConfiguration(new T_Customer_Configuration());
            builder.ApplyConfiguration(new T_EmailNotifiers_Configuration());
            builder.ApplyConfiguration(new T_GlobalRule_Configuration());
            builder.ApplyConfiguration(new T_Sla_Configuration());
            builder.ApplyConfiguration(new T_FormUsers_Configuration());
            builder.ApplyConfiguration(new SDM_TicketLog_Configuration());
            builder.ApplyConfiguration(new SDM_TicketFact_Configuration());

            builder.ApplyConfiguration(new DB_Dashboard_Configuration());
            builder.ApplyConfiguration(new DB_DashboardWidget_Configuration());
            builder.ApplyConfiguration(new DB_DashboardWidgetSetting_Configuration());
            builder.ApplyConfiguration(new DB_GlobalFilter_Configuration());
            builder.ApplyConfiguration(new DB_GlobalFilterSetting_Configuration());
            builder.ApplyConfiguration(new DB_Widget_Configuration());
            builder.ApplyConfiguration(new DB_WidgetCategory_Configuration());
            builder.ApplyConfiguration(new T_UserLandingPage_Configuration());
            builder.ApplyConfiguration(new T_ReportQuery_Configuration());
            builder.ApplyConfiguration(new T_ReportQueryParameter_Configuration());
            builder.ApplyConfiguration(new T_ReportQueryAssignment_Configuration());
            builder.ApplyConfiguration(new T_UserSetting_Configuration());
            builder.ApplyConfiguration(new T_PersonalReport_Configuration());
            builder.ApplyConfiguration(new T_OrganizationUnit_Configuration());
            builder.ApplyConfiguration(new T_ReportSpecialValue_Configuration());
            builder.ApplyConfiguration(new SDM_TicketException_Configuration());
            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            updateUpdatedProperty<T_Group>();
            updateUpdatedProperty<T_SecurityMembers>();
            updateUpdatedProperty<T_Widget>();
            updateUpdatedProperty<T_Page>();
            updateUpdatedProperty<T_Form>();
            updateUpdatedProperty<T_FormUsers>();
            updateUpdatedProperty<T_FormRule>();
            updateUpdatedProperty<T_APIAuthentication>();
            updateUpdatedProperty<T_Configuration>();
            updateUpdatedProperty<T_Exception>();
            updateUpdatedProperty<T_CatalogUser>();
            updateUpdatedProperty<T_Session>();
            updateUpdatedProperty<T_CatalogKPI>();
            updateUpdatedProperty<T_APIDetail>();
            updateUpdatedProperty<T_FormAttachment>();
            updateUpdatedProperty<T_FormLog>();
            updateUpdatedProperty<T_NotifierLog>();
            updateUpdatedProperty<T_UserRole>();
            updateUpdatedProperty<T_Role>();
            updateUpdatedProperty<T_RolePermission>();
            updateUpdatedProperty<T_Permission>();
            updateUpdatedProperty<T_Rule>();
            updateUpdatedProperty<T_User>();
            updateUpdatedProperty<SDM_TicketStatus>();
            updateUpdatedProperty<SDM_TicketGroup>();
            updateUpdatedProperty<T_User_KPI>();
            updateUpdatedProperty<T_Customer>();
            updateUpdatedProperty<T_EmailNotifiers>();
            updateUpdatedProperty<T_GlobalRule>();
            updateUpdatedProperty<T_Sla>();
            updateUpdatedProperty<SDM_TicketLog>();
            updateUpdatedProperty<SDM_TicketFact>();
            updateUpdatedProperty<DB_Dashboard>();
            updateUpdatedProperty<DB_DashboardWidget>();
            updateUpdatedProperty<DB_DashboardWidgetSetting>();
            updateUpdatedProperty<DB_GlobalFilter>();
            updateUpdatedProperty<DB_GlobalFilterSetting>();
            updateUpdatedProperty<DB_Widget>();
            updateUpdatedProperty<DB_WidgetCategory>();
            updateUpdatedProperty<T_UserLandingPage>();
            updateUpdatedProperty<T_ReportQuery>();
            updateUpdatedProperty<T_ReportQueryParameter>();
            updateUpdatedProperty<T_ReportQueryAssignment>();
            updateUpdatedProperty<T_UserSetting>();
            updateUpdatedProperty<T_PersonalReport>();
            updateUpdatedProperty<T_OrganizationUnit>();
            updateUpdatedProperty<T_ReportSpecialValue>();
            updateUpdatedProperty<SDM_TicketException>();
            return base.SaveChanges();
        }

        private void updateUpdatedProperty<T>() where T : class
        {
            var modifiedSourceInfo =
                ChangeTracker.Entries<T>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
        }
    }
}