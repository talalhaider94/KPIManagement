using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Quantis.WorkFlow.Services;
using System.Net;
using System.Threading.Tasks;

namespace Quantis.WorkFlow.APIBase.Framework
{
    public class QuantisPermissions : IAuthorizationRequirement
    {
        public string Permission { get; }

        public QuantisPermissions(string permission)
        {
            Permission = permission;
        }
    }

    public class QuantisPermissionHandler : AuthorizationHandler<QuantisPermissions>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       QuantisPermissions requirement)
        {
            var user = context.User as Quantis.WorkFlow.Services.Framework.AuthUser;
            var filterContext = context.Resource as AuthorizationFilterContext;
            var response = filterContext.HttpContext.Response;
            if (user == null)
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Task.CompletedTask;
            }
            else if (requirement.Permission == WorkFlowPermissions.BASIC_LOGIN || (user.Permissions != null && user.Permissions.Contains(requirement.Permission)))
            {
                context.Succeed(requirement);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
    }
}