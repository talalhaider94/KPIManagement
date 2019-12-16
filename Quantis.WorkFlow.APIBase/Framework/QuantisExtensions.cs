using Quantis.WorkFlow.Models;
using Quantis.WorkFlow.Services.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Quantis.WorkFlow.APIBase.Framework
{
    public static class QuantisExtensions
    {
        public static PagedList<T> GetPaged<T>(this IQueryable<T> query,
                                         PagingInfo pagingInfo) where T : class
        {
            var CurrentPage = pagingInfo.Index;
            var PageSize = pagingInfo.Size;
            var RowCount = query.Count();
            var skip = (pagingInfo.Index - 1) * pagingInfo.Size;
            if (!string.IsNullOrEmpty(pagingInfo.OrderBy))
            {
                if (pagingInfo.OrderDirection == OrderDirection.Desc)
                {
                    query = query.OrderBy(pagingInfo.OrderBy + " DESC");
                }
                else
                {
                    query = query.OrderBy(pagingInfo.OrderBy);
                }
            }

            var results = query.Skip(skip).Take(pagingInfo.Size).ToList();
            return new PagedList<T>(results, CurrentPage, PageSize, RowCount);
        }

        public static void Add(this Dictionary<string, string> dic, KeyValuePair<string, string> kp)
        {
            dic.Add(kp.Key, kp.Value);
        }

        public static void LogInformation(this WorkFlowPostgreSqlContext dbcontext, string logMessage)
        {
            try
            {
                var exception = new T_Exception()
                {
                    message = logMessage.Substring(0, Math.Min(999, logMessage.Length)),
                    stacktrace = null,
                    loglevel = "Information",
                    timestamp = DateTime.Now,
                    innerexceptions = null
                };
                dbcontext.Exceptions.Add(exception);
                dbcontext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string Bash(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}