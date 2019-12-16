using Microsoft.Extensions.Caching.Memory;
using Quantis.WorkFlow.Services.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Quantis.WorkFlow.APIBase.Framework
{
    public class SMTPService : ISMTPService
    {
        private string from;
        private string sslTrust;
        private string senderPassword;
        private string senderUsername;
        private bool startTlsEnable;
        private Int32 serverPort;
        private string serverHost;
        private bool isAuth;
        private string notifierAlias;
        private WorkFlowPostgreSqlContext _dbcontext;
        private IMemoryCache _cache;

        public SMTPService(WorkFlowPostgreSqlContext context, IMemoryCache cache)
        {
            _cache = cache;
            _dbcontext = context;
            from = _cache.GetOrCreate("SMTP_from", p => _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "notifier_from").value);
            sslTrust = _cache.GetOrCreate("SMTP_sslTrust", p => _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "ssl_trust").value);
            senderPassword = _cache.GetOrCreate("SMTP_senderPassword", p => _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "sender_password").value);
            senderUsername = _cache.GetOrCreate("SMTP_senderUsername", p => _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "sender_username").value);
            startTlsEnable = _cache.GetOrCreate("SMTP_startTlsEnable", p => bool.Parse(_dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "is_start_tls_enable").value));
            serverPort = _cache.GetOrCreate("SMTP_serverPort", p => System.Convert.ToInt32(_dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "server_port").value));
            serverHost = _cache.GetOrCreate("SMTP_serverHost", p => _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "server_host").value);
            isAuth = _cache.GetOrCreate("SMTP_isAuth", p => bool.Parse(_dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "is_auth").value));
            notifierAlias = _cache.GetOrCreate("SMTP_notifierAlias", p => _dbcontext.Configurations.FirstOrDefault(o => o.owner == "be_notifier" && o.key == "notifier_alias").value);
        }

        public bool SendEmail(string subject, string body, List<string> recipients)
        {
            try
            {
                SmtpClient client = new SmtpClient(serverHost, serverPort);

                client.Credentials = new NetworkCredential(senderUsername, senderPassword);
                client.EnableSsl = startTlsEnable;

                MailMessage mailMessage = new MailMessage();

                MailAddress fromaddr = new MailAddress(from);
                mailMessage.From = fromaddr;

                foreach (string recipient in recipients)
                {
                    mailMessage.To.Add(recipient);
                }

                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body;
                mailMessage.Subject = subject;

                client.Send(mailMessage);
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }
    }
}