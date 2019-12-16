using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Quantis.WorkFlow.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Quantis.WorkFlow.APIBase.Framework
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, WorkFlowPostgreSqlContext dbcontext)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, dbcontext);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex, WorkFlowPostgreSqlContext dbcontext)
        {
            try
            {
                var exception = new T_Exception()
                {
                    message = ex.Message.Substring(0, Math.Min(999, ex.Message.Length)),
                    stacktrace = ex.StackTrace.Substring(0, Math.Min(999, ex.StackTrace.Length)),
                    loglevel = null,
                    timestamp = DateTime.Now
                };
                var inner_exception = ex.InnerException;
                while (inner_exception != null)
                {
                    exception.innerexceptions += ">>>>>>" + inner_exception.Message;
                    inner_exception = inner_exception.InnerException;
                }
                exception.innerexceptions = (exception.innerexceptions != null) ? exception.innerexceptions.Substring(0, Math.Min(1000, exception.innerexceptions.Length)) : null;
                dbcontext.Exceptions.Add(exception);
                dbcontext.SaveChanges();
            }
            catch (Exception e)
            {
            }
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                var results = JsonConvert.SerializeObject(new { error = "Login not found!" });
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(results);
            }
            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                var results = JsonConvert.SerializeObject(new { error = "Permission denied! Forbidden" });
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(results);
            }
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            var result = JsonConvert.SerializeObject(new { error = "Some Error has occurred in API check logs or contact administrator" });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}