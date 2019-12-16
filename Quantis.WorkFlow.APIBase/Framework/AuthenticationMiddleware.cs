using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Quantis.WorkFlow.APIBase.Framework
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private static List<Tuple<string, string>> _authentications = null;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, WorkFlowPostgreSqlContext _context, ILogger<AuthenticationMiddleware> _logger, IInformationService info, IMemoryCache memoryCache)
        {
            if (_authentications == null)
            {
                _authentications = _context.Authentication.Select(o => new Tuple<string, string>(o.Username, o.Password)).ToList();
            }
            _logger.LogError(string.IsNullOrEmpty(context.Request.Headers["Authorization"].ToString()) ? "No Authorization in header" : context.Request.Headers["Authorization"].ToString());
            string token = context.Request.Headers["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string mytoken = encoding.GetString(Convert.FromBase64String(token));
                //string mytoken = token;
                var token_entity = _context.Sessions.FirstOrDefault(o => o.session_token == token && o.logout_time == null && o.expire_time > DateTime.Now);
                if (token_entity != null)
                {
                    var user_entity = _context.CatalogUsers.FirstOrDefault(o => o.ca_bsi_account == token_entity.user_name);

                    AuthUser usr = new AuthUser()
                    {
                        UserId = token_entity.user_id,
                        UserName = user_entity.ca_bsi_account,
                        SessionToken = token_entity.session_token,
                        Permissions = (List<string>)memoryCache.GetOrCreate("Permission_" + token_entity.user_id, f =>
                        {
                            var permissions = info.GetPermissionsByUserId(token_entity.user_id).Select(o => o.Code).ToList();
                            return permissions;
                        })
                    };
                    token_entity.expire_time = DateTime.Now.AddMinutes(getSessionTimeOut(_context));
                    _context.SaveChanges();
                    context.User = usr;
                }
            }
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                //Extract credentials
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                _logger.LogError(usernamePassword);
                int seperatorIndex = usernamePassword.IndexOf(':');

                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);

                if (_authentications.FirstOrDefault(o => o.Item1 == username && o.Item2 == password) != null)
                {
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.Headers["WWW-Authenticate"] = "Basic";// no authorization header
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
            }
            else
            {
                context.Response.Headers["WWW-Authenticate"] = "Basic";// no authorization header
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
        }

        private int getSessionTimeOut(WorkFlowPostgreSqlContext _context)
        {
            var session = _context.Configurations.FirstOrDefault(o => o.owner == "be_restserver" && o.key == "session_timeout");
            if (session != null)
            {
                int value = Int32.Parse(session.value);
                return value;
            }
            return 15;
        }
    }
}